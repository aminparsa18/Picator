using Picator.Entities.Models;

namespace Picator.Repository.Contracts;

public interface IGameWordRepository : IBaseRepository<GameWord>
{
    Task<string?> GetRandomWord(int randomIndex);
}