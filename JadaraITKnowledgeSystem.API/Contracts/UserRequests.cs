namespace JadaraITKnowledgeSystem.API.Contracts;

public sealed record UpdateProfileRequest(string? FullName = null, int? MajorId = null);
