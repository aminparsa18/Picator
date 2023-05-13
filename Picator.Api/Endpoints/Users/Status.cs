using FastEndpoints;
using Picator.Api.Constants;
using Picator.Common.Data.Dtos.Api;
using Picator.Common.Data.Dtos.Users;
using Picator.Service.Contracts.Users;
using System.Security.Claims;

namespace Picator.Api.Endpoints.Users;

public class Status : EndpointWithoutRequest<ApiResult<UserStatusResult>>
{
    private readonly IUserService _userService;

    public Status(IUserService userService)
    {
        _userService = userService;
    }

    public override void Configure()
    {
        Get("api/v1/users/status");
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
        await SendMemoryPackAsync(await _userService.GetStatus(userId), cancellation: ct);
    }
}