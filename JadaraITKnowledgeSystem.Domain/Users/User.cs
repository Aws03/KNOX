using JadaraITKnowledgeSystem.Domain.Common;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using JadaraITKnowledgeSystem.Domain.Users.Enums;
using JadaraITKnowledgeSystem.Domain.Users.ValueObjects;
using System.ComponentModel.DataAnnotations;

namespace JadaraITKnowledgeSystem.Domain.Users
{
    public sealed class User : AuditableEntity
    {
        //[Key]
        //public int UserId { get; private set; }
        public FullName Name { get; private set; }
        public Email Email { get; private set; }
        public PermissionLevel PermissionLevel { get; private set; }
        public DateTime DateJoined { get; private set; } // remove later (when add auditablity)

        private User() { }

        private User(FullName name, Email email, PermissionLevel permissionLevel)
        {
            Name = name;
            Email = email;
            PermissionLevel = permissionLevel;
            DateJoined = DateTime.UtcNow;
        }

        public static Result<User> Create(FullName name, Email email, PermissionLevel permissionLevel)
        {
            return new User(name, email, permissionLevel);
        }
    }

}
