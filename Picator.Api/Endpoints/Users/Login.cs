using FastEndpoints;
using Picator.Api.Constants;
using Picator.Common.Data.Dtos.Api.Auth;
using Picator.Common.Data.Dtos.Users;
using Picator.Service.Contracts.Users;

namespace Picator.Api.Endpoints.Users;

public class Login : Endpoint<UserLoginRequest, AuthResult>
{
    private readonly IUserLoginService _userLoginService;

    public Login(IUserLoginService userLoginService)
    {
        _userLoginService = userLoginService;
    }

    public override void Configure()
    {
        Post("api/v1/users/login");
        Summary(s =>
        {
            s.Summary = "sadasdas";
            s.Description = "desxvxcvxv";
        });
        Description(d => d.Accepts<UserLoginRequest>("application/x-memorypack").Produces<AuthResult>(200, "application/x-memorypack").WithTags(EndpointsTags.Users));
        AllowAnonymous();
    }

    public override async Task HandleAsync(UserLoginRequest request, CancellationToken ct) {
        
        var login = await _userLoginService.Login(request);
        await Send.OkAsync(login, cancellation: ct);

    }
}