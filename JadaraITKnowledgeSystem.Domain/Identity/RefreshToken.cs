using JadaraITKnowledgeSystem.Domain.Common;

namespace JadaraITKnowledgeSystem.Domain.Identity
{
    public sealed class RefreshToken : BaseEntity
    {
        public int UserId { get; private set; }
        public string Token { get; private set; }
        public DateTime ExpiresAt { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public bool IsRevoked { get; private set; }
        public string? RevokedByIp { get; private set; }
        public DateTime? RevokedAt { get; private set; }
        public string CreatedByIp { get; private set; }

        private RefreshToken() { }

        private RefreshToken(int userId, string token, DateTime expiresAt, string createdByIp)
        {
            UserId = userId;
            Token = token;
            ExpiresAt = expiresAt;
            CreatedAt = DateTime.UtcNow;
            CreatedByIp = createdByIp;
            IsRevoked = false;
        }

        public static RefreshToken Create(int userId, string token, DateTime expiresAt, string createdByIp)
        {
            return new RefreshToken(userId, token, expiresAt, createdByIp);
        }

        public void Revoke(string revokedByIp)
        {
            IsRevoked = true;
            RevokedAt = DateTime.UtcNow;
            RevokedByIp = revokedByIp;
        }

        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
        public bool IsActive => !IsRevoked && !IsExpired;
    }
}
