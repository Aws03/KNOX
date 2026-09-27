namespace JadaraITKnowledgeSystem.API.Contracts;

public sealed record UploadedFileResponse(string FileUrl, string FileName, long FileSize, DateTimeOffset UploadedAt);

public sealed record CreateMaterialUploadRequest(string FileName, long Size);
