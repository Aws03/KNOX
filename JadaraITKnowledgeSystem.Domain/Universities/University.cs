using System.ComponentModel.DataAnnotations;
using JadaraITKnowledgeSystem.Domain.Common;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using JadaraITKnowledgeSystem.Domain.Universities.Entities;

namespace JadaraITKnowledgeSystem.Domain.Universities
{
    public sealed class University : AuditableEntity
    {
        [Required]
        [MaxLength(120)]
        public string Name { get; private set; } = string.Empty;

        private readonly List<Faculty> _faculties = new();
        public IReadOnlyCollection<Faculty> Faculties => _faculties.AsReadOnly();

        // EF Core
        private University() { }

        private University(string name)
        {
            Name = name;
        }

        public static Result<University> Create(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Error.Validation("University.Name.Required", "University name cannot be empty.");

            return new University(name.Trim().ToLower());
        }

        public void UpdateName(string name) => SetName(name);

        private void SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("University name is required.", nameof(name));

            var trimmed = name.Trim();
            if (trimmed.Length > 120)
                throw new ArgumentException("University name must be 120 characters or fewer.", nameof(name));

            Name = trimmed;
        }
    }
}
