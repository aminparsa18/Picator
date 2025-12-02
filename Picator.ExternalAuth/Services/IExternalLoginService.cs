namespace Picator.ExternalAuth.Services;

public interface IExternalLoginService
{
    Task<ExtrenalAuthResult> Login(string email);
}