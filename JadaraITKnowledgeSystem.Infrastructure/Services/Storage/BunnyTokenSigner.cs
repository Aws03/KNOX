using System.Security.Cryptography;
using System.Text;

namespace JadaraITKnowledgeSystem.Infrastructure.Services.Storage;

/// <summary>
/// Bunny CDN token authentication (SHA-256): token = base64url(sha256(key + path + expires)).
/// The pull zone rejects requests whose token is missing, wrong or expired, so a signed URL is the
/// only way to read a private object through the CDN.
/// </summary>
public static class BunnyTokenSigner
{
    public static Uri Sign(string baseUrl, string key, string tokenKey, DateTimeOffset expiresAt)
    {
        var path = "/" + string.Join('/', key.Split('/').Select(Uri.EscapeDataString));
        var expires = expiresAt.ToUnixTimeSeconds();

        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(tokenKey + path + expires));
        var token = Convert.ToBase64String(hash).Replace('+', '-').Replace('/', '_').TrimEnd('=');

        return new Uri($"{baseUrl.TrimEnd('/')}{path}?token={token}&expires={expires}");
    }
}
