using JadaraITKnowledgeSystem.Domain.Courses.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JadaraITKnowledgeSystem.Infrastructure.Persistence.Configurations
{
    public class CourseResourceConfiguration : IEntityTypeConfiguration<CourseResource>
    {
        public void Configure(EntityTypeBuilder<CourseResource> builder)
        {
            builder.ToTable("CourseResources");
            builder.HasKey(cr => cr.Id);

            // Properties
            builder.Property(cr => cr.CourseInfoId)
                .IsRequired();

            builder.Property(cr => cr.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(cr => cr.Type)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(cr => cr.Url)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(cr => cr.Description)
                .HasMaxLength(1000)
                .IsRequired(false);

            builder.Property(cr => cr.DemonstrationVideoUrl)
                .HasMaxLength(1000)
                .IsRequired(false);

            // Auditable fields
            builder.Property(cr => cr.CreatedAt)
                .IsRequired();

            builder.Property(cr => cr.CreatedBy)
                .HasMaxLength(100)
                .IsRequired(false);

            builder.Property(cr => cr.UpdatedAt)
                .IsRequired(false);

            builder.Property(cr => cr.UpdatedBy)
                .HasMaxLength(100)
                .IsRequired(false);

            // Indexes
            builder.HasIndex(cr => cr.CourseInfoId);

            builder.HasIndex(cr => cr.Type);

            // Composite index for filtering resources by CourseInfo and Type
            builder.HasIndex(cr => new { cr.CourseInfoId, cr.Type });
        }
    }
}
