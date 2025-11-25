using JadaraITKnowledgeSystem.Application.DTOs;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;

namespace JadaraITKnowledgeSystem.Application.Fetures.Courses.Commands.CreateCourseMaterial;

public sealed record CreateCourseMaterialCommand(
string Title,
string ContemtUrl,
int CourseId,
int? FolderId,
string? Description
) : IRequest<Result<CourseMaterialDto>>;
