using JadaraITKnowledgeSystem.Domain.Common;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using JadaraITKnowledgeSystem.Domain.Courses.Entites;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadaraITKnowledgeSystem.Domain.Courses
{
    public sealed class Course : AuditableEntity
    {
        //[Key]
        //public int CourseId { get; private set; }
        [MaxLength(120)]
        public string CourseName { get; private set; }
        [MaxLength(500)]
        public string? Description { get; private set; }
        [MaxLength(20)]
        public string? CourseCode { get; private set; }
        public int? Credits { get; private set; }

        private readonly List<CourseRequirementMapping> _requirements = new();
        public IReadOnlyCollection<CourseRequirementMapping> Requirements => _requirements.AsReadOnly();

        private Course() { }

        private Course(string courseName, int? credits, string? description = null, string? courseCode = null)
        {
            SetName(courseName);
            SetCredits(credits);
            SetDescription(description);
            SetCourseCode(courseCode);
        }

        public static Result<Course> Create(string courseName, int? credits, string? description = null, string? courseCode = null)
        {
            return new Course(courseName, credits, description, courseCode);
        }

        private void SetName(string courseName)
        {
            if (string.IsNullOrWhiteSpace(courseName))
                throw new ArgumentException("Course name is required");
            CourseName = courseName;
        }

        public void ChangeName(string newName)
        {
            SetName(newName);
        }

        private void SetDescription(string? description)
        {
            if(description != null && description.Length > 500)
                throw new ArgumentException("Description must be 500 characters or fewer.", nameof(Description));

            Description = description;
        }

        public void ChangeDescription(string? newDescription)
        {
            if(string.IsNullOrWhiteSpace(newDescription))
                newDescription = null;

            SetDescription(newDescription);
        }

        private void SetCourseCode(string? courseCode)
        {
            if(courseCode != null && courseCode.Length > 20)
                throw new ArgumentException("Course code must be 20 characters or fewer.", nameof(CourseCode));
            CourseCode = courseCode;
        }

        public void ChangeCourseCode(string? newCourseCode)
        {
            if(string.IsNullOrWhiteSpace(newCourseCode))
                newCourseCode = null;
            SetCourseCode(newCourseCode);
        }

        private void SetCredits(int? credits)
        {
            if(credits != null && (credits < 0 || credits > 10))
                throw new ArgumentException("Credits must be between 0 and 10.", nameof(Credits));
            Credits = credits;
        }

        public void ChangeCredits(int? newCredits)
        {
            SetCredits(newCredits);
        }

        public Result<CourseRequirementMapping> AssignToMajor(int majorId, Domain.Courses.Enums.RequirementType requirementType, Domain.Courses.Enums.RequirementNature requirementNature)
        {
            if (majorId <= 0)
                return Error.Validation("MajorId.Invalid", "MajorId must be a positive integer.");

            // Prevent duplicate mapping for same major
            if (_requirements.Any(r => r.MajorId == majorId))
                return Error.Conflict("Course.AlreadyAssigned", "This course is already assigned to the specified major.");

            var mappingResult = CourseRequirementMapping.Create(this.Id, majorId, requirementType, requirementNature);
            if (mappingResult.IsError)
                return mappingResult;

            _requirements.Add(mappingResult.Value);
            return mappingResult;
        }

        // helper to check if already assigned (used by handlers before Attach)
        public bool IsAssignedToMajor(int majorId) => _requirements.Any(r => r.MajorId == majorId);
    }
}
