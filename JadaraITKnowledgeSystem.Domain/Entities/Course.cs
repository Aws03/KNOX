using JadaraITKnowledgeSystem.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadaraITKnowledgeSystem.Domain.Entities
{
    public class Course : AuditableEntity
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

        public Course(string courseName, int credits, string? description = null, string? courseCode = null)
        {
            SetName(courseName);
            SetCredits(credits);
            SetDescription(description);
            SetCourseCode(courseCode);
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
    }
}
