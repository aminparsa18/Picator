namespace Picator.Service.Contracts.Games;

public interface IGameLogicService
{
    Task SetTurn(Guid gameId, int index);
}