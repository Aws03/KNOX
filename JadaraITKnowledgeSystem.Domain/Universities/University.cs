using JadaraITKnowledgeSystem.Domain.Common;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using JadaraITKnowledgeSystem.Domain.Universities.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace JadaraITKnowledgeSystem.Domain.Universities
{
    public sealed class University : AuditableEntity
    {
        //[Key]
        //public int UniversityId { get; private set; }

        [Required]
        [MaxLength(120)]
        public string Name { get; private set; }

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
                return Error.Validation("University name cannot be empty.");

            return new University(name.Trim().ToLower());
        }

        public void AddFaculty(Faculty faculty)
        {
            if (faculty == null) throw new ArgumentNullException(nameof(faculty));

            if (_faculties.Any(f => f.Equals(faculty)))
                throw new InvalidOperationException("Faculty already exists in this university.");

            _faculties.Add(faculty);
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
