using Picator.Common.Data.Dtos.Api;
using Picator.Common.Data.Dtos.Data.Dtos.Api;
using Picator.Common.Data.Dtos.Matchmaking;
using Picator.Common.Data.Enums;
using Picator.Common.Helpers;
using Picator.Entities.Models;
using Picator.Repository;
using Picator.Service.Contracts.Matchmaking;

namespace Picator.Service.Services.Matchmaking;

public class MatchmakingService : IMatchmakingService
{
    private const int GameCodeLength = 12;

    private readonly IUnitOfWork _unitOfWork;

    public MatchmakingService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResult<MatchTicketResult>> EnqueueAsync(Guid userId, GameFormat format)
    {
        var existing = await _unitOfWork.MatchTicket.Get(t => t.UserId == userId && t.Status == MatchTicketStatus.Queued);
        if (existing != null)
            return new ApiResult<MatchTicketResult>
            {
                IsSuccess = false,
                Errors = ["Already searching for a match"],
                StatusCode = ApiResultStatusCode.Conflict,
                Data = new MatchTicketResult { TicketId = existing.Id }
            };

        var ticket = new MatchTicket
        {
            UserId = userId,
            Format = format,
            Status = MatchTicketStatus.Queued
        };
        await _unitOfWork.MatchTicket.Add(ticket);
        await _unitOfWork.Commit();

        return new ApiResult<MatchTicketResult>
        {
            IsSuccess = true,
            Data = new MatchTicketResult { TicketId = ticket.Id }
        };
    }

    public async Task<ApiResult> CancelAsync(Guid userId, Guid ticketId)
    {
        var ticket = await _unitOfWork.MatchTicket.Get(t => t.Id == ticketId && t.UserId == userId && t.Status == MatchTicketStatus.Queued);
        if (ticket == null)
            return new ApiResult
            {
                IsSuccess = false,
                Errors = ["No active ticket found"],
                StatusCode = ApiResultStatusCode.NotFound
            };

        ticket.Status = MatchTicketStatus.Cancelled;
        _unitOfWork.MatchTicket.Update(ticket);
        await _unitOfWork.Commit();

        return new ApiResult { IsSuccess = true };
    }

    public async Task<(Guid UserIdA, Guid UserIdB, string GameCode)?> TryPairAsync(GameFormat format)
    {
        var candidates = await _unitOfWork.MatchTicket.GetOldestQueued(format, 2);
        if (candidates.Count < 2)
            return null;

        var gameCode = RandomHelper.CreateRandomText(GameCodeLength);
        var claimed = await _unitOfWork.MatchTicket.TryClaimPair(candidates[0].Id, candidates[1].Id, gameCode);
        if (!claimed)
            return null;

        return (candidates[0].UserId, candidates[1].UserId, gameCode);
    }

    public Task<int> ExpireStaleAsync(TimeSpan ttl) => _unitOfWork.MatchTicket.ExpireStale(ttl);
}
