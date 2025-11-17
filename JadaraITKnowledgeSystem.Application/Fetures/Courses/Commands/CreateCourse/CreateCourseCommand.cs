using JadaraITKnowledgeSystem.Application.Fetures.Courses.Dtos;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using JadaraITKnowledgeSystem.Domain.Courses.Enums;
using MediatR;

namespace JadaraITKnowledgeSystem.Application.Fetures.Courses.Commands.CreateCourse
{
    public sealed record CreateCourseCommand(
        int MajorId,
        string CourseName,
        string? CourseCode,
        string? Description,
        RequirementType RequirementType,
        RequirementNature RequirementNature,
        int? Credits
    ) : IRequest<Result<CourseDto>>; // returns courseId
}
