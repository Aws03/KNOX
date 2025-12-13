using JadaraITKnowledgeSystem.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace JadaraITKnowledgeSystem.Domain.Users.Entities
{
    public class WriterApplication : AuditableEntity
    {
        public string PhoneNumber { get; private set; } = string.Empty;

        [ForeignKey(nameof(User))]
        public int UserId { get; private set; }
        public User User { get; private set; } = default!;

        private WriterApplication() { }

        private WriterApplication(int userId, string phoneNumber)
        {
            if (userId <= 0) throw new ArgumentOutOfRangeException(nameof(userId));
            if (string.IsNullOrWhiteSpace(phoneNumber)) throw new ArgumentException("Phone number is required", nameof(phoneNumber));

            UserId = userId;
            PhoneNumber = phoneNumber;
        }

        public static WriterApplication Create(int userId, string phoneNumber)
        {
            return new WriterApplication(userId, phoneNumber);
        }
    }
}
