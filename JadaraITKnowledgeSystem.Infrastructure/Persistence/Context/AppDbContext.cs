using JadaraITKnowledgeSystem.Application.Interfaces;
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
using JadaraITKnowledgeSystem.Infrastructure.Identity;
using JadaraITKnowledgeSystem.Infrastructure.Persistence.Conventions;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace JadaraITKnowledgeSystem.Infrastructure.Persistence.Context
{
    public class AppDbContext(DbContextOptions<AppDbContext> options)
        : IdentityDbContext<ApplicationUser, ApplicationRole, int>(options), IApplicationDbContext
    {
        // Domain users; the Identity accounts are ApplicationUsers (IdentityDbContext.Users).
        public new DbSet<User> Users => Set<User>();
        public DbSet<Quiz> Quizzes => Set<Quiz>();
        public DbSet<Question> Questions => Set<Question>();
        public DbSet<QuizAttempt> QuizAttempts => Set<QuizAttempt>();
        public DbSet<Choice> Choices => Set<Choice>();
        public DbSet<UserReaction> UserReactions => Set<UserReaction>();
        public DbSet<Course> Courses => Set<Course>();
        public DbSet<CourseRequirementMapping> MajorCourses => Set<CourseRequirementMapping>();
        public DbSet<Faculty> Faculties => Set<Faculty>();
        public DbSet<Major> Majors => Set<Major>();
        public DbSet<University> Universities => Set<University>();
        public DbSet<CourseMaterial> CourseMaterials => Set<CourseMaterial>();
        public DbSet<Folder> Folders => Set<Folder>();
        public DbSet<CourseInfo> CourseInfos => Set<CourseInfo>();
        public DbSet<CourseResource> CourseResources => Set<CourseResource>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
        public DbSet<VerificationOTP> VerificationOTPs => Set<VerificationOTP>();
        public DbSet<WriterApplication> WriterApplications => Set<WriterApplication>();
        public DbSet<Enrollment> Enrollments => Set<Enrollment>();
        public DbSet<QuizGenerationJob> QuizGenerationJobs => Set<QuizGenerationJob>();
        public DbSet<SystemSetting> SystemSettings => Set<SystemSetting>();

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Properties<DateTime>().HaveConversion<UtcDateTimeConverter>();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}
