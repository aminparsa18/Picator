using Picator.Common.Data.Dtos.Avatars;
using Picator.Data;
using Picator.Entities.Models;
using Picator.Repository.Contracts;
using System.Data;

namespace Picator.Repository.Repositories;

/// <inheritdoc/>
public class AvatarRepository : BaseRepository<Avatar>, IAvatarRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AvatarRepository"/> class.
    /// </summary>
    public AvatarRepository(ApplicationDbContext context, IDbConnection connection) : base(context, connection)
    {
    }

    /// <inheritdoc/>
    public Task<List<AvatarResult>> GetAllDtos()
    {
        return Context.Avatar.AsNoTracking().Select(s => new AvatarResult
        {
            Name = s.Name
        }).ToListAsync();
    }
}