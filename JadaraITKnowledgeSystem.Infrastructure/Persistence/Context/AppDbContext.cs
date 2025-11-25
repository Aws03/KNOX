using JadaraITKnowledgeSystem.Application.Interfaces;
using JadaraITKnowledgeSystem.Domain.Common;
using JadaraITKnowledgeSystem.Domain.Courses;
using JadaraITKnowledgeSystem.Domain.Courses.Entites;
using JadaraITKnowledgeSystem.Domain.Quizzes;
using JadaraITKnowledgeSystem.Domain.Quizzes.Entites;
using JadaraITKnowledgeSystem.Domain.Universities;
using JadaraITKnowledgeSystem.Domain.Universities.Entities;
using JadaraITKnowledgeSystem.Domain.Users;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JadaraITKnowledgeSystem.Infrastructure.Persistence.Context
{
    public class AppDbContext : DbContext , IApplicationDbContext
    {

        public virtual DbSet<Quiz> Quizzes { get; set; }
        public virtual DbSet<Question> Questions { get; set; }
        public virtual DbSet<QuizAttempt> QuizAttempts { get; set; }
        public virtual DbSet<Choice> Choices { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserReaction> UserReactions { get; set; }
        public virtual DbSet<Course> Courses { get; set; }
        // mapping table (major - course) 
        public virtual DbSet<CourseRequirementMapping> MajorCourses { get; set; }
        public virtual DbSet<Faculty> Faculties { get; set; }
        public virtual DbSet<Major> Majors { get; set; }
        public virtual DbSet<University> Universities { get; set; }
        // mapping table (material - course)
        public virtual DbSet<CourseMaterial> CourseMaterials { get; set; }
        public virtual DbSet<Folder> Folders { get; set; }

        public AppDbContext()
        {
        }

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
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await DispatchDomainEventsAsync(cancellationToken);
            return await base.SaveChangesAsync(cancellationToken);
        }

        private async Task DispatchDomainEventsAsync(CancellationToken cancellationToken)
        {
            // Get all entities with domain events
            var domainEntities = ChangeTracker.Entries<BaseEntity>()
                .Where(e => e.Entity.DomainEvents.Any())
                .Select(e => e.Entity)
                .ToList();

            var domainEvents = domainEntities
                .SelectMany(e => e.DomainEvents)
                .ToList();

            domainEntities.ForEach(e => e.ClearDomainEvents());

            // Publish each event
            foreach (var domainEvent in domainEvents)
            {
                await _mediator.Publish(domainEvent, cancellationToken);
            }
        }
    }
}
