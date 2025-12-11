using System;

namespace JadaraITKnowledgeSystem.Application.Fetures.Users.Dtos
{
    public sealed class UserProfileDto
    {
        // Identity
        public int IdentityUserId { get; init; }
        public int DomainUserId { get; init; }
        public string Email { get; init; } = string.Empty;
        public string FullName { get; init; } = string.Empty;
        public DateTime DateJoined { get; init; }
        public string Role { get; init; } = "User";

        // Domain
        public bool IsActive { get; init; }
        public bool IsVerfied { get; init; }
        public DateTime? VerficationDate { get; init; }
        public string? ProfilePictureUrl { get; init; }

        // Academic
        public int UniversityId { get; init; }
        public string UniversityName { get; init; } = string.Empty;
        public int FacultyId { get; init; }
        public string FacultyName { get; init; } = string.Empty;
        public int MajorId { get; init; }
        public string MajorName { get; init; } = string.Empty;
    }
}
