using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadaraITKnowledgeSystem.Domain.Common
{
    public abstract class AuditableEntity : BaseEntity
    {
        public DateTimeOffset CreatedAt { get; private set; }
        public string CreatedBy { get; private set; } = string.Empty;
        public DateTimeOffset? UpdatedAt { get; private set; }
        public string? UpdatedBy { get; private set; }

        protected AuditableEntity()
        {
            CreatedAt = DateTimeOffset.UtcNow;
        }

        public void SetCreated(string user)
        {
            CreatedAt = DateTimeOffset.UtcNow;
            CreatedBy = user;
        }

        public void SetUpdated(string user)
        {
            UpdatedAt = DateTimeOffset.UtcNow;
            UpdatedBy = user;
        }
    }
}
