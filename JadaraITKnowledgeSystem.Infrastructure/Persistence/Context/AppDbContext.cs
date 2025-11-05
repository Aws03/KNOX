using JadaraITKnowledgeSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JadaraITKnowledgeSystem.Infrastructure.Persistence.Context
{
    public class AppDbContext : DbContext
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

        public AppDbContext()
        {
        }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }


    }
}
