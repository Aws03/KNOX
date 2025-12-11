using JadaraITKnowledgeSystem.Application.Fetures.Courses.Dtos;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;
using System.Collections.Generic;

namespace JadaraITKnowledgeSystem.Application.Fetures.Courses.Commands.CreateCourseMaterial;

public sealed record CreateCourseMaterialCommand(
    string Title,
    string ContemtUrl,
    int CourseId,
    int? FolderId,
    string? Description,
    IEnumerable<string>? Tags
) : IRequest<Result<CourseMaterialDto>>;
