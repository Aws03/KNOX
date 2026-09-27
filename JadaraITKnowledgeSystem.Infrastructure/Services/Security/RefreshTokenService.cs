using System.Security.Cryptography;
using System.Text;
using JadaraITKnowledgeSystem.Application.Interfaces;
using JadaraITKnowledgeSystem.Application.Interfaces.Services;
using JadaraITKnowledgeSystem.Domain.Identity;
using JadaraITKnowledgeSystem.Infrastructure.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace JadaraITKnowledgeSystem.Infrastructure.Services.Security;

public sealed class RefreshTokenService(
    IApplicationDbContext context,
    IOptions<JwtOptions> jwtOptions,
    TimeProvider timeProvider,
    ILogger<RefreshTokenService> logger) : IRefreshTokenService
{
    public async Task<IssuedRefreshToken> IssueAsync(int identityUserId, string ipAddress, CancellationToken cancellationToken = default)
    {
        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        var now = UtcNow;
        var expiresAt = now.AddDays(jwtOptions.Value.RefreshTokenDays);

        context.RefreshTokens.Add(RefreshToken.Create(identityUserId, Hash(token), now, expiresAt, ipAddress));
        await context.SaveChangesAsync(cancellationToken);

        return new IssuedRefreshToken(token, expiresAt);
    }

    public async Task<int?> RedeemAsync(string token, string ipAddress, CancellationToken cancellationToken = default)
    {
        var stored = await FindAsync(token, cancellationToken);
        if (stored is null)
            return null;

        var now = UtcNow;
        if (!stored.IsActive(now))
        {
            if (stored.IsRevoked && now < stored.ExpiresAt)
            {
                logger.LogWarning("Rotated refresh token reused for user {UserId}; revoking all of their tokens", stored.UserId);
                await RevokeAllAsync(stored.UserId, ipAddress, cancellationToken);
            }
            return null;
        }

        stored.Revoke(ipAddress, now);
        await context.SaveChangesAsync(cancellationToken);
        return stored.UserId;
    }

    public async Task RevokeAsync(string token, int identityUserId, string ipAddress, CancellationToken cancellationToken = default)
    {
        var stored = await FindAsync(token, cancellationToken);
        if (stored is null || stored.UserId != identityUserId)
            return;

        stored.Revoke(ipAddress, UtcNow);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task RevokeAllAsync(int identityUserId, string ipAddress, CancellationToken cancellationToken = default)
    {
        // Tracked updates (not ExecuteUpdate) so entities already loaded in this context see the revocation.
        var now = UtcNow;
        var active = await context.RefreshTokens
            .Where(rt => rt.UserId == identityUserId && !rt.IsRevoked)
            .ToListAsync(cancellationToken);

        foreach (var token in active)
            token.Revoke(ipAddress, now);

        await context.SaveChangesAsync(cancellationToken);
    }

    private DateTime UtcNow => timeProvider.GetUtcNow().UtcDateTime;

    private Task<RefreshToken?> FindAsync(string token, CancellationToken cancellationToken) =>
        string.IsNullOrWhiteSpace(token)
            ? Task.FromResult<RefreshToken?>(null)
            : context.RefreshTokens.FirstOrDefaultAsync(rt => rt.TokenHash == Hash(token), cancellationToken);

    private static string Hash(string token) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
}
