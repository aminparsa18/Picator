using FastEndpoints;
using Picator.Api.Constants;
using Picator.Common.Data.Dtos.Api;
using Picator.Common.Data.Dtos.Users;
using Picator.Service.Contracts.Users;
using System.Security.Claims;

namespace Picator.Api.Endpoints.Users;

public class Update : Endpoint<UpdateProfileRequest, ApiResult>
{
    private readonly IUserUpdateService _userUpdateService;

    public Update(IUserUpdateService userUpdateService)
    {
        _userUpdateService = userUpdateService;
    }

    public override void Configure()
    {
        Post("api/v1/users/update");
        Summary(s =>
        {
            s.Summary = "sadasdas";
            s.Description = "desxvxcvxv";
        });
        Description(d => d.Produces(200).WithTags(EndpointsTags.Users));
    }

    public override async Task HandleAsync(UpdateProfileRequest request, CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.Name);
        await SendMemoryPackAsync(await _userUpdateService.Update(userId, request), cancellation: ct);
    }
}