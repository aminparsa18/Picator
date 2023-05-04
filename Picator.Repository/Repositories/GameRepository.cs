using Picator.Common.Data.Dtos.Games;
using Picator.Data;
using Picator.Entities.Models;
using Picator.Repository.Contracts;
using System.Data;

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
        //            return connection.ExecuteQueryAsync<GameDto>(@"SELECT CAST((
        //    SELECT COUNT(*)
        //    FROM [dbo].[GameMember] AS [g]
        //    WHERE ([g0].[Id] = [g].[GameId]) AND [g].[UserId] IS NULL) AS smallint) AS [Capacity], [g0].[Id], [g0].[RoomId], [r].[Image] AS [RoomImage]
        //FROM [dbo].[Game] AS [g0]
        //INNER JOIN [dbo].[Room] AS [r] ON [g0].[RoomId] = [r].[Id]
        //WHERE ([g0].[Status] = CAST(0 AS smallint)) AND ((
        //    SELECT COUNT(*)
        //    FROM [dbo].[GameMember] AS [g1]
        //    WHERE ([g0].[Id] = [g1].[GameId]) AND [g1].[UserId] IS NOT NULL) < (
        //    SELECT COUNT(*)
        //    FROM [dbo].[GameMember] AS [g2]
        //    WHERE [g0].[Id] = [g2].[GameId])) ");
        return Context.Game.Where(g => g.GameMember.Count(m => m.UserId != null) < g.GameMember.Count)
            .Select(s => new AvailableGameResult()
            {
                MemberCount = (short)s.GameMember.Count,
                Id = s.Id,
                RoomId = s.RoomId.Value
            }).ToListAsync();
    }

    /// <inheritdoc/>
    public Task<string> IsJoinedFast(string userId, string gameId)
    {
        return Connection.ExecuteScalarAsync<string>(
            @"SELECT TOP(1) [g].[Id]
                  FROM [dbo].[GameMember] AS [g]
                  WHERE ([g].[UserId] = @userId) AND ([g].[GameId] = @gameId)", new { userId, gameId });
    }

    /// <inheritdoc/>
    public Task<string> IsAlreadyPlaying(string roomId)
    {
        return Connection.ExecuteScalarAsync<string>(
            @"SELECT TOP(1) [g].[Id]
                  FROM [dbo].[Game] AS [g]
                  WHERE ([g].[RoomId] = @roomId) AND ([g].[Status] = 0 OR [g].[Status] = 1)", new { roomId });
    }

    /// <inheritdoc/>
    public Task<int> StartGame(string gameId)
    {
        return Connection.ExecuteNonQueryAsync("UPDATE [Game] SET [Status] = 1 WHERE [Id] = @gameId",
            new { gameId });
    }
}