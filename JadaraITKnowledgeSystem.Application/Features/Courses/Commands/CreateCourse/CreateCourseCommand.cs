using JadaraITKnowledgeSystem.Application.Features.Courses.Dtos;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using JadaraITKnowledgeSystem.Domain.Courses.Enums;
using MediatR;

namespace JadaraITKnowledgeSystem.Application.Features.Courses.Commands.CreateCourse;

public sealed record CreateCourseCommand(
    int MajorId,
    string CourseName,
    string? CourseCode,
    string? Description,
    RequirementType RequirementType,
    RequirementNature RequirementNature,
    int? Credits
) : IRequest<Result<CourseDto>>; // returns courseId
