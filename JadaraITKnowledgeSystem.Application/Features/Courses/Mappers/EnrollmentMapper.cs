using JadaraITKnowledgeSystem.Application.Features.Courses.Dtos;
using JadaraITKnowledgeSystem.Domain.Courses.Entities;

namespace JadaraITKnowledgeSystem.Application.Features.Courses.Mappers;

public static class EnrollmentMapper
{
    public static EnrollmentDto ToDto(this Enrollment enrollment)
    {
        return new EnrollmentDto(
            Id: enrollment.Id,
            UserId: enrollment.UserId,
            CourseId: enrollment.CourseId,
            CourseName: enrollment.Course?.CourseName ?? string.Empty,
            CourseCode: enrollment.Course?.CourseCode,
            IsFinished: enrollment.IsFinished,
            FinishedAt: enrollment.FinishedAt,
            Notes: enrollment.Notes,
            EnrolledAt: enrollment.CreatedAt
        );
    }
}
