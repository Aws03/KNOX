using JadaraITKnowledgeSystem.Domain.Courses;
using JadaraITKnowledgeSystem.Domain.Courses.Entities;
using JadaraITKnowledgeSystem.Domain.Identity;
using JadaraITKnowledgeSystem.Domain.Quizzes;
using JadaraITKnowledgeSystem.Domain.Quizzes.Entities;
using JadaraITKnowledgeSystem.Domain.System.Entities;
using JadaraITKnowledgeSystem.Domain.Universities;
using JadaraITKnowledgeSystem.Domain.Universities.Entities;
using JadaraITKnowledgeSystem.Domain.Users;
using JadaraITKnowledgeSystem.Domain.Users.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace JadaraITKnowledgeSystem.Application.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Quiz> Quizzes { get; }
    DbSet<Question> Questions { get; }
    DbSet<QuizAttempt> QuizAttempts { get; }
    DbSet<Choice> Choices { get; }
    DbSet<User> Users { get; }
    DbSet<UserReaction> UserReactions { get; }
    DbSet<Course> Courses { get; }
    DbSet<CourseRequirementMapping> MajorCourses { get; }
    DbSet<Faculty> Faculties { get; }
    DbSet<Major> Majors { get; }
    DbSet<University> Universities { get; }
    DbSet<CourseMaterial> CourseMaterials { get; }
    DbSet<Folder> Folders { get; }
    DbSet<CourseInfo> CourseInfos { get; }
    DbSet<CourseResource> CourseResources { get; }
    DbSet<RefreshToken> RefreshTokens { get; }
    DbSet<VerificationOTP> VerificationOTPs { get; }
    DbSet<WriterApplication> WriterApplications { get; }
    DbSet<Enrollment> Enrollments { get; }
    DbSet<QuizGenerationJob> QuizGenerationJobs { get; }
    DbSet<SystemSetting> SystemSettings { get; }

    DatabaseFacade Database { get; } // For transactions
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
