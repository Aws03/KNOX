using System.Security.Cryptography;
using System.Text;
using JadaraITKnowledgeSystem.Domain.Common;

namespace JadaraITKnowledgeSystem.Domain.Users.Entities
{
    /// <summary>
    /// A one-time code emailed to a user for account verification or password reset.
    /// A code is consumed by a correct guess, and burned after <see cref="MaxFailedAttempts"/>
    /// wrong ones so it cannot be brute-forced within its lifetime.
    /// </summary>
    public class VerificationOTP : AuditableEntity
    {
        public const int MaxFailedAttempts = 5;

        public string OTP { get; private set; } = string.Empty;

        public int UserId { get; private set; }
        public User User { get; private set; } = null!;

        public DateTime ExpiresAt { get; private set; }
        public bool IsUsed { get; private set; }
        public int FailedAttempts { get; private set; }

        private VerificationOTP() { }

        private VerificationOTP(int userId, string otp, DateTime expiresAt)
        {
            if (userId <= 0) throw new ArgumentOutOfRangeException(nameof(userId));
            if (string.IsNullOrWhiteSpace(otp)) throw new ArgumentException("OTP is required", nameof(otp));

            UserId = userId;
            OTP = otp;
            ExpiresAt = expiresAt;
        }

        public static VerificationOTP Create(int userId, string otp, DateTime expiresAt) => new(userId, otp, expiresAt);

        public bool IsActive(DateTime utcNow) => !IsUsed && utcNow < ExpiresAt;

        /// <summary>Checks <paramref name="code"/>; returns true (and consumes the code) only on a match.</summary>
        public bool TryVerify(string code, DateTime utcNow)
        {
            if (!IsActive(utcNow))
                return false;

            var matches = CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(OTP), Encoding.UTF8.GetBytes(code ?? string.Empty));

            if (matches)
            {
                IsUsed = true;
                return true;
            }

            FailedAttempts++;
            if (FailedAttempts >= MaxFailedAttempts)
                IsUsed = true;

            return false;
        }

        public void MarkUsed() => IsUsed = true;
    }
}
