using JadaraITKnowledgeSystem.Application.Interfaces;
using JadaraITKnowledgeSystem.Domain.Common;
using JadaraITKnowledgeSystem.Domain.Courses;
using JadaraITKnowledgeSystem.Domain.Courses.Entites;
using JadaraITKnowledgeSystem.Domain.Courses.Entities;
using JadaraITKnowledgeSystem.Domain.Identity;
using JadaraITKnowledgeSystem.Domain.Quizzes;
using JadaraITKnowledgeSystem.Domain.Quizzes.Entites;
using JadaraITKnowledgeSystem.Domain.Universities;
using JadaraITKnowledgeSystem.Domain.Universities.Entities;
using JadaraITKnowledgeSystem.Domain.Users;
using JadaraITKnowledgeSystem.Domain.Users.Entities;
using JadaraITKnowledgeSystem.Infrastructure.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace JadaraITKnowledgeSystem.Infrastructure.Persistence.Context
{
    public class AppDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int>, IApplicationDbContext
    {
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Quiz> Quizzes { get; set; }
        public virtual DbSet<Question> Questions { get; set; }
        public virtual DbSet<QuizAttempt> QuizAttempts { get; set; }
        public virtual DbSet<Choice> Choices { get; set; }
        public virtual DbSet<UserReaction> UserReactions { get; set; }
        public virtual DbSet<Course> Courses { get; set; }
        public virtual DbSet<CourseRequirementMapping> MajorCourses { get; set; }
        public virtual DbSet<Faculty> Faculties { get; set; }
        public virtual DbSet<Major> Majors { get; set; }
        public virtual DbSet<University> Universities { get; set; }
        public virtual DbSet<CourseMaterial> CourseMaterials { get; set; }
        public virtual DbSet<Folder> Folders { get; set; }
        public virtual DbSet<CourseInfo> CourseInfos { get; set; }
        public virtual DbSet<CourseResource> CourseResources { get; set; }
        public virtual DbSet<RefreshToken> RefreshTokens { get; set; }
        public virtual DbSet<VerificationOTP> VerificationOTPs { get; set; }

        private readonly IMediator _mediator;

        public AppDbContext(DbContextOptions<AppDbContext> options, IMediator mediator)
            : base(options)
        {
            _mediator = mediator;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            modelBuilder.Entity<ApplicationUser>()
                .HasOne(au => au.DomainUser)
                .WithOne()
                .HasForeignKey<ApplicationUser>(au => au.DomainUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RefreshToken>()
                .HasIndex(rt => rt.Token)
                .IsUnique();

            modelBuilder.Entity<RefreshToken>()
                .Property(rt => rt.Token)
                .HasMaxLength(256)
                .IsRequired();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await DispatchDomainEventsAsync(cancellationToken);
            return await base.SaveChangesAsync(cancellationToken);
        }

        private async Task DispatchDomainEventsAsync(CancellationToken cancellationToken)
        {
            var domainEntities = ChangeTracker.Entries<BaseEntity>()
                .Where(e => e.Entity.DomainEvents.Any())
                .Select(e => e.Entity)
                .ToList();

            var domainEvents = domainEntities
                .SelectMany(e => e.DomainEvents)
                .ToList();

            domainEntities.ForEach(e => e.ClearDomainEvents());

            foreach (var domainEvent in domainEvents)
            {
                await _mediator.Publish(domainEvent, cancellationToken);
            }
        }
    }
}
