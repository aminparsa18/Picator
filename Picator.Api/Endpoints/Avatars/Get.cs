using FastEndpoints;
using Picator.Api.Constants;
using Picator.Common.Data.Dtos.Api;
using Picator.Common.Data.Dtos.Avatars;
using Picator.Service.Contracts.Avatars;

namespace Picator.Api.Endpoints.Avatars;

public class Get : EndpointWithoutRequest<ApiResult<IEnumerable<AvatarResult>>>
{
    private readonly IAvatarService _avatarService;

    public Get(IAvatarService avatarService)
    {
        _avatarService = avatarService;
    }

    public override void Configure()
    {
        Get(ApiUrls.Avatars);
        Summary(s =>
        {
            s.Summary = "Get Avatars";
            s.Description = "Retrieves all avatars data.";
        });
        Description(d => d.Produces(200).WithTags(EndpointsTags.Avatars));
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var avatars = await _avatarService.GetAll();
        await Send.OkAsync(new ApiResult<IEnumerable<AvatarResult>>
        {
            Data = avatars,
            IsSuccess = true
        }, cancellation: ct);
    }
}