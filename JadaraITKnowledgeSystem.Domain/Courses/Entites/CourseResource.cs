using JadaraITKnowledgeSystem.Domain.Common;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using JadaraITKnowledgeSystem.Domain.Courses.Enums;
using System.ComponentModel.DataAnnotations;

namespace JadaraITKnowledgeSystem.Domain.Courses.Entities
{
    /// <summary>
    /// Represents a learning resource (course, video, article, etc.) associated with a course
    /// </summary>
    public sealed class CourseResource : AuditableEntity
    {
        public int CourseInfoId { get; private set; }

        [MaxLength(200)]
        public string Title { get; private set; }

        public ResourceType Type { get; private set; }

        // Main resource URL (course URL, video URL, article URL, etc.)
        [MaxLength(1000)]
        public string Url { get; private set; }

        [MaxLength(1000)]
        public string? Description { get; private set; }

        // Optional demonstration video (to explain about the resource)
        [MaxLength(1000)]
        public string? DemonstrationVideoUrl { get; private set; }

        private CourseResource() { }

        private CourseResource(
            int courseInfoId,
            string title,
            ResourceType type,
            string url,
            string? description = null,
            string? thumbnailVideoUrl = null)
        {
            CourseInfoId = courseInfoId;
            Title = title;
            Type = type;
            Url = url;
            Description = description;
            DemonstrationVideoUrl = thumbnailVideoUrl;
        }

        public static Result<CourseResource> Create(
            int courseInfoId,
            string title,
            ResourceType type,
            string url,
            string? description = null,
            string? demonstrationVideoUrl = null)
        {
            if (courseInfoId <= 0)
                return Error.Validation("CourseInfoId.Invalid", "CourseInfoId must be positive.");

            if (string.IsNullOrWhiteSpace(title))
                return Error.Validation("Title.Required", "Resource title is required.");

            if (string.IsNullOrWhiteSpace(url))
                return Error.Validation("Url.Required", "Resource URL is required.");

            if (!Uri.TryCreate(url, UriKind.Absolute, out _))
                return Error.Validation("Url.Invalid", "URL format is invalid.");

            // Validate demonstration video if provided
            if (!string.IsNullOrWhiteSpace(demonstrationVideoUrl))
                if (!Uri.TryCreate(demonstrationVideoUrl, UriKind.Absolute, out _))
                    return Error.Validation("DemonstrationVideo.InvalidUrl", "Demonstration video URL is invalid.");

               

            return new CourseResource(
                courseInfoId,
                title,
                type,
                url,
                description,
                demonstrationVideoUrl);
        }

        // ============ Update Methods ============

        public void UpdateTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be empty.", nameof(title));

            Title = title;
        }

        public Result<Success> UpdateUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return Error.Validation("Url.Required", "URL cannot be empty.");

            if (!Uri.TryCreate(url, UriKind.Absolute, out _))
                return Error.Validation("Url.Invalid", "URL format is invalid.");

            Url = url;
            return Result.Success;
        }

        public void UpdateDescription(string? description) => Description = description;

        public void UpdateType(ResourceType type) => Type = type;

        public Result<Success> SetDemonstrationVideo(
            string? videoUrl)
        {
            if (!string.IsNullOrWhiteSpace(videoUrl))
                if (!Uri.TryCreate(videoUrl, UriKind.Absolute, out _))
                    return Error.Validation("Video.InvalidUrl", "Video URL is invalid.");


            DemonstrationVideoUrl = videoUrl;
            
            return Result.Success;
        }

        public void RemoveDemonstrationVideo()
        {
            DemonstrationVideoUrl = null;
        }
        public bool HasDemonstrationVideo() => !string.IsNullOrWhiteSpace(DemonstrationVideoUrl);
    }
}