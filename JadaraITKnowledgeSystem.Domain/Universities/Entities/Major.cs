using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using JadaraITKnowledgeSystem.Domain.Common;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using JadaraITKnowledgeSystem.Domain.Courses.Entities;
using JadaraITKnowledgeSystem.Domain.Users;

namespace JadaraITKnowledgeSystem.Domain.Universities.Entities
{
    public sealed class Major : AuditableEntity
    {
        [Required]
        [MaxLength(120)]
        public string Name { get; private set; } = string.Empty;

        [ForeignKey(nameof(Faculty))]
        public int FacultyId { get; private set; }
        public Faculty Faculty { get; private set; } = null!;

        private readonly List<CourseRequirementMapping> _courseRequirements = new();
        public IReadOnlyCollection<CourseRequirementMapping> CourseRequirements => _courseRequirements.AsReadOnly();

        private Major() { }

        private Major(string name,int facultyId)
        {
            SetName(name);
            SetFacultyId(facultyId);
        }

        public static Result<Major> Create(string name,int facultyId)
        {
            return new Major(name,facultyId);
        }

        public void UpdateName(string name) => SetName(name);

        public void UpdateFaculty(int facultyId) => SetFacultyId(facultyId);

        private void SetFacultyId(int facultyId)
        {
            if (facultyId <= 0)
                throw new ArgumentOutOfRangeException(nameof(facultyId), "FacultyId must be a positive integer.");
            FacultyId = facultyId;
        }

        private void SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Major name is required.", nameof(name));

            var trimmed = name.Trim();
            if (trimmed.Length > 120)
                throw new ArgumentException("Major name must be 120 characters or fewer.", nameof(name));

            Name = trimmed;
        }
    }
}
