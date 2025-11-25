using JadaraITKnowledgeSystem.Domain.Common;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace JadaraITKnowledgeSystem.Domain.Courses.Entites
{
    public sealed class Folder : AuditableEntity
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; private set; }

        [ForeignKey(nameof(Course))]
        public int CourseId { get; private set; }
        public Course Course { get; private set; }

        [ForeignKey(nameof(ParentFolder))]
        public int? ParentFolderId { get; private set; }
        public Folder? ParentFolder { get; private set; }

        [MaxLength(500)]
        public string? Description { get; private set; }

        // Child folders
        private readonly List<Folder> _subFolders = new();
        public IReadOnlyCollection<Folder> SubFolders => _subFolders.AsReadOnly();

        // Materials in this folder
        private readonly List<CourseMaterial> _materials = new();
        public IReadOnlyCollection<CourseMaterial> Materials => _materials.AsReadOnly();

        private Folder() { }

        private Folder(string name, int courseId, int? parentFolderId = null, string? description = null)
        {
            SetName(name);
            SetCourseId(courseId);
            SetParentFolderId(parentFolderId);
            SetDescription(description);
        }

        public static Result<Folder> Create(string name, int courseId, int? parentFolderId = null, string? description = null)
        {
            // Validate name
            if (string.IsNullOrWhiteSpace(name))
                return Error.Validation("Folder.Name.Required", "Folder name is required.");

            if (name.Length > 200)
                return Error.Validation("Folder.Name.TooLong", "Folder name cannot exceed 200 characters.");

            // Validate courseId
            if (courseId <= 0)
                return Error.Validation("Folder.CourseId.Invalid", "CourseId must be a positive integer.");

            // Validate parentFolderId if provided
            if (parentFolderId.HasValue && parentFolderId.Value <= 0)
                return Error.Validation("Folder.ParentFolderId.Invalid", "ParentFolderId must be a positive integer.");

            return new Folder(name, courseId, parentFolderId, description);
        }

        public void SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Folder name is required.", nameof(name));

            if (name.Length > 200)
                throw new ArgumentException("Folder name cannot exceed 200 characters.", nameof(name));

            Name = name.Trim();
        }

        public void SetCourseId(int courseId)
        {
            if (courseId <= 0)
                throw new ArgumentOutOfRangeException(nameof(courseId), "CourseId must be a positive integer.");

            CourseId = courseId;
        }

        public void SetParentFolderId(int? parentFolderId)
        {
            if (parentFolderId.HasValue && parentFolderId.Value <= 0)
                throw new ArgumentOutOfRangeException(nameof(parentFolderId), "ParentFolderId must be a positive integer.");

            // Prevent self-referencing
            if (parentFolderId.HasValue && parentFolderId.Value == this.Id)
                throw new ArgumentException("A folder cannot be its own parent.", nameof(parentFolderId));

            ParentFolderId = parentFolderId;
        }

        public void SetDescription(string? description)
        {
            if (description != null && description.Length > 500)
                throw new ArgumentException("Description cannot exceed 500 characters.", nameof(description));

            Description = description?.Trim();
        }

        public Result<Success> ChangeName(string newName)
        {
            if (string.IsNullOrWhiteSpace(newName))
                return Error.Validation("Folder.Name.Required", "Folder name is required.");

            if (newName.Length > 200)
                return Error.Validation("Folder.Name.TooLong", "Folder name cannot exceed 200 characters.");

            SetName(newName);
            return Result.Success;
        }

        public Result<Success> ChangeDescription(string? newDescription)
        {
            if (string.IsNullOrWhiteSpace(newDescription))
                newDescription = null;

            SetDescription(newDescription);
            return Result.Success;
        }

        public Result<Success> MoveToFolder(int? newParentFolderId)
        {
            if (newParentFolderId.HasValue && newParentFolderId.Value <= 0)
                return Error.Validation("Folder.ParentFolderId.Invalid", "ParentFolderId must be a positive integer.");

            // Prevent self-referencing
            if (newParentFolderId.HasValue && newParentFolderId.Value == this.Id)
                return Error.Validation("Folder.SelfReference", "A folder cannot be its own parent.");

            SetParentFolderId(newParentFolderId);
            return Result.Success;
        }

        // Helper to check if this folder is at root level
        public bool IsRootFolder() => !ParentFolderId.HasValue;

        // Helper to get full path (for display purposes)
        public string GetPath(List<Folder> allFolders)
        {
            if (IsRootFolder())
                return Name;

            var path = new List<string> { Name };
            var currentFolderId = ParentFolderId;

            while (currentFolderId.HasValue)
            {
                var parent = allFolders.FirstOrDefault(f => f.Id == currentFolderId.Value);
                if (parent == null) break;

                path.Insert(0, parent.Name);
                currentFolderId = parent.ParentFolderId;
            }

            return string.Join("/", path);
        }
    }
}