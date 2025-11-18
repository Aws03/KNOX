using JadaraITKnowledgeSystem.Domain.Common;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JadaraITKnowledgeSystem.Domain.Universities.Entities
{
    public sealed class Faculty : AuditableEntity
    {
        //[Key]
        //public int FacultyId { get; private set; }

        [Required]
        [MaxLength(120)]
        public string Name { get; private set; }

        [ForeignKey(nameof(University))]
        public int UniversityId { get; private set; }
        public University University { get; private set; }

        private Faculty() { }

        //change to public
        private Faculty(string name, int universityId)
        {
            SetName(name);
            SetUniversityId(universityId);
        }

        public static Result<Faculty> Create(string name, int universityId)
        {
            return new Faculty(name, universityId);
        }

        public void UpdateName(string name) => SetName(name);

        public void UpdateUniversity(int universityId) => SetUniversityId(universityId);

        private void SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Faculty name is required.", nameof(name));

            var trimmed = name.Trim();
            if (trimmed.Length > 120)
                throw new ArgumentException("Faculty name must be 120 characters or fewer.", nameof(name));

            Name = trimmed;
        }

        private void SetUniversityId(int universityId)
        {
            if (universityId <= 0)
                throw new ArgumentOutOfRangeException(nameof(universityId), "UniversityId must be a positive integer.");

            UniversityId = universityId;
        }
    }
}