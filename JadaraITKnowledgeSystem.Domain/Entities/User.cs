using JadaraITKnowledgeSystem.Domain.Common;
using JadaraITKnowledgeSystem.Domain.Enums;
using JadaraITKnowledgeSystem.Domain.ValueObjects;
using System.ComponentModel.DataAnnotations;

namespace JadaraITKnowledgeSystem.Domain.Entities
{
    public class User : AuditableEntity
    {
        //[Key]
        //public int UserId { get; private set; }
        public FullName Name { get; private set; }
        public Email Email { get; private set; }
        public PermissionLevel PermissionLevel { get; private set; }
        public DateTime DateJoined { get; private set; } // remove later (when add auditablity)

        private User() { }

        public User(FullName name, Email email, PermissionLevel permissionLevel)
        {
            Name = name;
            Email = email;
            PermissionLevel = permissionLevel;
            DateJoined = DateTime.UtcNow;
        }
    }

}
