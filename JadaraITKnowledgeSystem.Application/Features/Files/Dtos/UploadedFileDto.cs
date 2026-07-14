

namespace JadaraITKnowledgeSystem.Application.Features.Files.Dtos;

public sealed record UploadedFileDto(
    string FileName,
    string Url,
    string? Path
);
