using Picator.Data;
using Picator.Entities.Models;
using Picator.Repository.Contracts;
using System.Data;

namespace Picator.Repository.Repositories;
public class GameWordRepository : BaseRepository<GameWord>, IGameWordRepository
{
    public GameWordRepository(ApplicationDbContext context, IDbConnection connection) : base(context, connection)
    {
    }

    public Task<string?> GetRandomWord(int randomIndex)
    {
        var random = new Random();
        return Context.GameWord.Skip(randomIndex).Take(1).Select(g => g.Word).FirstOrDefaultAsync();
    }
}