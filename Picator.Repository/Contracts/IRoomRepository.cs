using Picator.Common.Data.Dtos.Rooms;
using Picator.Entities.Models;

namespace Picator.Repository.Contracts;

/// <summary>
/// Repository provides methods to retrieve/handle room data.
/// </summary>
public interface IRoomRepository : IBaseRepository<Room>
{
    /// <summary>
    /// Retrieves room details.
    /// </summary>
    /// <param name="roomId">Room key identifier.</param>
    /// <returns>Room details.</returns>
    Task<List<RoomDetailsResult>> GetRoomFast(Guid roomId);

    /// <summary>
    /// Retrieves room details.
    /// </summary>
    /// <param name="roomId">Room key identifier.</param>
    /// <returns>Room details.</returns>
    Task<RoomDetailsResult?> GetRoom(string roomId);

    /// <summary>
    /// Retrieves all rooms by user.
    /// </summary>
    /// <param name="userId">User key identifier.</param>
    /// <returns>List of user rooms.</returns>
    Task<List<RoomDetailsResult>> GetMyRooms(Guid userId);

    /// <summary>
    /// Retrieves all rooms by user.
    /// </summary>
    /// <param name="userId">User key identifier.</param>
    /// <returns>List of user rooms.</returns>
    Task<List<RoomDetailsResult>> GetMyRoomsFast(Guid userId);

    /// <summary>
    /// Retrievs room member if is joined in room.
    /// </summary>
    /// <param name="userId">User key identifier.</param>
    /// <param name="roomId">Room key identifier.</param>
    /// <returns>Room member.</returns>
    Task<RoomMember?> IsJoined(Guid userId, Guid roomId);

    /// <summary>
    /// Retrievs room member if is joined in room.
    /// </summary>
    /// <param name="userId">User key identifier.</param>
    /// <param name="roomId">Room key identifier.</param>
    /// <returns>Room member.</returns>
    Task<Guid?> IsJoinedFast(Guid userId, Guid roomId);

    /// <summary>
    /// Retrievs room identifier by code.
    /// </summary>
    /// <param name="code">Room code.</param>
    /// <returns>Room key identifier.</returns>
    Task<Guid?> GetByCode(string code);
}