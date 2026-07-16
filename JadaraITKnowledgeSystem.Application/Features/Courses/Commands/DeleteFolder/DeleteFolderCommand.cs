using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;

namespace JadaraITKnowledgeSystem.Application.Features.Courses.Commands.DeleteFolder;

public sealed record DeleteFolderCommand(int FolderId, bool DeleteContents = false) : IRequest<Result<Success>>;
