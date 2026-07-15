using JadaraITKnowledgeSystem.Domain.System.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace JadaraITKnowledgeSystem.Infrastructure.Persistence.Configurations;

public class SystemSettingConfiguration : IEntityTypeConfiguration<SystemSetting>
{
    public void Configure(EntityTypeBuilder<SystemSetting> builder)
    {
        builder.ToTable("SystemSettings");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Key)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.Value)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(s => s.Description)
            .HasMaxLength(200);

        builder.Property(s => s.IsEnabled)
            .IsRequired()
            .HasDefaultValue(true);

        builder.HasIndex(s => s.Key)
            .IsUnique();

        // Seed default settings
        builder.HasData(
            new
            {
                Id = 1,
                Key = "Features.QuizGeneration.Enabled",
                Value = "true",
                Description = "Enable AI-powered quiz generation from course materials",
                IsEnabled = true,
                CreatedAt = new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero),
                CreatedBy = "System",
                UpdatedAt = (DateTimeOffset?)null,
                UpdatedBy = (string?)null
            },
            new
            {
                Id = 2,
                Key = "Features.QuizGeneration.MaxConcurrent",
                Value = "3",
                Description = "Maximum concurrent quiz generation jobs",
                IsEnabled = true,
                CreatedAt = new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero),
                CreatedBy = "System",
                UpdatedAt = (DateTimeOffset?)null,
                UpdatedBy = (string?)null
            },
            new
            {
                Id = 3,
                Key = "Features.QuizGeneration.DefaultQuestions",
                Value = "8",
                Description = "Default number of questions per generated quiz",
                IsEnabled = true,
                CreatedAt = new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero),
                CreatedBy = "System",
                UpdatedAt = (DateTimeOffset?)null,
                UpdatedBy = (string?)null
            }
        );
    }
}
