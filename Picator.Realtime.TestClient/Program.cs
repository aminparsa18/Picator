using Grpc.Core;
using Grpc.Net.Client;
using MagicOnion.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Picator.Common.Data.Enums;
using Picator.Common.Helpers;
using Picator.Configuration.Extensions;
using Picator.Data;
using Picator.Entities.Identity;
using Picator.Realtime.Common.Services;
using Picator.Repository;
using Picator.Service.Contracts.Identity;
using Picator.Service.Services.Identity;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Npgsql;

var options = CliOptions.Parse(args);
if (options == null)
    return 1;

var services = new ServiceCollection();
services.AddDbContext<ApplicationDbContext>(o => o.UseNpgsql(options.PgConnectionString));
services.AddTransient<IDbConnection>(_ => new NpgsqlConnection(options.PgConnectionString));
services.AddScoped<IUnitOfWork, UnitOfWork>();
services.AddTransient<ITokenService, TokenService>();

var config = new ConfigurationBuilder()
    .AddInMemoryCollection(new Dictionary<string, string?>
    {
        ["Jwt:Secret"] = options.JwtSecret,
        ["Jwt:TokenLifeTime"] = "01:00:00"
    })
    .Build();
services.AddJwtBearerAuthentication(config);

await using var provider = services.BuildServiceProvider();
await using var scope = provider.CreateAsyncScope();
var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
var tokenService = scope.ServiceProvider.GetRequiredService<ITokenService>();
var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

var results = new List<(string Name, bool Passed, string Detail)>();

async Task RunAsync(string name, Func<Task<(string, bool, string)>> scenario)
{
    Console.WriteLine($"--- Running '{name}' ---");
    try
    {
        results.Add(await scenario());
    }
    catch (Exception ex)
    {
        results.Add((name, false, $"Threw {ex.GetType().Name}: {ex.Message}"));
    }
}

if (options.Scenarios.Contains("pair"))
    await RunAsync("pair", () => Scenarios.RunPairScenario(db, tokenService, options));

if (options.Scenarios.Contains("unauthorized"))
    await RunAsync("unauthorized", () => Scenarios.RunUnauthorizedScenario(options));

if (options.Scenarios.Contains("cancel"))
    await RunAsync("cancel", () => Scenarios.RunCancelScenario(db, tokenService, unitOfWork, options));

if (options.Scenarios.Contains("expire"))
    await RunAsync("expire", () => Scenarios.RunExpireScenario(db, tokenService, unitOfWork, options));

Console.WriteLine();
Console.WriteLine("=== Summary ===");
foreach (var (name, passed, detail) in results)
    Console.WriteLine($"[{(passed ? "PASS" : "FAIL")}] {name} - {detail}");

return results.All(r => r.Passed) ? 0 : 1;

sealed class CliOptions
{
    public required string PgConnectionString { get; init; }
    public required string RealtimeUrl { get; init; }
    public required string JwtSecret { get; init; }
    public required HashSet<string> Scenarios { get; init; }

    public static CliOptions? Parse(string[] args)
    {
        string? pg = null, realtime = null, jwtSecret = null, scenario = null;
        for (var i = 0; i < args.Length - 1; i++)
        {
            switch (args[i])
            {
                case "--pg": pg = args[++i]; break;
                case "--realtime": realtime = args[++i]; break;
                case "--jwt-secret": jwtSecret = args[++i]; break;
                case "--scenario": scenario = args[++i]; break;
            }
        }

        if (pg == null)
        {
            Console.WriteLine("""
                Usage: dotnet run -- --pg "<postgres connection string>" [--realtime <url>] [--jwt-secret <secret>] [--scenario pair,cancel,unauthorized,expire]

                --pg           Required. Postgres connection string for PicatorDB (copy from the Aspire dashboard's "sql" resource).
                --realtime     Default: https://localhost:5205 (Picator.Realtime's launchSettings HTTPS port; check the Aspire dashboard for the actual bound port).
                --jwt-secret   Default: the dev secret from appsettings.json (npvFXTOBcSnzwZV8rpc1xBn61mFfqH5Y). Must match Picator.Realtime's Jwt:Secret.
                --scenario     Default: pair,cancel,unauthorized. Add "expire" to also run the 65s TTL-expiry check.
                """);
            return null;
        }

        return new CliOptions
        {
            PgConnectionString = pg,
            RealtimeUrl = realtime ?? "https://localhost:5205",
            JwtSecret = jwtSecret ?? "npvFXTOBcSnzwZV8rpc1xBn61mFfqH5Y",
            Scenarios = (scenario ?? "pair,cancel,unauthorized").Split(',', StringSplitOptions.TrimEntries).ToHashSet()
        };
    }
}

