using FastEndpoints;
using Picator.Api.Constants;
using Picator.Common.Data.Dtos.Api;
using Picator.Common.Data.Dtos.Users;
using Picator.Service.Contracts.Users;
using System.Security.Claims;

namespace Picator.Api.Endpoints.Users;

public class UpdateDisplayName : Endpoint<UpdateDisplayNameRequest, ApiResult>
{
    private readonly IUserUpdateDisplayNameService _userUpdateDisplayNameService;

    public UpdateDisplayName(IUserUpdateDisplayNameService userUpdateDisplayNameService)
    {
        _userUpdateDisplayNameService = userUpdateDisplayNameService;
    }

    public override void Configure()
    {
        Post("api/v1/users/display-name");
        Summary(s =>
        {
            s.Summary = "Update display name";
            s.Description = "Updates the current user's display name.";
        });
        Description(d => d.Produces(200).WithTags(EndpointsTags.Users));
    }

    public override async Task HandleAsync(UpdateDisplayNameRequest request, CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.Name);
        await Send.OkAsync(await _userUpdateDisplayNameService.UpdateDisplayName(userId, request), cancellation: ct);
    }
}
