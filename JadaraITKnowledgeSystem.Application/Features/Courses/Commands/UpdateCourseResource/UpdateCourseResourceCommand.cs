using JadaraITKnowledgeSystem.Application.Features.Courses.Dtos;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using JadaraITKnowledgeSystem.Domain.Courses.Enums;
using MediatR;

namespace JadaraITKnowledgeSystem.Application.Features.Courses.Commands.UpdateCourseResource;

public sealed record UpdateCourseResourceCommand(
    int CourseId,
    int ResourceId,
    string? Title = null,
    ResourceType? Type = null,
    string? Url = null,
    string? Description = null,
    string? DemonstrationVideoUrl = null
) : IRequest<Result<CourseResourceDto>>;
