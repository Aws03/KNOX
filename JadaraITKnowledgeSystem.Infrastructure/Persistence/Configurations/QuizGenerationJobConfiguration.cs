using JadaraITKnowledgeSystem.Domain.Courses;
using JadaraITKnowledgeSystem.Domain.Quizzes.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JadaraITKnowledgeSystem.Infrastructure.Persistence.Configurations;

public class QuizGenerationJobConfiguration : IEntityTypeConfiguration<QuizGenerationJob>
{
    public void Configure(EntityTypeBuilder<QuizGenerationJob> builder)
    {
        builder.ToTable("QuizGenerationJobs");

        builder.HasKey(j => j.Id);

        builder.Property(j => j.MaterialId)
            .IsRequired();

        builder.Property(j => j.CourseId)
            .IsRequired();

        builder.Property(j => j.RequestedByUserId)
            .IsRequired();

        builder.Property(j => j.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(j => j.ErrorMessage)
            .HasMaxLength(1000);

        builder.Property(j => j.GeneratedQuizCount)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(j => j.GeneratedQuizIdsJson)
            .HasMaxLength(500)
            .HasColumnName("GeneratedQuizIds");

        builder.Property(j => j.OptionsJson)
            .IsRequired()
            .HasMaxLength(1000)
            .HasColumnName("Options");

        builder.Property(j => j.CompletedAt);

        // Relationships
        builder.HasOne(j => j.Material)
            .WithMany()
            .HasForeignKey(j => j.MaterialId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(j => j.Course)
            .WithMany()
            .HasForeignKey(j => j.CourseId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(j => j.RequestedByUser)
            .WithMany()
            .HasForeignKey(j => j.RequestedByUserId)
            .OnDelete(DeleteBehavior.NoAction);

        // Indexes
        builder.HasIndex(j => j.MaterialId);
        builder.HasIndex(j => j.Status);
        builder.HasIndex(j => j.CreatedAt);
    }
}
