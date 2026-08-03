using Picator.Common.Data.Enums;
using Picator.Realtime.Services;
using Picator.Service.Contracts.Games;
using Picator.Service.Contracts.Matchmaking;
using TickerQ.Utilities.Base;

namespace Picator.Realtime.Jobs;

/// <summary>
/// Recurring sweep: pairs any queued tickets that didn't get matched by the Hub's enqueue-time fast path
/// (e.g. both players were waiting before either could trigger the other's pairing attempt), and expires
/// tickets that have been queued past the TTL. Runs every 2 seconds.
/// </summary>
public class MatchmakingSweepJob
{
    private static readonly TimeSpan TicketTtl = TimeSpan.FromSeconds(60);

    private readonly IMatchmakingService _matchmakingService;
    private readonly IGameCreateService _gameCreateService;
    private readonly MatchmakingGroupService _groupService;
    private readonly ILogger<MatchmakingSweepJob> _logger;

    public MatchmakingSweepJob(IMatchmakingService matchmakingService, IGameCreateService gameCreateService, MatchmakingGroupService groupService, ILogger<MatchmakingSweepJob> logger)
    {
        _matchmakingService = matchmakingService;
        _gameCreateService = gameCreateService;
        _groupService = groupService;
        _logger = logger;
    }

    [TickerFunction("MatchmakingSweep", cronExpression: "*/2 * * * * *")]
    public async Task SweepAsync(TickerFunctionContext context, CancellationToken cancellationToken)
    {
        foreach (var format in Enum.GetValues<GameFormat>())
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var pair = await _matchmakingService.TryPairAsync(format);
                if (pair is not { } p)
                    break;

                // UserIdA is the older ticket (queued first) - it becomes the drawer, same convention as the Hub's fast path.
                await _gameCreateService.CreateMatchedGame(p.GameCode, drawerUserId: p.UserIdA, guesserUserId: p.UserIdB);

                _groupService.GetGroup(format).Only([p.UserIdA]).OnMatchFound(p.GameCode, isDrawer: true);
                _groupService.GetGroup(format).Only([p.UserIdB]).OnMatchFound(p.GameCode, isDrawer: false);
                _logger.LogInformation("***Sweep matched {UserIdA} (drawer) vs {UserIdB} (guesser) -> {GameCode}***", p.UserIdA, p.UserIdB, p.GameCode);
            }
        }

        var expired = await _matchmakingService.ExpireStaleAsync(TicketTtl);
        if (expired > 0)
            _logger.LogInformation("***Sweep expired {Count} stale matchmaking tickets***", expired);
    }
}
