using JadaraITKnowledgeSystem.Application.Features.Courses.Dtos;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using JadaraITKnowledgeSystem.Domain.Courses.Enums;
using MediatR;

namespace JadaraITKnowledgeSystem.Application.Features.Courses.Commands.AddCourseResource;

public sealed record AddCourseResourceCommand(
    int CourseId,
    string Title,
    ResourceType Type,
    string Url,
    string? Description = null,
    string? DemonstrationVideoUrl = null
) : IRequest<Result<CourseResourceDto>>;
