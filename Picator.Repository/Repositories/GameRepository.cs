using Picator.Common.Data.Dtos.Games;

namespace Picator.Repository.Repositories;

/// <inheritdoc/>
public sealed class GameRepository : BaseRepository<Game>, IGameRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GameRepository"/> class.
    /// </summary>
    public GameRepository(ApplicationDbContext context, IDbConnection connection) : base(context, connection)
    {
    }

    /// <inheritdoc/>
    public Task<List<AvailableGameResult>> GetAvailables()
    {
        return Context.Game.Where(g => g.GameMember.Count(m => m.UserId != null) < g.GameMember.Count)
            .Select(s => new AvailableGameResult()
            {
                MemberCount = (short)s.GameMember.Count,
                Id = s.Id,
                RoomId = s.RoomId.Value
            }).ToListAsync();
    }

    /// <inheritdoc/>
    public Task<string?> IsJoinedFast(string userId, string gameId)
    {
        return Context.Database.SqlQuery<string>($"SELECT TOP(1) [g].[Id] FROM [dbo].[GameMember] AS [g] WHERE ([g].[UserId] = {userId}) AND ([g].[GameId] = {gameId})").FirstOrDefaultAsync();
    }

    /// <inheritdoc/>
    public Task<string?> IsAlreadyPlaying(string roomId)
    {
        return Context.Database.SqlQuery<string>($"SELECT TOP(1) [g].[Id] FROM [dbo].[Game] AS [g] WHERE ([g].[RoomId] = {roomId}) AND ([g].[Status] = 0 OR [g].[Status] = 1)").FirstOrDefaultAsync();
    }

    /// <inheritdoc/>
    public Task<int> StartGame(string gameId)
    {
        return Context.Database.ExecuteSqlAsync($"UPDATE [Game] SET [Status] = 1 WHERE [Id] = {gameId}");
    }
}