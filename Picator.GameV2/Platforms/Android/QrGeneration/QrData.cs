using System.Globalization;
using System.Net;

namespace Picator.GameV2.Platforms.Android.QrGeneration;

/// <summary>
/// Content encoded into a QR code. Native port of a subset of customqrgenerator's QrData.kt —
/// only the variants <c>QrCodeType</c> (see <see cref="Views.Components.QrCodeType"/>) exposes.
/// </summary>
public interface IQrData
{
    string Encode();
}

public sealed record QrDataText(string Value) : IQrData
{
    public string Encode() => Value;
}

public sealed record QrDataUrl(string Url) : IQrData
{
    public string Encode() => Url;
}

public sealed record QrDataPhone(string PhoneNumber) : IQrData
{
    public string Encode() => $"TEL:{PhoneNumber}";
}

public sealed record QrDataGeoPos(float Lat, float Lon) : IQrData
{
    public string Encode() =>
        $"GEO:{Lat.ToString(CultureInfo.InvariantCulture)},{Lon.ToString(CultureInfo.InvariantCulture)}";
}

public sealed record QrDataEmail(string Email, string? CopyTo = null, string? Subject = null, string? Body = null)
    : IQrData
{
    public string Encode()
    {
        var sb = new System.Text.StringBuilder();
        sb.Append("mailto:").Append(Email);

        var hasQuery = !string.IsNullOrEmpty(CopyTo) || !string.IsNullOrEmpty(Subject) || !string.IsNullOrEmpty(Body);
        if (hasQuery)
            sb.Append('?');

        var queries = new List<string>();
        if (!string.IsNullOrEmpty(CopyTo))
            queries.Add($"cc={CopyTo}");
        if (!string.IsNullOrEmpty(Subject))
            queries.Add($"subject={Escape(Subject)}");
        if (!string.IsNullOrEmpty(Body))
            queries.Add($"body={Escape(Body)}");

        sb.Append(string.Join('&', queries));
        return sb.ToString();
    }

    private static string Escape(string text) => WebUtility.UrlEncode(text).Replace("+", " ");
}

public sealed record QrDataWifi(
    QrDataWifi.WifiAuthentication? Authentication = null,
    string? Ssid = null,
    string? Psk = null,
    bool Hidden = false) : IQrData
{
    public enum WifiAuthentication { Wep, Wpa, Open }

    public string Encode()
    {
        var sb = new System.Text.StringBuilder("WIFI:");
        if (!string.IsNullOrEmpty(Ssid))
            sb.Append("S:").Append(Escape(Ssid)).Append(';');
        if (Authentication is { } auth)
            sb.Append("T:").Append(AuthToken(auth)).Append(';');
        if (!string.IsNullOrEmpty(Psk))
            sb.Append("P:").Append(Escape(Psk)).Append(';');
        sb.Append("H:").Append(Hidden ? "true" : "false").Append(';');
        return sb.ToString();
    }

    private static string AuthToken(WifiAuthentication auth) => auth switch
    {
        WifiAuthentication.Wep => "WEP",
        WifiAuthentication.Wpa => "WPA",
        WifiAuthentication.Open => "nopass",
        _ => "nopass",
    };

    internal static string Escape(string text) => text
        .Replace("\\", "\\\\")
        .Replace(",", "\\,")
        .Replace(";", "\\;")
        .Replace(".", "\\.")
        .Replace("\"", "\\\"")
        .Replace("'", "\\'");
}
