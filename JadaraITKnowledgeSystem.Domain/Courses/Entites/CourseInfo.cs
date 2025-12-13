using JadaraITKnowledgeSystem.Domain.Common;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using JadaraITKnowledgeSystem.Domain.Courses.Enums;
using System.ComponentModel.DataAnnotations;

namespace JadaraITKnowledgeSystem.Domain.Courses.Entities
{
    /// <summary>
    /// Represents detailed course information displayed on the course page
    /// </summary>
    public sealed class CourseInfo : AuditableEntity
    {
        private const int MaxDescriptionLength = 2000;
        private const int MaxVideoUrlLength = 1000;
        private const int MaxVideoTitleLength = 200;

        public int CourseId { get; private set; }
        public Course Course { get; private set; }

        public DifficultyLevel DifficultyLevel { get; private set; }

        [MaxLength(MaxDescriptionLength)]
        public string? Description { get; private set; }

        [MaxLength(MaxVideoUrlLength)]
        public string? DemonstrationVideoUrl { get; private set; }

        [MaxLength(MaxVideoTitleLength)]
        public string? DemonstrationVideoTitle { get; private set; }

        private readonly List<CourseResource> _resources = new();
        public IReadOnlyCollection<CourseResource> Resources => _resources.AsReadOnly();

        private CourseInfo() { }

        private CourseInfo(
            int courseId,
            DifficultyLevel difficultyLevel,
            string? description = null,
            string? demonstrationVideoUrl = null,
            string? demonstrationVideoTitle = null)
        {
            CourseId = courseId;
            DifficultyLevel = difficultyLevel;
            Description = description?.Trim();
            DemonstrationVideoUrl = demonstrationVideoUrl?.Trim();
            DemonstrationVideoTitle = demonstrationVideoTitle?.Trim();
        }

        public static Result<CourseInfo> Create(
            int courseId,
            DifficultyLevel difficultyLevel,
            string? description = null,
            string? demonstrationVideoUrl = null,
            string? demonstrationVideoTitle = null)
        {
            // Validate courseId
            if (courseId <= 0)
                return Error.Validation("CourseId.Invalid", "CourseId must be positive.");

            // Validate description length
            if (!string.IsNullOrWhiteSpace(description) && description.Length > MaxDescriptionLength)
                return Error.Validation("Description.TooLong", $"Description cannot exceed {MaxDescriptionLength} characters.");

            // Validate demonstration video URL
            if (!string.IsNullOrWhiteSpace(demonstrationVideoUrl))
            {
                if (demonstrationVideoUrl.Length > MaxVideoUrlLength)
                    return Error.Validation("DemonstrationVideo.UrlTooLong", $"Demonstration video URL cannot exceed {MaxVideoUrlLength} characters.");

                if (!Uri.TryCreate(demonstrationVideoUrl, UriKind.Absolute, out _))
                    return Error.Validation("DemonstrationVideo.InvalidUrl", "Demonstration video URL is invalid.");
            }

            // Validate demonstration video title
            if (!string.IsNullOrWhiteSpace(demonstrationVideoTitle) && demonstrationVideoTitle.Length > MaxVideoTitleLength)
                return Error.Validation("DemonstrationVideo.TitleTooLong", $"Demonstration video title cannot exceed {MaxVideoTitleLength} characters.");

            return new CourseInfo(
                courseId,
                difficultyLevel,
                description,
                demonstrationVideoUrl,
                demonstrationVideoTitle);
        }

        // ============ Update Methods ============

        public void UpdateDifficultyLevel(DifficultyLevel level)
        {
            DifficultyLevel = level;
        }

        public Result<Success> UpdateDescription(string? description)
        {
            if (!string.IsNullOrWhiteSpace(description))
            {
                if (description.Length > MaxDescriptionLength)
                    return Error.Validation("Description.TooLong", $"Description cannot exceed {MaxDescriptionLength} characters.");

                Description = description.Trim();
            }
            else
            {
                Description = null;
            }

            return Result.Success;
        }

        public Result<Success> SetDemonstrationVideo(
            string? videoUrl,
            string? videoTitle)
        {
            // Validate URL if provided
            if (!string.IsNullOrWhiteSpace(videoUrl))
            {
                if (videoUrl.Length > MaxVideoUrlLength)
                    return Error.Validation("Video.UrlTooLong", $"Video URL cannot exceed {MaxVideoUrlLength} characters.");

                if (!Uri.TryCreate(videoUrl, UriKind.Absolute, out _))
                    return Error.Validation("Video.InvalidUrl", "Video URL is invalid.");
            }

            // Validate title if provided
            if (!string.IsNullOrWhiteSpace(videoTitle))
            {
                if (videoTitle.Length > MaxVideoTitleLength)
                    return Error.Validation("Video.TitleTooLong", $"Video title cannot exceed {MaxVideoTitleLength} characters.");
            }

            DemonstrationVideoUrl = videoUrl?.Trim();
            DemonstrationVideoTitle = videoTitle?.Trim();

            return Result.Success;
        }

        public void RemoveDemonstrationVideo()
        {
            DemonstrationVideoUrl = null;
            DemonstrationVideoTitle = null;
        }

        // ============ Resource Management ============

        public Result<CourseResource> AddResource(
            string title,
            ResourceType type,
            string url,
            string? description = null,
            string? demonstrationVideoUrl = null)
        {
            // Validate that CourseInfo has been persisted
            if (this.Id <= 0)
                return Error.Validation("CourseInfo.NotPersisted", "CourseInfo must be saved before adding resources.");

            // Check for duplicate URLs (case-insensitive)
            if (_resources.Any(r => r.Url.Equals(url, StringComparison.OrdinalIgnoreCase)))
                return Error.Conflict("Resource.Duplicate", "A resource with this URL already exists.");

            // Create the resource
            var resourceResult = CourseResource.Create(
                this.Id,
                title,
                type,
                url,
                description,
                demonstrationVideoUrl);

            if (resourceResult.IsError)
                return resourceResult.Errors;

            _resources.Add(resourceResult.Value);
            return resourceResult.Value;
        }

        public Result<Success> RemoveResource(int resourceId)
        {
            var resource = _resources.FirstOrDefault(r => r.Id == resourceId);
            if (resource == null)
                return Error.NotFound("Resource.NotFound", "Resource not found.");

            _resources.Remove(resource);
            return Result.Success;
        }

        public Result<CourseResource> GetResource(int resourceId)
        {
            var resource = _resources.FirstOrDefault(r => r.Id == resourceId);
            if (resource == null)
                return Error.NotFound("Resource.NotFound", "Resource not found.");

            return resource;
        }

        public IEnumerable<CourseResource> GetResourcesByType(ResourceType type)
        {
            return _resources.Where(r => r.Type == type).OrderBy(r => r.CreatedAt);
        }

        public IEnumerable<CourseResource> GetAllResources()
        {
            return _resources.OrderBy(r => r.CreatedAt);
        }

        public bool HasResources() => _resources.Count > 0;

        public int ResourceCount => _resources.Count;
    }
}