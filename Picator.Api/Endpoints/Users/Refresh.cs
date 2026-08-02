using FastEndpoints;
using Picator.Api.Constants;
using Picator.Common.Data.Dtos.Api.Auth;
using Picator.Service.Contracts.RefreshTokens;

namespace Picator.Api.Endpoints.Users;

public class Refresh : Endpoint<RefreshTokenRequest, AuthResult>
{
    private readonly IRefreshTokenService _refreshTokenService;

    public Refresh(IRefreshTokenService refreshTokenService)
    {
        _refreshTokenService = refreshTokenService;
    }

    public override void Configure()
    {
        Post("api/v1/users/refresh");
        Summary(s =>
        {
            s.Summary = "sadasdas";
            s.Description = "desxvxcvxv";
        });
        Description(d => d.Accepts<RefreshTokenRequest>("application/x-memorypack").Produces(200).WithTags(EndpointsTags.Users));
    }

    public override async Task HandleAsync(RefreshTokenRequest request, CancellationToken ct) =>
        await Send.OkAsync(await _refreshTokenService.Refresh(request), cancellation: ct);
}