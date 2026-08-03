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
using Picator.Entities.Models;
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

if (options.Scenarios.Contains("live-bot"))
    await RunAsync("live-bot", () => Scenarios.RunLiveBotScenario(db, tokenService, options));

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
                Usage: dotnet run -- --pg "<postgres connection string>" [--realtime <url>] [--jwt-secret <secret>] [--scenario pair,cancel,unauthorized,expire,live-bot]

                --pg           Required. Postgres connection string for PicatorDB (copy from the Aspire dashboard's "sql" resource).
                --realtime     Default: https://localhost:5205 (Picator.Realtime's launchSettings HTTPS port; check the Aspire dashboard for the actual bound port).
                --jwt-secret   Default: the dev secret from appsettings.json (npvFXTOBcSnzwZV8rpc1xBn61mFfqH5Y). Must match Picator.Realtime's Jwt:Secret.
                --scenario     Default: pair,cancel,unauthorized. Add "expire" to also run the 65s TTL-expiry check.
                               Add "live-bot" (not in the default set - must be requested explicitly) to play the
                               second player interactively against a REAL device across every round until the
                               game completes: it prompts you to start Quick Match first (so you queue older and
                               become round 1's drawer), then for each round either draws a few scripted lines
                               itself (bot's turn as drawer - not aiming for art, just exercising the sync path)
                               or prompts you before submitting a correct guess (bot's turn as guesser, reading
                               the answer straight from Postgres, never over the wire), looping until the game
                               reports GameCompleted.
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
    public readonly TaskCompletionSource<(string GameCode, bool IsDrawer)> MatchFound = new();
    public readonly TaskCompletionSource QueueExpired = new();

    public void OnMatchFound(string gameCode, bool isDrawer) => MatchFound.TrySetResult((gameCode, isDrawer));
    public void OnQueueExpired() => QueueExpired.TrySetResult();
}

sealed class BotGameReceiver : IGameDrawingReceiver
{
    private TaskCompletionSource<(bool WasCorrect, string Word, int Points, bool GameCompleted, int DrawerScore, int GuesserScore)> _roundEndedTcs = new();
    public int PointsReceived { get; private set; }
    public int LinesCompleted { get; private set; }

    /// <summary>Call before taking this round's action (join/draw/guess), so a fresh completion is armed for it.</summary>
    public void ResetRoundEnded() => _roundEndedTcs = new();

    public Task<(bool WasCorrect, string Word, int Points, bool GameCompleted, int DrawerScore, int GuesserScore)> WaitForRoundEndedAsync() => _roundEndedTcs.Task;

    public void OnPlayerJoined() => Console.WriteLine("  [bot] other player joined the game group.");
    public void OnPlayerLeft() => Console.WriteLine("  [bot] the other player left.");
    public void OnGameWordReceived(string? word) { }
    public void OnPointAdded(float x, float y) => PointsReceived++;
    public void OnColorChanged(uint color) => Console.WriteLine($"  [bot] drawer changed color to {color:X6}.");
    public void OnThicknessChanged(float thickness) => Console.WriteLine($"  [bot] drawer changed line thickness to {thickness}.");
    public void OnLineCompleted()
    {
        LinesCompleted++;
        Console.WriteLine($"  [bot] drawer completed a stroke (line #{LinesCompleted}, {PointsReceived} points received so far).");
    }
    public void OnRoundEnded(bool wasCorrect, string word, int pointsAwarded, bool gameCompleted, int drawerScore, int guesserScore) =>
        _roundEndedTcs.TrySetResult((wasCorrect, word, pointsAwarded, gameCompleted, drawerScore, guesserScore));
}

static class DevChannel
{
    // The realtime server uses the same locally-issued dev cert trusted on the test device, which
    // this Mac-native console app doesn't have in its own trust store (chain validation fails with
    // "PartialChain"). Test-only bypass, same pattern as Picator.GameV2's BaseHttpClient DEBUG bypass.
    public static GrpcChannel ForAddress(string address) => GrpcChannel.ForAddress(address, new GrpcChannelOptions
    {
        HttpHandler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (_, _, _, _) => true
        }
    });
}

static class HubConnection
{
    public static async Task<(IMatchmakingHub Hub, TestReceiver Receiver, GrpcChannel Channel)> ConnectAsync(string realtimeUrl, string? jwt)
    {
        var channel = DevChannel.ForAddress(realtimeUrl);
        var receiver = new TestReceiver();
        var callOptions = jwt == null
            ? default
            : new CallOptions(headers: new Metadata { { "Authorization", $"Bearer {jwt}" } });

        var hub = await StreamingHubClient.ConnectAsync<IMatchmakingHub, IMatchFoundReceiver>(channel, receiver, option: callOptions);
        return (hub, receiver, channel);
    }
}

static class GameHubConnection
{
    public static async Task<(IGameHub Hub, BotGameReceiver Receiver, GrpcChannel Channel)> ConnectAsync(string realtimeUrl, string jwt)
    {
        var channel = DevChannel.ForAddress(realtimeUrl);
        var receiver = new BotGameReceiver();
        var callOptions = new CallOptions(headers: new Metadata { { "Authorization", $"Bearer {jwt}" } });

        var hub = await StreamingHubClient.ConnectAsync<IGameHub, IGameDrawingReceiver>(channel, receiver, option: callOptions);
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
            var matchedA = await Task.WhenAny(receiverA.MatchFound.Task, Task.Delay(MatchTimeout)) == receiverA.MatchFound.Task;
            var matchedB = await Task.WhenAny(receiverB.MatchFound.Task, Task.Delay(MatchTimeout)) == receiverB.MatchFound.Task;

            if (resolvedA != userIdA || resolvedB != userIdB)
                return ("pair", false, $"EnterQueueAsync returned a userId that didn't match the JWT claim (spoofing guard broken): A={resolvedA} expected {userIdA}, B={resolvedB} expected {userIdB}");

            if (!matchedA || !matchedB)
                return ("pair", false, $"Timed out waiting for OnMatchFound (A matched={matchedA}, B matched={matchedB})");

            var (gameCodeA, isDrawerA) = await receiverA.MatchFound.Task;
            var (gameCodeB, isDrawerB) = await receiverB.MatchFound.Task;

            if (gameCodeA != gameCodeB)
                return ("pair", false, $"Players received different game codes: A={gameCodeA}, B={gameCodeB}");

            if (isDrawerA == isDrawerB)
                return ("pair", false, $"Both players got the same role (isDrawer={isDrawerA}) - role assignment broken");

            return ("pair", true, $"Both players matched into game {gameCodeA} (A isDrawer={isDrawerA}, B isDrawer={isDrawerB})");
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
        var channel = DevChannel.ForAddress(options.RealtimeUrl);
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

    public static async Task<(string, bool, string)> RunLiveBotScenario(ApplicationDbContext db, ITokenService tokenService, CliOptions options)
    {
        var (_, jwt) = await TestIdentity.CreateUserAndTokenAsync(db, tokenService, "Bot");

        var (matchHub, matchReceiver, matchChannel) = await HubConnection.ConnectAsync(options.RealtimeUrl, jwt);
        IGameHub? gameHub = null;
        GrpcChannel? gameChannel = null;
        try
        {
            // The OLDER queued ticket becomes the drawer, so the bot must not enter the queue until
            // the human already has - queueing first here would make the bot the drawer instead.
            Console.WriteLine("Tap Start Quick Match on the real device now. Once you see \"Searching...\", press Enter here to queue the bot.");
            Console.ReadLine();

            await matchHub.EnterQueueAsync(GameFormat.Solo);
            Console.WriteLine("Bot queued as guesser candidate, waiting for the match...");

            var waitTimeout = TimeSpan.FromMinutes(10);
            var matched = await Task.WhenAny(matchReceiver.MatchFound.Task, Task.Delay(waitTimeout)) == matchReceiver.MatchFound.Task;
            if (!matched)
                return ("live-bot", false, $"Timed out after {waitTimeout.TotalMinutes} minutes waiting for a human to start Quick Match.");

            var (gameCode, isDrawer) = await matchReceiver.MatchFound.Task;
            if (isDrawer)
                return ("live-bot", false, "Bot was matched as the drawer - it must queue AFTER the human. Restart the human's Quick Match first, then re-run the bot.");

            Console.WriteLine($"Matched into game {gameCode}. Joining the game hub...");

            BotGameReceiver gameReceiver;
            (gameHub, gameReceiver, gameChannel) = await GameHubConnection.ConnectAsync(options.RealtimeUrl, jwt);

            // Loop rounds until the game completes - roles swap each round (Solo: RoundsPerPlayer * 2
            // total rounds), so the bot needs to handle being either drawer or guesser depending on the round.
            var roundNumber = 1;
            while (true)
            {
                gameReceiver.ResetRoundEnded();
                var (joinIsDrawer, _, _, roundDurationSeconds) = await gameHub.JoinGameAsync(gameCode);
                Console.WriteLine($"--- Round {roundNumber}: bot is {(joinIsDrawer ? "drawer" : "guesser")} ---");

                if (joinIsDrawer)
                {
                    await Task.Delay(TimeSpan.FromSeconds(3)); // rough alignment with the human's 10s intro screen
                    Console.WriteLine("Bot is drawing a few scripted lines (not expecting a masterpiece)...");
                    await DrawScriptedLinesAsync(gameHub, gameCode);
                    Console.WriteLine("Bot finished drawing. Guess it on the device (or let the round time out) to continue...");
                }
                else
                {
                    var round = await db.Round.AsNoTracking()
                        .Where(r => r.Game!.GameCode == gameCode && r.Status == RoundStatus.Active)
                        .OrderByDescending(r => r.RoundNumber)
                        .FirstOrDefaultAsync();
                    if (round == null)
                        return ("live-bot", false, $"No active round found in the database for game {gameCode} (round {roundNumber}).");

                    Console.WriteLine("Joined as guesser. Draw a stroke or two on the device to confirm drawing sync, then press Enter here to submit the bot's (correct) guess...");
                    Console.ReadLine();

                    var wasCorrect = await gameHub.SubmitGuessAsync(round.Word);
                    if (!wasCorrect)
                        return ("live-bot", false, $"Bot's guess of \"{round.Word}\" was rejected as incorrect by the server (round {roundNumber}).");
                }

                var roundTimeout = TimeSpan.FromSeconds(roundDurationSeconds + 60);
                var roundEndedReceived = await Task.WhenAny(gameReceiver.WaitForRoundEndedAsync(), Task.Delay(roundTimeout)) == gameReceiver.WaitForRoundEndedAsync();
                if (!roundEndedReceived)
                    return ("live-bot", false, $"Round {roundNumber} never resolved - no OnRoundEnded within {roundTimeout.TotalSeconds}s.");

                var result = await gameReceiver.WaitForRoundEndedAsync();
                Console.WriteLine($"Round {roundNumber} ended: correct={result.WasCorrect}, word=\"{result.Word}\", gameCompleted={result.GameCompleted}");

                if (result.GameCompleted)
                {
                    return ("live-bot", true, $"Game completed after {roundNumber} round(s) in {gameCode}. " +
                        $"Final scores - drawer role: {result.DrawerScore}, guesser role: {result.GuesserScore} " +
                        $"(received {gameReceiver.PointsReceived} drawing points across {gameReceiver.LinesCompleted} strokes total as guesser).");
                }

                roundNumber++;
            }
        }
        finally
        {
            await matchHub.DisposeAsync();
            await matchChannel.ShutdownAsync();

            if (gameHub != null)
                await gameHub.DisposeAsync();
            if (gameChannel != null)
                await gameChannel.ShutdownAsync();
        }
    }

    // Scripted, deliberately unimpressive doodle - just enough to exercise the drawing-sync path
    // (point/color/line-completed broadcasts) when the bot ends up as drawer for a later round.
    private static async Task DrawScriptedLinesAsync(IGameHub gameHub, string gameCode)
    {
        var random = new Random();
        await gameHub.SendDrawingColor(gameCode, 0xFF1A1A1AU);

        for (var line = 0; line < 3; line++)
        {
            var startX = random.Next(40, 220);
            var startY = random.Next(40, 220);
            for (var p = 0; p < 10; p++)
            {
                var x = startX + p * 6;
                var y = startY + (float)(Math.Sin(p * 0.6) * 30);
                await gameHub.SendDrawingPoint(gameCode, x, y);
                await Task.Delay(30);
            }
            await gameHub.SendDrawingCompleted(gameCode);
            await Task.Delay(300);
        }
    }
}
