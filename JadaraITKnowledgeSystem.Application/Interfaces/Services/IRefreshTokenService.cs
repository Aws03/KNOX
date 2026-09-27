namespace JadaraITKnowledgeSystem.Application.Interfaces.Services;

public interface IRefreshTokenService
{
    Task<IssuedRefreshToken> IssueAsync(int identityUserId, string ipAddress, CancellationToken cancellationToken = default);

    /// <summary>
    /// Consumes (revokes) the presented token and returns its owner's identity user id if it
    /// was active. Presenting an already-rotated token revokes all of that user's tokens,
    /// since it means a copy of the token is in someone else's hands.
    /// </summary>
    Task<int?> RedeemAsync(string token, string ipAddress, CancellationToken cancellationToken = default);

    /// <summary>Revokes one refresh token. Possessing the token is proof enough, so no user id is needed.</summary>
    Task RevokeAsync(string token, string ipAddress, CancellationToken cancellationToken = default);

    Task RevokeAllAsync(int identityUserId, string ipAddress, CancellationToken cancellationToken = default);
}

public sealed record IssuedRefreshToken(string Token, DateTime ExpiresAt);
