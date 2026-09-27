using JadaraITKnowledgeSystem.Domain.Courses.Enums;

namespace JadaraITKnowledgeSystem.API.Contracts;

public sealed record AssignCourseToMajorRequest(int MajorId, RequirementType RequirementType, RequirementNature RequirementNature);

public sealed record CreateCourseInfoRequest(
    DifficultyLevel DifficultyLevel,
    string? Description = null,
    string? DemonstrationVideoUrl = null,
    string? DemonstrationVideoTitle = null);

public sealed record UpdateCourseInfoRequest(
    DifficultyLevel? DifficultyLevel = null,
    string? Description = null,
    string? DemonstrationVideoUrl = null,
    string? DemonstrationVideoTitle = null);

public sealed record AddCourseResourceRequest(
    string Title,
    ResourceType Type,
    string Url,
    string? Description = null,
    string? DemonstrationVideoUrl = null);

public sealed record UpdateCourseResourceRequest(
    string? Title = null,
    ResourceType? Type = null,
    string? Url = null,
    string? Description = null,
    string? DemonstrationVideoUrl = null);

/// <summary>Creates a material from a finished upload: UploadKey is the key returned by POST api/files/material-uploads.</summary>
public sealed record CreateMaterialRequest(string Title, string UploadKey, int? FolderId, string? Description, List<string>? Tags);

public sealed record CreateFolderRequest(string Name, int? ParentFolderId, string? Description);

public sealed record EnrollCourseRequest(string? Notes = null);
