using FastEndpoints;
using Picator.Api.Constants;
using Picator.Common.Data.Dtos.Api;
using Picator.Common.Data.Dtos.Api.Auth;
using Picator.Service.Contracts.RefreshTokens;
using System.Security.Claims;

namespace Picator.Api.Endpoints.Users;

public class Logout : Endpoint<LogoutRequest, ApiResult>
{
    private readonly IRefreshTokenService _refreshTokenService;

    public Logout(IRefreshTokenService refreshTokenService)
    {
        _refreshTokenService = refreshTokenService;
    }

    public override void Configure()
    {
        Post("api/v1/users/logout");
        Summary(s =>
        {
            s.Summary = "Logout";
            s.Description = "Invalidates the current refresh token so it can no longer be used to mint new access tokens.";
        });
        Description(d => d.Accepts<LogoutRequest>("application/x-memorypack").Produces(200).WithTags(EndpointsTags.Users));
    }

    public override async Task HandleAsync(LogoutRequest request, CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.Name);
        await Send.OkAsync(await _refreshTokenService.Logout(userId, request), cancellation: ct);
    }
}
