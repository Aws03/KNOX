using JadaraITKnowledgeSystem.Application.Fetures.Courses.Dtos;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using JadaraITKnowledgeSystem.Domain.Courses.Enums;
using MediatR;

namespace JadaraITKnowledgeSystem.Application.Fetures.Courses.Commands.AddCourseResource;

public sealed record AddCourseResourceCommand(
    int CourseId,
    string Title,
    ResourceType Type,
    string Url,
    string? Description = null,
    string? DemonstrationVideoUrl = null
) : IRequest<Result<CourseResourceDto>>;
