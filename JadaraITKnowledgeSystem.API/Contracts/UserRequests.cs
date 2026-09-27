namespace JadaraITKnowledgeSystem.API.Contracts;

public sealed record UpdateProfileRequest(string? FullName = null, int? MajorId = null);
public sealed record ToggleFeatureRequest(bool Enabled);
public sealed record FeatureStatusResponse(bool Enabled);

public sealed record UploadedFileResponse(string FileUrl, string FileName, long FileSize, DateTimeOffset UploadedAt);
