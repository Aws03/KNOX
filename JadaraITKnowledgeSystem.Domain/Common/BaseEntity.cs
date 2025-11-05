using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadaraITKnowledgeSystem.Domain.Common
{
    public abstract class BaseEntity
    {
        public int Id { get; protected set; }

        public override bool Equals(object? obj)
        {
            if (obj is not BaseEntity other)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (GetType() != other.GetType())
                return false;

            return Id == other.Id && Id != 0;
        }

        public override int GetHashCode() => (GetType().ToString() + Id).GetHashCode();
    }
}
