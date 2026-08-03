using Picator.Repository.Contracts;

namespace Picator.Repository;

/// <summary>
/// Unit of work.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Repository provides methods to retrieve/handle avatar data.
    /// </summary>
    IAvatarRepository Avatar { get; }

    /// <summary>
    /// Repository provides methods to retrieve/handle game member data.
    /// </summary>
    IGameMemberRepository GameMember { get; }

    /// <summary>
    /// Repository provides methods to retrieve/handle game message data.
    /// </summary>
    IGameMessageRepository GameMessage { get; }

    /// <summary>
    /// Repository provides methods to retrieve/handle game data.
    /// </summary>
    IGameRepository Game { get; }

    /// <summary>
    /// Repository provides methods to retrieve/handle game word data.
    /// </summary>
    IGameWordRepository GameWord { get; }

    /// <summary>
    /// Repository provides methods to retrieve/handle matchmaking ticket data.
    /// </summary>
    IMatchTicketRepository MatchTicket { get; }

    /// <summary>
    /// Repository provides methods to retrieve/handle refresh token data.
    /// </summary>
    IRefreshTokenRepository RefreshToken { get; }

    /// <summary>
    /// Repository provides methods to retrieve/handle room member data.
    /// </summary>
    IRoomMemberRepository RoomMember { get; }

    /// <summary>
    /// Repository provides methods to retrieve/handle room data.
    /// </summary>
    IRoomRepository Room { get; }

    /// <summary>
    /// Commit changes to database.
    /// </summary>
    Task<int> Commit();
}