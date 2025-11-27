using JadaraITKnowledgeSystem.Application.Fetures.Courses.Dtos;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace JadaraITKnowledgeSystem.Application.Fetures.Courses.Commands.CreateFolder
{
    public sealed record CreateFolderCommand(
        string Name,
        int CourseId,
        int? ParentFolderId,
        string? Description
        ) : IRequest<Result<FolderDto>>;

}
