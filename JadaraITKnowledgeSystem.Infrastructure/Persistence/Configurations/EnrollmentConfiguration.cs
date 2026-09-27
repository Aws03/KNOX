using JadaraITKnowledgeSystem.Domain.Courses.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JadaraITKnowledgeSystem.Infrastructure.Persistence.Configurations;

internal class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
{
    public void Configure(EntityTypeBuilder<Enrollment> builder)
    {
        builder.ToTable("Enrollments");
        builder.HasKey(e => e.Id);

        // Unique constraint: a user can only enroll once per course
        builder.HasIndex(e => new { e.UserId, e.CourseId })
               .IsUnique();

        builder.Property(e => e.IsFinished)
               .IsRequired()
               .HasDefaultValue(false);

        builder.Property(e => e.FinishedAt)
               .IsRequired(false);

        builder.Property(e => e.Notes)
               .HasMaxLength(500)
               .IsRequired(false);

        // Auditable fields
        builder.Property(e => e.CreatedAt)
               .IsRequired();

        builder.Property(e => e.CreatedBy)
               .HasMaxLength(100)
               .IsRequired(false);

        builder.Property(e => e.UpdatedAt)
               .IsRequired(false);

        builder.Property(e => e.UpdatedBy)
               .HasMaxLength(100)
               .IsRequired(false);

        // Relationships
        builder.HasOne(e => e.User)
               .WithMany()
               .HasForeignKey(e => e.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Course)
               .WithMany()
               .HasForeignKey(e => e.CourseId)
               .OnDelete(DeleteBehavior.Cascade);

        // Indexes for common queries
        builder.HasIndex(e => e.UserId);
        builder.HasIndex(e => e.CourseId);
        builder.HasIndex(e => e.IsFinished);
    }
}