static class TestIdentity
{
    public static async Task<(Guid UserId, string Jwt)> CreateUserAndTokenAsync(ApplicationDbContext db, ITokenService tokenService, string displayName)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            UserName = $"{displayName}-{RandomHelper.CreateRandomText(6)}",
            Code = RandomHelper.CreateRandomText(10),
            DisplayName = displayName,
            EmailConfirmed = true,
            Score = 100
        };
        user.Email = user.UserName;
        db.Users.Add(user);
        await db.SaveChangesAsync();

        var claims = new List<Claim>
        {
            new(ClaimTypes.Role, Picator.Data.Constants.PlayerRole),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.Name, user.Id.ToString())
        };
        var token = tokenService.GenerateAccessToken(user, claims);
        return (user.Id, token.Token);
    }
}

sealed class TestReceiver : IMatchFoundReceiver
{
    public readonly TaskCompletionSource<string?> MatchFound = new();
    public readonly TaskCompletionSource QueueExpired = new();

    public void OnMatchFound(string gameCode) => MatchFound.TrySetResult(gameCode);
    public void OnQueueExpired() => QueueExpired.TrySetResult();
}

static class HubConnection
{
    public static async Task<(IMatchmakingHub Hub, TestReceiver Receiver, GrpcChannel Channel)> ConnectAsync(string realtimeUrl, string? jwt)
    {
        var channel = GrpcChannel.ForAddress(realtimeUrl);
        var receiver = new TestReceiver();
        var callOptions = jwt == null
            ? default
            : new CallOptions(headers: new Metadata { { "Authorization", $"Bearer {jwt}" } });

        var hub = await StreamingHubClient.ConnectAsync<IMatchmakingHub, IMatchFoundReceiver>(channel, receiver, option: callOptions);
        return (hub, receiver, channel);
    }
}

static class Scenarios
{
    private static readonly TimeSpan MatchTimeout = TimeSpan.FromSeconds(10);

    public static async Task<(string, bool, string)> RunPairScenario(ApplicationDbContext db, ITokenService tokenService, CliOptions options)
    {
        var (userIdA, jwtA) = await TestIdentity.CreateUserAndTokenAsync(db, tokenService, "PairA");
        var (userIdB, jwtB) = await TestIdentity.CreateUserAndTokenAsync(db, tokenService, "PairB");

        var (hubA, receiverA, channelA) = await HubConnection.ConnectAsync(options.RealtimeUrl, jwtA);
        var resolvedA = await hubA.EnterQueueAsync(GameFormat.Solo);

        await Task.Delay(200);

        var (hubB, receiverB, channelB) = await HubConnection.ConnectAsync(options.RealtimeUrl, jwtB);
        var resolvedB = await hubB.EnterQueueAsync(GameFormat.Solo);

        try
        {
            var matchA = await Task.WhenAny(receiverA.MatchFound.Task, Task.Delay(MatchTimeout)) == receiverA.MatchFound.Task
                ? await receiverA.MatchFound.Task
                : null;
            var matchB = await Task.WhenAny(receiverB.MatchFound.Task, Task.Delay(MatchTimeout)) == receiverB.MatchFound.Task
                ? await receiverB.MatchFound.Task
                : null;

            if (resolvedA != userIdA || resolvedB != userIdB)
                return ("pair", false, $"EnterQueueAsync returned a userId that didn't match the JWT claim (spoofing guard broken): A={resolvedA} expected {userIdA}, B={resolvedB} expected {userIdB}");

            if (matchA == null || matchB == null)
                return ("pair", false, $"Timed out waiting for OnMatchFound (A={matchA ?? "null"}, B={matchB ?? "null"})");

            if (matchA != matchB)
                return ("pair", false, $"Players received different game codes: A={matchA}, B={matchB}");

            return ("pair", true, $"Both players matched into game {matchA}");
        }
        finally
        {
            await hubA.DisposeAsync();
            await hubB.DisposeAsync();
            await channelA.ShutdownAsync();
            await channelB.ShutdownAsync();
        }
    }

