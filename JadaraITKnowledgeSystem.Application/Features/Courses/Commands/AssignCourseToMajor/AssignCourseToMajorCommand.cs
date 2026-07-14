using JadaraITKnowledgeSystem.Application.DTOs;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using JadaraITKnowledgeSystem.Domain.Courses.Enums;
using MediatR;

namespace JadaraITKnowledgeSystem.Application.Features.Courses.Commands.AssignCourseToMajor;

public sealed record AssignCourseToMajorCommand(
    int CourseId,
    int MajorId,
    RequirementType RequirementType,
    RequirementNature RequirementNature
) : IRequest<Result<CourseRequirementMappingDto>>;
