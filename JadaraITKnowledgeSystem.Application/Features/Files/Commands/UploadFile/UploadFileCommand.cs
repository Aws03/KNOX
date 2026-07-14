using JadaraITKnowledgeSystem.Application.Features.Files.Dtos;
using MediatR;

namespace JadaraITKnowledgeSystem.Application.Features.Files.Commands.UploadFile;

public sealed record UploadFileCommand(
    Stream FileStream,
    string OriginalFileName,
    string? Path = null
) : IRequest<UploadedFileDto>;
