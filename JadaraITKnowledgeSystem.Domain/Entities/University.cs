using JadaraITKnowledgeSystem.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace JadaraITKnowledgeSystem.Domain.Entities
{
    public class University : AuditableEntity
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

        public University(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("University name is required", nameof(name));

            Name = name.Trim();
        }

        public void AddFaculty(Faculty faculty)
        {
            if (faculty == null) throw new ArgumentNullException(nameof(faculty));

            if (_faculties.Any(f => f.Equals(faculty)))
                throw new InvalidOperationException("Faculty already exists in this university.");

            _faculties.Add(faculty);
        }
    }
}
