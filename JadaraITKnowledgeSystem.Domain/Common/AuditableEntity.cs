using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadaraITKnowledgeSystem.Domain.Common
{
    public abstract class AuditableEntity : BaseEntity
    {
        public DateTime CreatedAt { get; private set; }
        public string CreatedBy { get; private set; } = string.Empty;
        public DateTime? UpdatedAt { get; private set; }
        public string? UpdatedBy { get; private set; }

        protected AuditableEntity()
        {
            CreatedAt = DateTime.UtcNow;
        }

        public void SetCreated(string user)
        {
            CreatedAt = DateTime.UtcNow;
            CreatedBy = user;
        }

        public void SetUpdated(string user)
        {
            UpdatedAt = DateTime.UtcNow;
            UpdatedBy = user;
        }
    }
}
