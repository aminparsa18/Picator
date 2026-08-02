using FastEndpoints;
using Picator.Api.Constants;
using Picator.Common.Data.Dtos.Api;
using Picator.Common.Data.Dtos.Users;
using Picator.Service.Contracts.Users;
using System.Security.Claims;

namespace Picator.Api.Endpoints.Users;

public class ChangePassword : Endpoint<ChangePasswordRequest, ApiResult>
{
    private readonly IUserChangePasswordService _userChangePasswordService;

    public ChangePassword(IUserChangePasswordService userChangePasswordService)
    {
        _userChangePasswordService = userChangePasswordService;
    }

    public override void Configure()
    {
        Post("api/v1/users/change-password");
        Summary(s =>
        {
            s.Summary = "Change password";
            s.Description = "Changes the current user's password.";
        });
        Description(d => d.Produces(200).WithTags(EndpointsTags.Users));
    }

    public override async Task HandleAsync(ChangePasswordRequest request, CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.Name);
        await Send.OkAsync(await _userChangePasswordService.ChangePassword(userId, request), cancellation: ct);
    }
}
