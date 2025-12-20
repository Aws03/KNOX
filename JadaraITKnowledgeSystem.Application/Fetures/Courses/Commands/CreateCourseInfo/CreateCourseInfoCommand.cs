using JadaraITKnowledgeSystem.Application.Fetures.Courses.Dtos;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using JadaraITKnowledgeSystem.Domain.Courses.Enums;
using MediatR;

namespace JadaraITKnowledgeSystem.Application.Fetures.Courses.Commands.CreateCourseInfo;

public sealed record CreateCourseInfoCommand(
    int CourseId,
    DifficultyLevel DifficultyLevel,
    string? Description = null,
    string? DemonstrationVideoUrl = null,
    string? DemonstrationVideoTitle = null
) : IRequest<Result<CourseInfoDto>>;
