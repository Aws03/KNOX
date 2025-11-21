using JadaraITKnowledgeSystem.Application.Fetures.Files.Dtos;
using MediatR;

namespace JadaraITKnowledgeSystem.Application.Fetures.Files.Commands.UploadFile;

public sealed record UploadFileCommand(
    Stream FileStream,
    string OriginalFileName,
    string? Path = null
) : IRequest<UploadedFileDto>;
