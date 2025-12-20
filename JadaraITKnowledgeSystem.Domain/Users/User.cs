using JadaraITKnowledgeSystem.Domain.Common;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using JadaraITKnowledgeSystem.Domain.Universities.Entities;
using JadaraITKnowledgeSystem.Domain.Users.ValueObjects;

namespace JadaraITKnowledgeSystem.Domain.Users
{
    public sealed class User : AuditableEntity
    {
        public FullName Name { get; private set; }
        public Email Email { get; private set; }
        //public DateTime DateJoined { get; private set; } // no need , there is an auditable mechanisime

        public int MajorId { get; private set; }
        public Major Major { get; private set; }

        public bool IsVerfied { get; private set; } = false;
        public DateTime? VerficationDate { get; private set; } = null;

        public bool IsActive { get; private set; } = true; // default active
        public string? ProfilePictureUrl { get; private set; } = null;

        private User() { }

        private User(FullName name, Email email, int majorId)
        {
            ArgumentNullException.ThrowIfNull(name);
            ArgumentNullException.ThrowIfNull(email);
            if (majorId <= 0) throw new ArgumentException("MajorId must be a positive integer.", nameof(majorId));
            
            Name = name;
            Email = email;
            MajorId = majorId;
        }

        public static Result<User> Create(FullName name, Email email, int majorId)
        {
            return new User(name, email, majorId);
        }

        public Result<Success> VerifyAccount()
        {
            if (IsVerfied)
                return Result.Success;

            IsVerfied = true;
            VerficationDate = DateTime.UtcNow;
            return Result.Success;
        }

        public Result<Success> BlockAccount()
        {
            if(IsActive)
            IsActive = false;

            return Result.Success;
        }
        
        public Result<Success> ActivateAccount()
        {
            if (!IsActive)
                IsActive = true;

            return Result.Success;
        }

        public void UpdateName(FullName newName)
        {
            ArgumentNullException.ThrowIfNull(newName);
            Name = newName;
        }

        public void UpdateMajor(int newMajorId)
        {
            if (newMajorId <= 0)
                throw new ArgumentException("MajorId must be a positive integer.", nameof(newMajorId));
            
            MajorId = newMajorId;
        }

        public void SetProfilePicture(string pictureUrl)
        {
            if (string.IsNullOrWhiteSpace(pictureUrl))
                throw new ArgumentException("Profile picture URL cannot be empty.", nameof(pictureUrl));
            
            ProfilePictureUrl = pictureUrl;
        }

        public void RemoveProfilePicture()
        {
            ProfilePictureUrl = null;
        }
    }
}
