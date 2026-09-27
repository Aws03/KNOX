namespace JadaraITKnowledgeSystem.Domain.Common
{
    public abstract class BaseEntity
    {
        public int Id { get; protected set; }

        protected BaseEntity() { }

        protected BaseEntity(int id)
        {
            Id = id;
        }

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

        public override int GetHashCode() => HashCode.Combine(GetType(), Id);
    }
}
