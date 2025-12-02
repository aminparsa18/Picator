using FastEndpoints;
using Picator.Api.Constants;
using Picator.Common.Data.Dtos.Api;
using Picator.Common.Data.Dtos.Users;
using Picator.Service.Contracts.Users;
using System.Security.Claims;

namespace Picator.Api.Endpoints.Users;

public class Get : EndpointWithoutRequest<ApiResult<UserDetailsResult>>
{
    private readonly IUserService _userService;

    public Get(IUserService userService)
    {
        _userService = userService;
    }

    public override void Configure()
    {
        Get("api/v1/users");
        Summary(s =>
        {
            s.Summary = "sadasdas";
            s.Description = "desxvxcvxv";
        });
        Description(d => d.Produces(200).WithTags(EndpointsTags.Users));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.Name);
        await Send.OkAsync(await _userService.GetDetails(userId), cancellation: ct);
    }
}