using JadaraITKnowledgeSystem.Domain.Common;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace JadaraITKnowledgeSystem.Domain.Users.Entities
{
    public class VerificationOTP : AuditableEntity
    {
        public string OTP { get; private set; } = string.Empty;

        [ForeignKey(nameof(User))]
        public int UserId { get; private set; }
        public User User { get; private set; } = default!;

        public DateTime ExpiresAt { get; private set; }
        public bool IsUsed { get; private set; }

        private VerificationOTP() { }

        private VerificationOTP(int userId, string otp, DateTime expiresAt)
        {
            if (userId <= 0) throw new ArgumentOutOfRangeException(nameof(userId));
            if (string.IsNullOrWhiteSpace(otp)) throw new ArgumentException("OTP is required", nameof(otp));

            UserId = userId;
            OTP = otp;
            ExpiresAt = expiresAt;
            IsUsed = false;
        }


        public static VerificationOTP Create(int userId, string otp, DateTime expiresAt)
        {
            return new VerificationOTP(userId, otp, expiresAt);
        }

        public void MarkUsed()
        {
            IsUsed = true;
        }

        public bool IsExpired() => DateTime.UtcNow > ExpiresAt;
    }
}
