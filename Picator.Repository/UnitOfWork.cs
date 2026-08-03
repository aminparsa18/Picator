using Picator.Data;
using Picator.Repository.Contracts;
using Picator.Repository.Repositories;
using System.Data;

namespace Picator.Repository;

/// <inheritdoc/>
public sealed class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private readonly IDbConnection _connection;
    private IAvatarRepository? _avatar;
    private IGameMemberRepository? _gameMember;
    private IGameMessageRepository? _gameMessage;
    private IGameRepository? _game;
    private IGameWordRepository? _gameWord;
    private IMatchTicketRepository? _matchTicket;
    private IRefreshTokenRepository? _refreshToken;
    private IRoomMemberRepository? _roomMember;
    private IRoomRepository? _room;

    /// <summary>
    /// Initializes a new instance of the <see cref="UnitOfWork"/> class.
    /// </summary>
    public UnitOfWork(ApplicationDbContext context, IDbConnection connection)
    {
        _context = context;
        _connection = connection;
    }

    public IAvatarRepository Avatar => _avatar ??= new AvatarRepository(_context, _connection);

    public IGameMemberRepository GameMember => _gameMember ??= new GameMemberRepository(_context, _connection);

    public IGameMessageRepository GameMessage => _gameMessage ??= new GameMessageRepository(_context, _connection);

    public IGameRepository Game => _game ??= new GameRepository(_context, _connection);

    public IGameWordRepository GameWord => _gameWord ??= new GameWordRepository(_context, _connection);

    public IMatchTicketRepository MatchTicket => _matchTicket ??= new MatchTicketRepository(_context, _connection);

    public IRefreshTokenRepository RefreshToken => _refreshToken ??= new RefreshTokenRepository(_context, _connection);

    public IRoomMemberRepository RoomMember => _roomMember ??= new RoomMemberRepository(_context, _connection);

    public IRoomRepository Room => _room ??= new RoomRepository(_context, _connection);

    public Task<int> Commit()
    {
        return _context.SaveChangesAsync();
    }
}