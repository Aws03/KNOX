using JadaraITKnowledgeSystem.Domain.Courses;
using JadaraITKnowledgeSystem.Domain.Courses.Entites;
using JadaraITKnowledgeSystem.Domain.Courses.Entities;
using JadaraITKnowledgeSystem.Domain.Identity;
using JadaraITKnowledgeSystem.Domain.Quizzes;
using JadaraITKnowledgeSystem.Domain.Quizzes.Entites;
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
    public DbSet<Quiz> Quizzes { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<QuizAttempt> QuizAttempts { get; set; }
    public DbSet<Choice> Choices { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserReaction> UserReactions { get; set; }
    public DbSet<Course> Courses { get; set; }
    // mapping table (major - course) 
    public DbSet<CourseRequirementMapping> MajorCourses { get; set; }
    public DbSet<Faculty> Faculties { get; set; }
    public DbSet<Major> Majors { get; set; }
    public DbSet<University> Universities { get; set; }
    // mapping table (material - course)
    public DbSet<CourseMaterial> CourseMaterials { get; set; }
    // hierarchical folders
    public DbSet<Folder> Folders { get; set; }
    // course detailed information
    public DbSet<CourseInfo> CourseInfos { get; set; }
    public DbSet<CourseResource> CourseResources { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<VerificationOTP> VerificationOTPs { get; set; }
    public DbSet<WriterApplication> WriterApplications { get; set; }
    public DbSet<Enrollment> Enrollments { get; set; }
    public DbSet<QuizGenerationJob> QuizGenerationJobs { get; set; }
    public DbSet<SystemSetting> SystemSettings { get; set; }

    DatabaseFacade Database { get; } // For transactions
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
