using FastEndpoints;
using Picator.Api.Constants;
using Picator.Common.Data.Dtos.Api;
using Picator.Common.Data.Dtos.Api.Auth;
using Picator.Common.Data.Dtos.Users;
using Picator.Service.Contracts.Users;

namespace Picator.Api.Endpoints.Users;

public class Register : Endpoint<RegisterUserRequest, ApiResult>
{
    private readonly IUserRegisterService _userRegisterService;

    public Register(IUserRegisterService userRegisterService)
    {
        _userRegisterService = userRegisterService;
    }

    public override void Configure()
    {
        Post("api/v1/users/register");
        Summary(s =>
        {
            s.Summary = "Registers a new user";
        });
        Description(d => d.Accepts<UserLoginRequest>("application/x-memorypack").Produces<AuthResult>(200, "application/x-memorypack").WithTags(EndpointsTags.Users));
        AllowAnonymous();
    }

    public override async Task HandleAsync(RegisterUserRequest request, CancellationToken ct) =>
        await Send.OkAsync(await _userRegisterService.Register(request), cancellation: ct);
}