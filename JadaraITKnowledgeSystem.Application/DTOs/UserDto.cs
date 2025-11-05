using JadaraITKnowledgeSystem.Domain.Enums;
using JadaraITKnowledgeSystem.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadaraITKnowledgeSystem.Application.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }
        public FullName Name { get; set; }
        public Email Email { get; set; }
        public PermissionLevel PermissionLevel { get; set; }
        public DateTime DateJoined { get; set; } // remove later (when add auditablity)
    }
}
