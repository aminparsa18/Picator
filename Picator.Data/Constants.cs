namespace Picator.Data;

/// <summary>
/// Constants.
/// </summary>
public sealed class Constants
{
    /// <summary>
    /// Admin role.
    /// </summary>
    public static string AdminRole => "Administrator";

    /// <summary>
    /// Player role.
    /// </summary>
    public static string PlayerRole => "Player";

    /// <summary>
    /// RustFs blob storage endpoint. Routed through the Traefik ingress (rustfsStorageHostname
    /// in Picator.AppHost/Program.cs) rather than RustFS's raw NodePort, so avatar URLs handed
    /// back to browser clients don't get blocked as mixed content on the https web app.
    /// </summary>
    public static string BlobStorageEndpoint => "https://kososher.picator.online/avatars/";
}