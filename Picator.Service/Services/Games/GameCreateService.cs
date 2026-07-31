using FluentValidation;
using Picator.Common.Data.Dtos.Api;
using Picator.Common.Data.Dtos.Data.Dtos.Api;
using Picator.Common.Data.Dtos.Games;
using Picator.Common.Data.Enums;
using Picator.Entities.Models;
using Picator.Repository;
using Picator.Service.Contracts.Games;

namespace Picator.Service.Services.Games;

public class GameCreateService : IGameCreateService
{
    private readonly IValidator<GameCreateRequest> _validator;
    private readonly IUnitOfWork _unitOfWork;

    public GameCreateService(IValidator<GameCreateRequest> validator, IUnitOfWork unitOfWork)
    {
        _validator = validator;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResult<GameCreateResult>> Create(GameCreateRequest request)
    {
        var validationResult = await _validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return new ApiResult<GameCreateResult>
            {
                StatusCode = ApiResultStatusCode.BadRequest,
                Errors = validationResult.Errors.Select(e => e.ErrorMessage)
            };
        var already = await _unitOfWork.Game.IsAlreadyPlaying(request.RoomId.ToString());
        if (!string.IsNullOrEmpty(already))
            return new ApiResult<GameCreateResult>()
            {
                IsSuccess = false,
                StatusCode = ApiResultStatusCode.Conflict,
                Errors = new[] { "Another game is already playing" }
            };

        //var game = _mapper.Map<GameCreateRequest, Game>(request);
        //await _unitOfWork.Game.AddFast(game);
        //var members = new List<GameMember>();
        //await _unitOfWork.GameMember.AddRangeFast(members);
        return new ApiResult<GameCreateResult>
        {
           // Data = new GameCreateResult() { Id = game.Id },
            IsSuccess = true
        };
    }

    public async Task<string?> CreateTimeGame(string gameCode)
    {
        var randomIndex = new Random().Next(0, 150);
        var word = await _unitOfWork.GameWord.GetRandomWord(randomIndex);
        var game = new Game
        {
            GameCode = gameCode,
            Status = GameStatus.InProgress,
            RoundsPerPlayer = 1,
            Round = new List<Round>
            {
                new()
                {
                    RoundNumber = 1,
                    Word = word!,
                    Status = RoundStatus.Active,
                    StartedAt = DateTime.UtcNow,
                }
            }
        };

        await _unitOfWork.Game.Add(game);
        await _unitOfWork.Commit();
        return word;
    }
}