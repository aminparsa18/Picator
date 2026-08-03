using Grpc.Net.Client;

namespace Picator.Game.Helpers;

/// <summary>
/// Creates gRPC channels for the game/matchmaking hubs.
/// </summary>
public static class GrpcChannelFactory
{
    /// <summary>
    /// Creates a channel for <paramref name="address"/>. On Android debug builds, bypasses server
    /// certificate validation so a real device can reach a local Aspire-hosted backend over HTTPS
    /// using the untrusted ASP.NET Core dev certificate (Android never trusts that CA, regardless
    /// of hostname). Never used in release builds.
    ///
    /// Must use SocketsHttpHandler, not HttpClientHandler - gRPC on Android is incompatible with
    /// HttpClientHandler/Android's native HTTP/2 stack (https://aka.ms/aspnet/grpc/android).
    /// </summary>
    public static GrpcChannel Create(string address)
    {
#if ANDROID && DEBUG
        return GrpcChannel.ForAddress(address, new GrpcChannelOptions
        {
            HttpHandler = new SocketsHttpHandler
            {
                SslOptions = new System.Net.Security.SslClientAuthenticationOptions
                {
                    RemoteCertificateValidationCallback = (_, _, _, _) => true
                }
            }
        });
#else
        return GrpcChannel.ForAddress(address);
#endif
    }
}
