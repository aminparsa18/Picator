using FastEndpoints;
using Picator.Api.Constants;
using Picator.Common.Data.Dtos.Api;
using Picator.Entities.Models;
using Picator.Service.Contracts.GameWords;
using Picator.Repository;

namespace Picator.Api.Endpoints.GameWords;

public class GetRandom : EndpointWithoutRequest<ApiResult<List<string>>>
{
    private readonly IGameWordsService _gameWordsService;
    private readonly IUnitOfWork _unitOfWork;
    public GetRandom(IGameWordsService gameWordsService, IUnitOfWork unitOfWork)
    {
        _gameWordsService = gameWordsService;
        _unitOfWork = unitOfWork;
    }

    public override void Configure()
    {
        Get(ApiUrls.GameWords);
        Summary(s =>
        {
            s.Summary = "Get Avatars";
            s.Description = "Retrieves all avatars data.";
        });
        Description(d => d.Produces(200).WithTags(nameof(GameWord)));
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var words = await _gameWordsService.GetRandomWords();
        await SendMemoryPackAsync(words, cancellation: ct);
    }
}