using System;
using System.Collections.Generic;
using System.Text;

namespace JadaraITKnowledgeSystem.Application.Interfaces
{
    public interface ICurrentUserService
    {
        int? UserId { get; }
        string? Name { get; }
        string? Email { get; }
    }
}
