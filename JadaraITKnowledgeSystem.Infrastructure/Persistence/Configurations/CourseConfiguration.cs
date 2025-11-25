using JadaraITKnowledgeSystem.Domain.Courses;
using JadaraITKnowledgeSystem.Domain.Courses.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JadaraITKnowledgeSystem.Infrastructure.Persistence.Configurations
{
    public class CourseConfiguration : IEntityTypeConfiguration<Course>
    {
        public void Configure(EntityTypeBuilder<Course> builder)
        {
            builder.ToTable("Courses");
            builder.HasKey(c => c.Id);

            builder.Property(c => c.CourseName)
                .IsRequired()
                .HasMaxLength(120);

            builder.Property(c => c.Description)
                .HasMaxLength(500)
                .IsRequired(false);

            builder.Property(c => c.CourseCode)
                .HasMaxLength(20)
                .IsRequired(false);

            builder.Property(c => c.Credits)
                .IsRequired(false);

            // Auditable fields
            builder.Property(c => c.CreatedAt)
                   .IsRequired();

            builder.Property(c => c.CreatedBy)
                   .HasMaxLength(100)
                   .IsRequired(false);

            builder.Property(c => c.UpdatedAt)
                   .IsRequired(false);

            builder.Property(c => c.UpdatedBy)
                   .HasMaxLength(100)
                   .IsRequired(false);

            // Requirements navigation
            builder.HasMany(c => c.Requirements)
                   .WithOne(r => r.Course)
                   .HasForeignKey(r => r.CourseId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Folders navigation
            builder.HasMany(c => c.Folders)
                   .WithOne(f => f.Course)
                   .HasForeignKey(f => f.CourseId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Materials navigation
            builder.HasMany<CourseMaterial>(nameof(Course.Materials))
                   .WithOne(m => m.Course)
                   .HasForeignKey(m => m.CourseId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Backing field access mode for collections
            builder.Navigation(c => c.Requirements)
                   .UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.Navigation(c => c.Folders)
                   .UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.Navigation(nameof(Course.Materials))
                   .UsePropertyAccessMode(PropertyAccessMode.Field);

            // Indexes / constraints
            builder.HasIndex(c => c.CourseCode)
                   .IsUnique(false); // allow nullable / duplicates unless enforced elsewhere
        }
    }
}