using FastEndpoints;
using Picator.Api.Constants;
using Picator.Common.Data.Dtos.Api.Auth;
using Picator.Common.Data.Dtos.Users;
using Picator.Service.Contracts.Users;

namespace Picator.Api.Endpoints.Users;

public class Confirm : Endpoint<ConfirmEmailRequest, AuthResult>
{
    private readonly IUserConfirmService _userConfirmService;

    public Confirm(IUserConfirmService userConfirmService)
    {
        _userConfirmService = userConfirmService;
    }

    public override void Configure()
    {
        Post("api/v1/users/confirm");
        Summary(s =>
        {
            s.Summary = "sadasdas";
            s.Description = "desxvxcvxv";
        });
        Description(d => d.Produces(200).WithTags(EndpointsTags.Users));
    }

    public override async Task HandleAsync(ConfirmEmailRequest request, CancellationToken ct) =>
        await SendMemoryPackAsync(await _userConfirmService.Confirm(request), cancellation: ct);
}