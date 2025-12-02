using FastEndpoints;
using Picator.Api.Constants;
using Picator.Common.Data.Dtos.Api.Auth;
using Picator.Common.Data.Dtos.Users;
using Picator.Service.Contracts.Users;

namespace Picator.Api.Endpoints.Users;

public class Confirm : EndpointWithoutRequest<AuthResult>
{
    private readonly IUserConfirmService _userConfirmService;

    public Confirm(IUserConfirmService userConfirmService)
    {
        _userConfirmService = userConfirmService;
    }

    public override void Configure()
    {
        Get("api/v1/users/confirm-email");
        Summary(s =>
        {
            s.Summary = "sadasdas";
            s.Description = "desxvxcvxv";
        });
        Description(d => d.Produces(302).WithTags(EndpointsTags.Users));
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var email = Query<string>("email");
        var token = Query<string>("token");
        var request = new ConfirmEmailRequest { Email = email, Token = token };
        var confirmResult = await _userConfirmService.Confirm(request);
        var deepLink = $"app://ec.pctor/email-confirmation?isSuccess={confirmResult.IsSuccess}&token={Uri.EscapeDataString(confirmResult.Token)}&refresh_token={Uri.EscapeDataString(confirmResult?.RefreshToken)}&errors={Uri.EscapeDataString(string.Join(',', confirmResult.Errors ?? []))}";
        await Send.RedirectAsync(deepLink, true, true);
    }
}