using JadaraITKnowledgeSystem.Application.Features.Courses.Dtos;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using JadaraITKnowledgeSystem.Domain.Courses.Enums;
using MediatR;

namespace JadaraITKnowledgeSystem.Application.Features.Courses.Commands.UpdateCourseInfo;

public sealed record UpdateCourseInfoCommand(
    int CourseId,
    DifficultyLevel? DifficultyLevel = null,
    string? Description = null,
    string? DemonstrationVideoUrl = null,
    string? DemonstrationVideoTitle = null
) : IRequest<Result<CourseInfoDto>>;
