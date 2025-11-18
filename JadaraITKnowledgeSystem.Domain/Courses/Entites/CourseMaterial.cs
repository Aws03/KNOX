using JadaraITKnowledgeSystem.Domain.Common;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JadaraITKnowledgeSystem.Domain.Courses.Entites
{
    public sealed class CourseMaterial : AuditableEntity
    {
        //[Key]
        //public int CourseMaterialId { get; private set; }

        [Required]
        [MaxLength(250)]
        public string Title { get; private set; }

        [Required]
        [MaxLength(500)]
        public string ContentUrl { get; private set; }

        [ForeignKey(nameof(Course))]
        public int CourseId { get; private set; }

        public Course Course { get; private set; } // read-only navigation property

        [MaxLength(500)]
        public string? Description { get; private set; }

        private CourseMaterial() { }

        private CourseMaterial(string title, string contentUrl, int courseId, string? description = null)
        {
            SetTitle(title);
            SetContentUrl(contentUrl);
            SetCourseId(courseId);
            SetDescription(description);
        }

        public static Result<CourseMaterial> Create(string title, string contentUrl, int courseId, string? description = null)
        {
            return new CourseMaterial(title, contentUrl, courseId, description);
        }

        public void SetTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title is required.", nameof(title));
            if (title.Length > 250)
                throw new ArgumentException("Title cannot exceed 250 characters.", nameof(title));

            Title = title.Trim();
        }

        public void SetContentUrl(string contentUrl)
        {
            if (string.IsNullOrWhiteSpace(contentUrl))
                throw new ArgumentException("Content URL is required.", nameof(contentUrl));
            if (contentUrl.Length > 500)
                throw new ArgumentException("Content URL cannot exceed 500 characters.", nameof(contentUrl));

            ContentUrl = contentUrl.Trim();
        }

        public void SetCourseId(int courseId)
        {
            if (courseId <= 0)
                throw new ArgumentOutOfRangeException(nameof(courseId), "CourseId must be a positive integer.");

            CourseId = courseId;
        }

        public void SetDescription(string? description)
        {
            if (description != null && description.Length > 500)
                throw new ArgumentException("Description cannot exceed 500 characters.", nameof(description));

            Description = description?.Trim();
        }
    }
}
