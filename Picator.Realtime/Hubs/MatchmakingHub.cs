using Grpc.Core;
using MagicOnion.Server.Hubs;
using Microsoft.AspNetCore.Authorization;
using Picator.Common.Data.Enums;
using Picator.Realtime.Common.Services;
using Picator.Realtime.Services;
using Picator.Service.Contracts.Games;
using Picator.Service.Contracts.Matchmaking;
using System.Security.Claims;

namespace Picator.Realtime.Hubs;

[Authorize]
public class MatchmakingHub : StreamingHubBase<IMatchmakingHub, IMatchFoundReceiver>, IMatchmakingHub
{
    private readonly IMatchmakingService _matchmakingService;
    private readonly IGameCreateService _gameCreateService;
    private readonly MatchmakingGroupService _groupService;
    private readonly ILogger<MatchmakingHub> _logger;

    private Guid _userId;
    private Guid _ticketId;
    private GameFormat _format;
    private bool _inQueue;

    public MatchmakingHub(IMatchmakingService matchmakingService, IGameCreateService gameCreateService, MatchmakingGroupService groupService, ILogger<MatchmakingHub> logger)
    {
        _matchmakingService = matchmakingService;
        _gameCreateService = gameCreateService;
        _groupService = groupService;
        _logger = logger;
    }

    public async ValueTask<Guid> EnterQueueAsync(GameFormat format)
    {
        var user = Context.CallContext.GetHttpContext()!.User;
        _userId = Guid.Parse(user.FindFirstValue(ClaimTypes.Name)!);
        _format = format;

        var result = await _matchmakingService.EnqueueAsync(_userId, format);
        if (result.Data == null)
        {
            _logger.LogError("Matchmaking enqueue failed for user {UserId}: {Errors}", _userId, string.Join(", ", result.Errors ?? []));
            throw new InvalidOperationException("Unable to enter matchmaking queue.");
        }

        _ticketId = result.Data.TicketId;
        _inQueue = true;
        _groupService.GetGroup(format).Add(_userId, Client);
        _logger.LogInformation("***User {UserId} entered {Format} matchmaking queue (ticket {TicketId})***", _userId, format, _ticketId);

        var pair = await _matchmakingService.TryPairAsync(format);
        if (pair is { } p)
        {
            try
            {
                // UserIdA is the older ticket (queued first) - it becomes the drawer.
                await _gameCreateService.CreateMatchedGame(p.GameCode, drawerUserId: p.UserIdA, guesserUserId: p.UserIdB);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "***Failed to create matched game {GameCode} for {UserIdA} vs {UserIdB}***", p.GameCode, p.UserIdA, p.UserIdB);
                throw;
            }

            _groupService.GetGroup(format).Only([p.UserIdA]).OnMatchFound(p.GameCode, isDrawer: true);
            _groupService.GetGroup(format).Only([p.UserIdB]).OnMatchFound(p.GameCode, isDrawer: false);
            _logger.LogInformation("***Matched {UserIdA} (drawer) vs {UserIdB} (guesser) -> {GameCode}***", p.UserIdA, p.UserIdB, p.GameCode);
        }

        return _userId;
    }

    public async ValueTask CancelQueueAsync()
    {
        if (!_inQueue)
            return;

        await _matchmakingService.CancelAsync(_userId, _ticketId);
        _groupService.GetGroup(_format).Remove(_userId);
        _inQueue = false;
        _logger.LogInformation("***User {UserId} cancelled matchmaking (ticket {TicketId})***", _userId, _ticketId);
    }

    protected override ValueTask OnDisconnected()
    {
        if (_inQueue)
            _groupService.GetGroup(_format).Remove(_userId);

        return base.OnDisconnected();
    }
}
