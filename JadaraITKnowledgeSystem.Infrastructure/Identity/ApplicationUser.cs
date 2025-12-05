using JadaraITKnowledgeSystem.Domain.Users;
using Microsoft.AspNetCore.Identity;

namespace JadaraITKnowledgeSystem.Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser<int>
    {
        public string FullName { get; set; } = string.Empty;
        public DateTime DateJoined { get; set; } = DateTime.UtcNow;

        public int DomainUserId { get; set; }
        public User DomainUser { get; set; } = null!;
    }
}
