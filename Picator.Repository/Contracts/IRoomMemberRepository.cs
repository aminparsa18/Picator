using Picator.Common.Data.Dtos.RoomMembers;
using Picator.Entities.Models;

namespace Picator.Repository.Contracts;

/// <summary>
/// Repository provides methods to retrieve/handle room member data.
/// </summary>
public interface IRoomMemberRepository : IBaseRepository<RoomMember>
{
    /// <summary>
    /// Retrieves room member identifier by room.
    /// </summary>
    /// <param name="roomId">Room key identifier.</param>
    /// <param name="userId">User key identifier.</param>
    /// <returns>Room member key identifier.</returns>
    Task<IEnumerable<Guid>> FindInRoom(Guid roomId, Guid userId);

    /// <summary>
    /// Retrieves all member of room.
    /// </summary>
    /// <param name="roomId">Room key identifier.</param>
    /// <returns>List of room members.</returns>
    Task<List<RoomMemberResult>> GetByRoom(Guid roomId);

    /// <summary>
    /// Retrieves all member of room.
    /// </summary>
    /// <param name="roomId">Room key identifier.</param>
    /// <returns>List of room members.</returns>
    Task<IEnumerable<RoomMemberResult>> GetByRoomFast(Guid roomId);
}