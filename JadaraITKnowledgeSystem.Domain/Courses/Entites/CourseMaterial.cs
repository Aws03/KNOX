using JadaraITKnowledgeSystem.Domain.Common;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JadaraITKnowledgeSystem.Domain.Courses.Entites
{
    public sealed class CourseMaterial : AuditableEntity
    {
        [Required]
        [MaxLength(250)]
        public string Title { get; private set; }

        [Required]
        [MaxLength(500)]
        public string ContentUrl { get; private set; }

        [ForeignKey(nameof(Course))]
        public int CourseId { get; private set; }
        public Course Course { get; private set; }

        // New: Optional folder assignment
        [ForeignKey(nameof(Folder))]
        public int? FolderId { get; private set; }
        public Folder? Folder { get; private set; }

        [MaxLength(500)]
        public string? Description { get; private set; }

        private CourseMaterial() { }

        private CourseMaterial(string title, string contentUrl, int courseId, int? folderId = null, string? description = null)
        {
            SetTitle(title);
            SetContentUrl(contentUrl);
            SetCourseId(courseId);
            SetFolderId(folderId);
            SetDescription(description);
        }

        public static Result<CourseMaterial> Create(string title, string contentUrl, int courseId, int? folderId = null, string? description = null)
        {
            // Validate title
            if (string.IsNullOrWhiteSpace(title))
                return Error.Validation("CourseMaterial.Title.Required", "Title is required.");

            if (title.Length > 250)
                return Error.Validation("CourseMaterial.Title.TooLong", "Title cannot exceed 250 characters.");

            // Validate contentUrl
            if (string.IsNullOrWhiteSpace(contentUrl))
                return Error.Validation("CourseMaterial.ContentUrl.Required", "Content URL is required.");

            if (contentUrl.Length > 500)
                return Error.Validation("CourseMaterial.ContentUrl.TooLong", "Content URL cannot exceed 500 characters.");

            // Validate courseId
            if (courseId <= 0)
                return Error.Validation("CourseMaterial.CourseId.Invalid", "CourseId must be a positive integer.");

            // Validate folderId if provided
            if (folderId.HasValue && folderId.Value <= 0)
                return Error.Validation("CourseMaterial.FolderId.Invalid", "FolderId must be a positive integer.");

            return new CourseMaterial(title, contentUrl, courseId, folderId, description);
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

        public void SetFolderId(int? folderId)
        {
            if (folderId.HasValue && folderId.Value <= 0)
                throw new ArgumentOutOfRangeException(nameof(folderId), "FolderId must be a positive integer.");

            FolderId = folderId;
        }

        public void SetDescription(string? description)
        {
            if (description != null && description.Length > 500)
                throw new ArgumentException("Description cannot exceed 500 characters.", nameof(description));

            Description = description?.Trim();
        }

        public Result<Success> MoveToFolder(int? folderId)
        {
            if (folderId.HasValue && folderId.Value <= 0)
                return Error.Validation("CourseMaterial.FolderId.Invalid", "FolderId must be a positive integer.");

            SetFolderId(folderId);
            return Result.Success;
        }

        public Result<Success> MoveToRoot()
        {
            SetFolderId(null);
            return Result.Success;
        }

        // Helper to check if material is at root level
        public bool IsAtRoot() => !FolderId.HasValue;
    }
}