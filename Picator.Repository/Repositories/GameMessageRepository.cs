using Picator.Data;
using Picator.Entities.Models;
using Picator.Repository.Contracts;
using System.Data;

namespace Picator.Repository.Repositories;

/// <inheritdoc/>
public sealed class GameMessageRepository : BaseRepository<GameMessage>, IGameMessageRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GameMessageRepository"/> class.
    /// </summary>
    public GameMessageRepository(ApplicationDbContext context, IDbConnection connection) : base(context, connection)
    {
    }
}