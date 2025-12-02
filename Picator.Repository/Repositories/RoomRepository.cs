using Picator.Common.Data.Dtos.Rooms;

namespace Picator.Repository.Repositories;

/// <inheritdoc/>
public sealed class RoomRepository : BaseRepository<Room>, IRoomRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RoomRepository"/> class.
    /// </summary>
    public RoomRepository(ApplicationDbContext context, IDbConnection connection) : base(context, connection)
    {
    }

    /// <inheritdoc/>
    public Task<RoomDetailsResult?> GetRoom(string roomId)
    {
        var parsed = Guid.Parse(roomId);
        return Context.Room.AsNoTracking().Where(r => r.Id == parsed)
            .Select(s => new RoomDetailsResult()
            {
                Id = s.Id,
                MemberCount = (short)s.RoomMember.Count,
                Name = s.Name,
                Code = s.Code
            }).FirstOrDefaultAsync();
    }

    /// <inheritdoc/>
    public Task<List<RoomDetailsResult>> GetRoomFast(Guid roomId)
    {
        return Context.Database.SqlQuery<RoomDetailsResult>($"SELECT TOP(1) CAST((SELECT COUNT(*) FROM[dbo].[RoomMember] AS[r] WHERE[r0].[Id] = [r].[RoomId]) AS smallint) AS[MemberCount], (SELECT COUNT(*) FROM[dbo].[Game] AS[g] WHERE([r0].[Id] = [g].[RoomId]) AND([g].[Status] <> CAST(0 AS smallint))) AS[GamePlayedCount], [r0].[Name],[r0].[Code] FROM[dbo].[Room] AS[r0] WHERE[r0].[Id] = {roomId}").ToListAsync();
    }

    /// <inheritdoc/>
    public Task<List<RoomDetailsResult>> GetMyRooms(Guid userId)
    {
        return Context.RoomMember.AsNoTracking().Where(r => r.UserId == userId)
            .Select(s => new RoomDetailsResult()
            {
                Id = s.RoomId,
                MemberCount = (short)s.Room.RoomMember.Count,
                Name = s.Room.Name,
                IsAdmin = s.Room.UserId == userId
            }).ToListAsync();
    }

    /// <inheritdoc/>
    public Task<List<RoomDetailsResult>> GetMyRoomsFast(Guid userId)
    {
        return Context.Database.SqlQuery<RoomDetailsResult>($"SELECT [r0].[RoomId] AS [Id], CAST((SELECT COUNT(*) FROM [dbo].[RoomMember] AS [r] WHERE [r1].[Id] = [r].[RoomId]) AS smallint) AS [MemberCount], (SELECT COUNT(*) FROM [dbo].[Game] AS [g] WHERE ([r1].[Id] = [g].[RoomId]) AND ([g].[Status] <> CAST(0 AS smallint))) AS [GamePlayedCount],[r1].[Code],[r1].[Name], CASE WHEN [r1].[UserId] = @userId THEN CAST(1 AS bit) ELSE CAST(0 AS bit) END AS [IsAdmin] FROM [dbo].[RoomMember] AS [r0] INNER JOIN [dbo].[Room] AS [r1] ON [r0].[RoomId] = [r1].[Id] WHERE [r0].[UserId] = {userId}").ToListAsync();
    }

    /// <inheritdoc/>
    public Task<RoomMember?> IsJoined(Guid userId, Guid roomId) =>
        Context.RoomMember.FirstOrDefaultAsync(m => m.UserId == userId && m.RoomId == roomId);

    /// <inheritdoc/>
    public Task<Guid?> IsJoinedFast(Guid userId, Guid roomId)
    {
        return Context.Database.SqlQuery<Guid?>($"SELECT TOP(1) [r].[Id] FROM [dbo].[RoomMember] AS [r] WHERE ([r].[UserId] = {userId}) AND ([r].[RoomId] = {roomId})").FirstOrDefaultAsync();
    }

    /// <inheritdoc/>
    public Task<Guid?> GetByCode(string code)
    {
        return Context.Database.SqlQuery<Guid?>($"SELECT TOP(1) [r].[Id] FROM [dbo].[Room] AS [r] WHERE ([r].[Code] = {code})").FirstOrDefaultAsync();
    }
}