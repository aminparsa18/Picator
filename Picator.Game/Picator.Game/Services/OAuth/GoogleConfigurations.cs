namespace Picator.Game.Services.OAuth;

public static class GoogleConfigurations
{
    public static readonly string ClientId = "890605411237-de23cp1r0r89ihtf65k0rihc8fpl24gi.apps.googleusercontent.com";
    public static readonly string Scope = "profile";
    public static readonly string ClientSecret = "";
    public static readonly string AuthorizeUrl = "https://accounts.google.com/o/oauth2/auth";
    public static readonly string RedirectUrl = "app://callback.pctor";
    public static readonly string AcessTokenUrl = "https://www.googleapis.com/oauth2/v4/token";
    public static bool IsUsingNativeUI = true;
}