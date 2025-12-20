using JadaraITKnowledgeSystem.Domain.Courses.Entites;
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

        // TODO: Grade functionality is temporarily disabled.
        // Universities may have different grading systems (A, A+, B, etc.)
        // This needs to be redesigned to support flexible grading systems.
        // builder.Property(e => e.Grade)
        //        .HasPrecision(5, 2)
        //        .IsRequired(false);

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
