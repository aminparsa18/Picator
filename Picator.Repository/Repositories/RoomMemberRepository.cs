using Picator.Common.Data.Dtos.RoomMembers;

namespace Picator.Repository.Repositories;

/// <inheritdoc/>
public sealed class RoomMemberRepository : BaseRepository<RoomMember>, IRoomMemberRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RoomMemberRepository"/> class.
    /// </summary>
    public RoomMemberRepository(ApplicationDbContext context, IDbConnection connection) : base(context, connection)
    {
    }

    /// <inheritdoc/>
    public Task<List<Guid>> FindInRoom(Guid roomId, Guid userId)
    {
        return Context.Database.SqlQuery<Guid>($"SELECT TOP 1 [Id] FROM [RoomMember] Where [RoomId]={roomId} AND [UserId]={userId}").ToListAsync();
    }

    /// <inheritdoc/>
    public Task<List<RoomMemberResult>> GetByRoom(Guid roomId)
    {
        return Context.RoomMember.AsNoTracking().Where(r => r.RoomId == roomId)
            .OrderByDescending(o => o.CreatedDate)
            .Select(s => new RoomMemberResult()
            {
                UserId = s.UserId,
                Name = s.User.DisplayName,
                Avatar = s.User.Avatar,
                TotalGame = s.User.GameMember.Count
            }).ToListAsync();
    }

    /// <inheritdoc/>
    public Task<List<RoomMemberResult>> GetByRoomFast(Guid roomId)
    {
        return Context.Database.SqlQuery<RoomMemberResult>($"SELECT [r].[UserId], [u].[DisplayName] AS [Name], [u].[Image], (SELECT COUNT(*) FROM[dbo].[GameMember] AS[g] WHERE [u].[Id] = [g].[UserId]) AS[TotalGame] FROM[dbo].[RoomMember] AS[r] INNER JOIN[dbo].[Users] AS[u] ON[r].[UserId] = [u].[Id] WHERE[r].[RoomId] = {roomId} ORDER BY[r].[CreatedDate] DESC ").ToListAsync();
    }
}