    public static async Task<(string, bool, string)> RunUnauthorizedScenario(CliOptions options)
    {
        var channel = GrpcChannel.ForAddress(options.RealtimeUrl);
        try
        {
            var receiver = new TestReceiver();
            IMatchmakingHub hub;
            try
            {
                hub = await StreamingHubClient.ConnectAsync<IMatchmakingHub, IMatchFoundReceiver>(channel, receiver);
            }
            catch (RpcException ex) when (ex.StatusCode is StatusCode.Unauthenticated or StatusCode.PermissionDenied)
            {
                return ("unauthorized", true, $"Connection correctly rejected with {ex.StatusCode}");
            }

            try
            {
                await hub.EnterQueueAsync(GameFormat.Solo);
                return ("unauthorized", false, "EnterQueueAsync succeeded without a token - [Authorize] is not enforced");
            }
            catch (RpcException ex) when (ex.StatusCode is StatusCode.Unauthenticated or StatusCode.PermissionDenied)
            {
                return ("unauthorized", true, $"EnterQueueAsync correctly rejected with {ex.StatusCode}");
            }
            finally
            {
                await hub.DisposeAsync();
            }
        }
        finally
        {
            await channel.ShutdownAsync();
        }
    }

    public static async Task<(string, bool, string)> RunCancelScenario(ApplicationDbContext db, ITokenService tokenService, IUnitOfWork unitOfWork, CliOptions options)
    {
        var (userId, jwt) = await TestIdentity.CreateUserAndTokenAsync(db, tokenService, "Cancel");
        var (hub, _, channel) = await HubConnection.ConnectAsync(options.RealtimeUrl, jwt);
        try
        {
            await hub.EnterQueueAsync(GameFormat.Solo);
            await hub.CancelQueueAsync();

            var ticket = await unitOfWork.MatchTicket.Get(t => t.UserId == userId);
            if (ticket == null)
                return ("cancel", false, "No ticket found for user after cancel");

            if (ticket.Status != MatchTicketStatus.Cancelled)
                return ("cancel", false, $"Expected status Cancelled, got {ticket.Status}");

            await Task.Delay(TimeSpan.FromSeconds(5));
            var recheck = await unitOfWork.MatchTicket.Get(t => t.UserId == userId);
            if (recheck?.Status != MatchTicketStatus.Cancelled)
                return ("cancel", false, $"Ticket status changed after cancel (sweep picked it up?): {recheck?.Status}");

            return ("cancel", true, "Ticket correctly cancelled and left untouched by the sweep");
        }
        finally
        {
            await hub.DisposeAsync();
            await channel.ShutdownAsync();
        }
    }

    public static async Task<(string, bool, string)> RunExpireScenario(ApplicationDbContext db, ITokenService tokenService, IUnitOfWork unitOfWork, CliOptions options)
    {
        var (userId, jwt) = await TestIdentity.CreateUserAndTokenAsync(db, tokenService, "Expire");
        var (hub, _, channel) = await HubConnection.ConnectAsync(options.RealtimeUrl, jwt);
        try
        {
            await hub.EnterQueueAsync(GameFormat.Solo);
            Console.WriteLine("Queued alone; waiting 65s for the sweep to expire the ticket...");
            await Task.Delay(TimeSpan.FromSeconds(65));

            var ticket = await unitOfWork.MatchTicket.Get(t => t.UserId == userId);
            return ticket?.Status == MatchTicketStatus.Expired
                ? ("expire", true, "Ticket expired after TTL as expected")
                : ("expire", false, $"Expected status Expired, got {ticket?.Status.ToString() ?? "null"}");
        }
        finally
        {
            await hub.DisposeAsync();
            await channel.ShutdownAsync();
        }
    }
}
