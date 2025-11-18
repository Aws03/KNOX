using MediatR;
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

        private readonly List<INotification> _domainEvents = new();
        public IReadOnlyList<INotification> DomainEvents => _domainEvents.AsReadOnly();

        protected BaseEntity() { }
        protected BaseEntity(int id)
        {
            Id = id;
        }

        public void AddDomainEvent(INotification eventItem)
            => _domainEvents.Add(eventItem);

        public void ClearDomainEvents()
            => _domainEvents.Clear();

        public void RemoveDomainEvent(INotification eventItem)
            => _domainEvents.Remove(eventItem);

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
