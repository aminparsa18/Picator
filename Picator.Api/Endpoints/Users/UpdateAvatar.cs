using FastEndpoints;
using Picator.Api.Constants;
using Picator.Common.Data.Dtos.Api;
using Picator.Common.Data.Dtos.Users;
using Picator.Service.Contracts.Users;
using System.Security.Claims;

namespace Picator.Api.Endpoints.Users;

public class UpdateAvatar : Endpoint<UpdateAvatarRequest, ApiResult>
{
    private readonly IUserUpdateAvatarService _userUpdateAvatarService;

    public UpdateAvatar(IUserUpdateAvatarService userUpdateAvatarService)
    {
        _userUpdateAvatarService = userUpdateAvatarService;
    }

    public override void Configure()
    {
        Post("api/v1/users/avatar");
        Summary(s =>
        {
            s.Summary = "Update avatar";
            s.Description = "Updates the current user's avatar.";
        });
        Description(d => d.Produces(200).WithTags(EndpointsTags.Users));
    }

    public override async Task HandleAsync(UpdateAvatarRequest request, CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.Name);
        await Send.OkAsync(await _userUpdateAvatarService.UpdateAvatar(userId, request), cancellation: ct);
    }
}
