using Picator.Common.Data.Enums;

namespace Picator.Repository.Repositories;

/// <inheritdoc/>
public sealed class MatchTicketRepository : BaseRepository<MatchTicket>, IMatchTicketRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MatchTicketRepository"/> class.
    /// </summary>
    public MatchTicketRepository(ApplicationDbContext context, IDbConnection connection) : base(context, connection)
    {
    }

    /// <inheritdoc/>
    public Task<List<MatchTicket>> GetOldestQueued(GameFormat format, int take)
    {
        return Context.MatchTicket
            .Where(t => t.Format == format && t.Status == MatchTicketStatus.Queued)
            .OrderBy(t => t.CreatedDate)
            .Take(take)
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<bool> TryClaimPair(Guid ticketId1, Guid ticketId2, string gameCode)
    {
        var affected = await Context.MatchTicket
            .Where(t => (t.Id == ticketId1 || t.Id == ticketId2) && t.Status == MatchTicketStatus.Queued)
            .ExecuteUpdateAsync(s => s
                .SetProperty(t => t.Status, MatchTicketStatus.Matched)
                .SetProperty(t => t.GameCode, gameCode));
        return affected == 2;
    }

    /// <inheritdoc/>
    public Task<int> ExpireStale(TimeSpan ttl)
    {
        var cutoff = DateTime.UtcNow - ttl;
        return Context.MatchTicket
            .Where(t => t.Status == MatchTicketStatus.Queued && t.CreatedDate < cutoff)
            .ExecuteUpdateAsync(s => s.SetProperty(t => t.Status, MatchTicketStatus.Expired));
    }
}
