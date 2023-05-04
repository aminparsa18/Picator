using Picator.Common.Data.Dtos.GameMembers;
using Picator.Data;
using Picator.Entities.Models;
using Picator.Repository.Contracts;
using System.Data;

namespace Picator.Repository.Repositories;

/// <inheritdoc/>
public sealed class GameMemberRepository : BaseRepository<GameMember>, IGameMemberRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GameMemberRepository"/> class.
    /// </summary>
    public GameMemberRepository(ApplicationDbContext context, IDbConnection connection) : base(context, connection)
    {
    }

    /// <inheritdoc/>
    public Task<List<GameMemberResult>> GetByGame(Guid gameId)
    {
        return Context.GameMember.AsNoTracking().Where(g => g.UserId.HasValue && g.GameId == gameId).Select(s =>
            new GameMemberResult()
            {
                DisplayName = s.User.DisplayName,
                Avatar = s.User.Avatar,
                Score = s.User.Score,
                Id = s.Id,
                Status = s.Status
            }).ToListAsync();
    }

    /// <inheritdoc/>
    public Task<IEnumerable<GameMemberResult>> GetByGameFast(Guid gameId)
    {
        return Connection.ExecuteQueryAsync<GameMemberResult>(
            @"SELECT [u].[DisplayName], [u].[Image], [u].[Score], [g].[Id],[g].[Status]
               FROM [dbo].[GameMember] AS [g]
               LEFT JOIN [dbo].[Users] AS [u] ON [g].[UserId] = [u].[Id]
               WHERE [g].[UserId] IS NOT NULL AND ([g].[GameId] = @gameId)", new { gameId });
    }

    /// <inheritdoc/>
    public Task<int> Join(Guid userId, Guid memberId)
    {
        return Connection.ExecuteNonQueryAsync("UPDATE [GameMember] SET [UserId] = @userId WHERE [Id] = @memberId",
            new { userId, memberId });
    }

    /// <inheritdoc/>
    public Task<int> Leave(Guid userId)
    {
        return Connection.ExecuteNonQueryAsync("UPDATE [GameMember] SET [UserId] = NULL WHERE [UserId] = @userId",
            new { userId });
    }
}