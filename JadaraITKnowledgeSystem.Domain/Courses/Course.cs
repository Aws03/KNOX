using JadaraITKnowledgeSystem.Domain.Common;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using JadaraITKnowledgeSystem.Domain.Courses.Entites;
using JadaraITKnowledgeSystem.Domain.Courses.Entities;
using JadaraITKnowledgeSystem.Domain.Courses.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace JadaraITKnowledgeSystem.Domain.Courses
{
    public sealed class Course : AuditableEntity
    {
        [MaxLength(120)]
        public string CourseName { get; private set; }

        [MaxLength(500)]
        public string? Description { get; private set; }

        [MaxLength(20)]
        public string? CourseCode { get; private set; }

        public int? Credits { get; private set; }

        // One-to-One relationship with CourseInfo
        public CourseInfo? CourseInfo { get; private set; }

        private readonly List<CourseRequirementMapping> _requirements = new();
        public IReadOnlyCollection<CourseRequirementMapping> Requirements => _requirements.AsReadOnly();

        // Folders collection
        private readonly List<Folder> _folders = new();
        public IReadOnlyCollection<Folder> Folders => _folders.AsReadOnly();

        // Materials collection
        private readonly List<CourseMaterial> _materials = new();
        public IReadOnlyCollection<CourseMaterial> Materials => _materials.AsReadOnly();

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
            if (description != null && description.Length > 500)
                throw new ArgumentException("Description must be 500 characters or fewer.", nameof(Description));

            Description = description;
        }

        public void ChangeDescription(string? newDescription)
        {
            if (string.IsNullOrWhiteSpace(newDescription))
                newDescription = null;

            SetDescription(newDescription);
        }

        private void SetCourseCode(string? courseCode)
        {
            if (courseCode != null && courseCode.Length > 20)
                throw new ArgumentException("Course code must be 20 characters or fewer.", nameof(CourseCode));
            CourseCode = courseCode;
        }

        public void ChangeCourseCode(string? newCourseCode)
        {
            if (string.IsNullOrWhiteSpace(newCourseCode))
                newCourseCode = null;
            SetCourseCode(newCourseCode);
        }

        private void SetCredits(int? credits)
        {
            if (credits != null && (credits < 0 || credits > 10))
                throw new ArgumentException("Credits must be between 0 and 10.", nameof(Credits));
            Credits = credits;
        }

        public void ChangeCredits(int? newCredits)
        {
            SetCredits(newCredits);
        }

        // ============ CourseInfo Management ============

        public Result<CourseInfo> SetCourseInfo(
            DifficultyLevel difficultyLevel,
            string? description = null,
            string? demonstrationVideoUrl = null,
            string? demonstrationVideoTitle = null)
        {
            // Validate that Course has been persisted
            if (this.Id <= 0)
                return Error.Validation("Course.NotPersisted", "Course must be saved before adding course info.");

            // Check if CourseInfo already exists
            if (CourseInfo != null)
                return Error.Conflict("CourseInfo.AlreadyExists", "Course already has course info. Use update methods instead.");

            // Create CourseInfo
            var courseInfoResult = Entities.CourseInfo.Create(
                this.Id,
                difficultyLevel,
                description,
                demonstrationVideoUrl,
                demonstrationVideoTitle);

            if (courseInfoResult.IsError)
                return courseInfoResult.Errors;

            CourseInfo = courseInfoResult.Value;
            return courseInfoResult.Value;
        }

        public Result<Success> UpdateCourseInfo(
            DifficultyLevel? difficultyLevel = null,
            string? description = null,
            string? demonstrationVideoUrl = null,
            string? demonstrationVideoTitle = null)
        {
            if (CourseInfo == null)
                return Error.NotFound("CourseInfo.NotFound", "Course info does not exist. Create it first.");

            // Update difficulty level if provided
            if (difficultyLevel.HasValue)
                CourseInfo.UpdateDifficultyLevel(difficultyLevel.Value);

            // Update description if provided
            if (description != null)
            {
                var descResult = CourseInfo.UpdateDescription(description);
                if (descResult.IsError)
                    return descResult;
            }

            // Update demonstration video if provided
            if (demonstrationVideoUrl != null || demonstrationVideoTitle != null)
            {
                var videoResult = CourseInfo.SetDemonstrationVideo(demonstrationVideoUrl, demonstrationVideoTitle);
                if (videoResult.IsError)
                    return videoResult;
            }

            return Result.Success;
        }

        public Result<Success> RemoveCourseInfo()
        {
            if (CourseInfo == null)
                return Error.NotFound("CourseInfo.NotFound", "Course info does not exist.");

            // Check if there are resources
            if (CourseInfo.HasResources())
                return Error.Validation("CourseInfo.HasResources", "Cannot remove course info with existing resources. Remove resources first.");

            CourseInfo = null;
            return Result.Success;
        }

        public bool HasCourseInfo() => CourseInfo != null;

        // ============ Major Assignment ============

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

        public bool IsAssignedToMajor(int majorId) => _requirements.Any(r => r.MajorId == majorId);

        // ============ Folder Management Methods ============

        public Result<Folder> AddFolder(string folderName, int? parentFolderId = null, string? description = null)
        {
            // Check if a folder with the same name already exists at the same level
            var existingFolder = parentFolderId.HasValue
                ? _folders.FirstOrDefault(f => f.Name.Equals(folderName, StringComparison.OrdinalIgnoreCase)
                                              && f.ParentFolderId == parentFolderId)
                : _folders.FirstOrDefault(f => f.Name.Equals(folderName, StringComparison.OrdinalIgnoreCase)
                                              && !f.ParentFolderId.HasValue);

            if (existingFolder != null)
                return Error.Conflict("Folder.DuplicateName", "A folder with the same name already exists at this level.");

            // If parent folder is specified, validate it exists and belongs to this course
            if (parentFolderId.HasValue)
            {
                var parentFolder = _folders.FirstOrDefault(f => f.Id == parentFolderId.Value);
                if (parentFolder == null)
                    return Error.NotFound("Folder.ParentNotFound", "The specified parent folder does not exist.");
            }

            var folderResult = Folder.Create(folderName, this.Id, parentFolderId, description);
            if (folderResult.IsError)
                return folderResult;

            _folders.Add(folderResult.Value);
            return folderResult;
        }

        public Result<Success> RemoveFolder(int folderId, bool deleteContents = false)
        {
            var folder = _folders.FirstOrDefault(f => f.Id == folderId);
            if (folder == null)
                return Error.NotFound("Folder.NotFound", "The specified folder does not exist.");

            // Check if folder has sub-folders
            var hasSubFolders = _folders.Any(f => f.ParentFolderId == folderId);
            if (hasSubFolders && !deleteContents)
                return Error.Validation("Folder.HasSubFolders", "Cannot delete folder with sub-folders. Set deleteContents to true to delete all contents.");

            // Check if folder has materials
            var hasMaterials = _materials.Any(m => m.FolderId == folderId);
            if (hasMaterials && !deleteContents)
                return Error.Validation("Folder.HasMaterials", "Cannot delete folder with materials. Set deleteContents to true to delete all contents.");

            if (deleteContents)
            {
                // Remove all sub-folders recursively
                RemoveSubFoldersRecursively(folderId);

                // Move materials to root or remove them
                var materialsInFolder = _materials.Where(m => m.FolderId == folderId).ToList();
                foreach (var material in materialsInFolder)
                {
                    material.MoveToRoot();
                }
            }

            _folders.Remove(folder);
            return Result.Success;
        }

        private void RemoveSubFoldersRecursively(int parentFolderId)
        {
            var subFolders = _folders.Where(f => f.ParentFolderId == parentFolderId).ToList();
            foreach (var subFolder in subFolders)
            {
                RemoveSubFoldersRecursively(subFolder.Id);
                _folders.Remove(subFolder);
            }
        }

        public Result<Folder> GetFolder(int folderId)
        {
            var folder = _folders.FirstOrDefault(f => f.Id == folderId);
            if (folder == null)
                return Error.NotFound("Folder.NotFound", "The specified folder does not exist.");

            return folder;
        }

        public IEnumerable<Folder> GetRootFolders()
        {
            return _folders.Where(f => !f.ParentFolderId.HasValue);
        }

        public IEnumerable<Folder> GetSubFolders(int parentFolderId)
        {
            return _folders.Where(f => f.ParentFolderId == parentFolderId);
        }

        // Validate folder hierarchy (prevent circular references)
        public Result<Success> ValidateFolderHierarchy(int folderId, int? newParentFolderId)
        {
            if (!newParentFolderId.HasValue)
                return Result.Success;

            // Check if new parent is a descendant of the folder
            if (IsFolderDescendant(newParentFolderId.Value, folderId))
                return Error.Validation("Folder.CircularReference", "Cannot move folder to its own descendant.");

            return Result.Success;
        }

        private bool IsFolderDescendant(int potentialDescendantId, int ancestorId)
        {
            var folder = _folders.FirstOrDefault(f => f.Id == potentialDescendantId);
            while (folder != null && folder.ParentFolderId.HasValue)
            {
                if (folder.ParentFolderId.Value == ancestorId)
                    return true;

                folder = _folders.FirstOrDefault(f => f.Id == folder.ParentFolderId.Value);
            }
            return false;
        }
    }
}