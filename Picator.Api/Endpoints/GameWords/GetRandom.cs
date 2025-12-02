using FastEndpoints;
using Picator.Api.Constants;
using Picator.Common.Data.Dtos.Api;
using Picator.Entities.Models;
using Picator.Service.Contracts.GameWords;

namespace Picator.Api.Endpoints.GameWords;

public class GetRandom : EndpointWithoutRequest<ApiResult<string>>
{
    private readonly IGameWordsService _gameWordsService;

    public GetRandom(IGameWordsService gameWordsService)
    {
        _gameWordsService = gameWordsService;
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
        var words = await _gameWordsService.GetRandomWord();
        await Send.OkAsync(words, cancellation: ct);
    }
}