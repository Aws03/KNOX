using JadaraITKnowledgeSystem.Application.Fetures.Courses.Dtos;
using JadaraITKnowledgeSystem.Domain.Courses.Entites;

namespace JadaraITKnowledgeSystem.Application.Fetures.Courses.Mappers;

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

    public static EnrolledCourseSummaryDto ToSummaryDto(
        this Enrollment enrollment,
        int numberOfMaterials,
        int numberOfQuizzes)
    {
        return new EnrolledCourseSummaryDto(
            EnrollmentId: enrollment.Id,
            CourseId: enrollment.CourseId,
            CourseName: enrollment.Course?.CourseName ?? string.Empty,
            CourseCode: enrollment.Course?.CourseCode,
            Description: enrollment.Course?.Description,
            Credits: enrollment.Course?.Credits,
            IsFinished: enrollment.IsFinished,
            FinishedAt: enrollment.FinishedAt,
            EnrolledAt: enrollment.CreatedAt,
            NumberOfMaterials: numberOfMaterials,
            NumberOfQuizzes: numberOfQuizzes
        );
    }
}
