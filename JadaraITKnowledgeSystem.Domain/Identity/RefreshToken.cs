using JadaraITKnowledgeSystem.Domain.Common;

namespace JadaraITKnowledgeSystem.Domain.Identity
{
    /// <summary>
    /// A rotating refresh token. Only a hash of the token is stored; the raw value exists
    /// only in the client and in the response that issued it.
    /// </summary>
    public sealed class RefreshToken : BaseEntity
    {
        public int UserId { get; private set; }
        public string TokenHash { get; private set; } = string.Empty;
        public DateTime ExpiresAt { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public string CreatedByIp { get; private set; } = string.Empty;
        public bool IsRevoked { get; private set; }
        public DateTime? RevokedAt { get; private set; }
        public string? RevokedByIp { get; private set; }

        private RefreshToken() { }

        public static RefreshToken Create(int userId, string tokenHash, DateTime createdAt, DateTime expiresAt, string createdByIp)
        {
            if (userId <= 0) throw new ArgumentOutOfRangeException(nameof(userId));
            if (string.IsNullOrWhiteSpace(tokenHash)) throw new ArgumentException("Token hash is required.", nameof(tokenHash));

            return new RefreshToken
            {
                UserId = userId,
                TokenHash = tokenHash,
                CreatedAt = createdAt,
                ExpiresAt = expiresAt,
                CreatedByIp = createdByIp
            };
        }

        public bool IsActive(DateTime utcNow) => !IsRevoked && utcNow < ExpiresAt;

        public void Revoke(string revokedByIp, DateTime utcNow)
        {
            if (IsRevoked) return;

            IsRevoked = true;
            RevokedAt = utcNow;
            RevokedByIp = revokedByIp;
        }
    }
}
