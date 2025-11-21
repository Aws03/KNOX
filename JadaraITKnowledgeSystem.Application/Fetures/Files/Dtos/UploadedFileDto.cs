

namespace JadaraITKnowledgeSystem.Application.Fetures.Files.Dtos;

public sealed record UploadedFileDto(
    string FileName,
    string Url,
    string? Path
);
