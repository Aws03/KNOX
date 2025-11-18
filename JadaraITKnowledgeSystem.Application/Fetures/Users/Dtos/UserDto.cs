using JadaraITKnowledgeSystem.Domain.Users.Enums;
using JadaraITKnowledgeSystem.Domain.Users.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadaraITKnowledgeSystem.Application.Fetures.Users.Dtos
{
    public sealed record UserDto
    {
        public int Id { get; init; }
        public required FullName Name { get; init; } 
        public required Email Email { get; init; } 
        public PermissionLevel PermissionLevel { get; init; }
    }
}
