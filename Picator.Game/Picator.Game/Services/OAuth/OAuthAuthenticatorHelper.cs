using Xamarin.Auth;

namespace Picator.Game.Services.OAuth;

public static class OAuthAuthenticatorHelper
{
    private static OAuth2Authenticator? _oAuth2Authenticator;

    public static OAuth2Authenticator CreateOAuth2()
    {
        _oAuth2Authenticator = new OAuth2Authenticator(
            clientId: GoogleConfigurations.ClientId,
            clientSecret: GoogleConfigurations.ClientSecret,
            scope: GoogleConfigurations.Scope,
            authorizeUrl: new Uri(GoogleConfigurations.AuthorizeUrl),
            redirectUrl: new Uri(GoogleConfigurations.RedirectUrl),
            getUsernameAsync: null,
            isUsingNativeUI: GoogleConfigurations.IsUsingNativeUI,
            accessTokenUrl: new Uri(GoogleConfigurations.AcessTokenUrl))
        {
            AllowCancel = true,
            ShowErrors = false,
            ClearCookiesBeforeLogin = true
        };


        AuthenticationState = _oAuth2Authenticator;
        return _oAuth2Authenticator;
    }

    public static OAuth2Authenticator? AuthenticationState { get; private set; }
}