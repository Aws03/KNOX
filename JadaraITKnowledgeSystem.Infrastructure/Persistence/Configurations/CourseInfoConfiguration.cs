using JadaraITKnowledgeSystem.Domain.Courses;
using JadaraITKnowledgeSystem.Domain.Courses.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JadaraITKnowledgeSystem.Infrastructure.Persistence.Configurations
{
    public class CourseInfoConfiguration : IEntityTypeConfiguration<CourseInfo>
    {
        public void Configure(EntityTypeBuilder<CourseInfo> builder)
        {
            builder.ToTable("CourseInfos");
            builder.HasKey(ci => ci.Id);

            // Properties
            builder.Property(ci => ci.CourseId)
                .IsRequired();

            builder.Property(ci => ci.DifficultyLevel)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(ci => ci.Description)
                .HasMaxLength(2000)
                .IsRequired(false);

            builder.Property(ci => ci.DemonstrationVideoUrl)
                .HasMaxLength(1000)
                .IsRequired(false);

            builder.Property(ci => ci.DemonstrationVideoTitle)
                .HasMaxLength(200)
                .IsRequired(false);

            // Auditable fields
            builder.Property(ci => ci.CreatedAt)
                .IsRequired();

            builder.Property(ci => ci.CreatedBy)
                .HasMaxLength(100)
                .IsRequired(false);

            builder.Property(ci => ci.UpdatedAt)
                .IsRequired(false);

            builder.Property(ci => ci.UpdatedBy)
                .HasMaxLength(100)
                .IsRequired(false);

            // Relationships
            // Note: One-to-One with Course is configured in CourseConfiguration
            // to avoid circular configuration

            // Index for CourseId (ensures one-to-one)
            builder.HasIndex(ci => ci.CourseId)
                .IsUnique();

            // One-to-Many with CourseResource
            builder.HasMany(ci => ci.Resources)
                .WithOne()
                .HasForeignKey(cr => cr.CourseInfoId)
                .OnDelete(DeleteBehavior.Cascade);

            // Backing field access mode for collections
            builder.Navigation(ci => ci.Resources)
                .UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
