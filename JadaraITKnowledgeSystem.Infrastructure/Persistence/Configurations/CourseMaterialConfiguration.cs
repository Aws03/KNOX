using JadaraITKnowledgeSystem.Domain.Courses.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JadaraITKnowledgeSystem.Infrastructure.Persistence.Configurations
{
    internal class CourseMaterialConfiguration : IEntityTypeConfiguration<CourseMaterial>
    {
        public void Configure(EntityTypeBuilder<CourseMaterial> builder)
        {
            builder.ToTable("CourseMaterials");
            builder.HasKey(m => m.Id);

            builder.Property(m => m.Title)
                   .IsRequired()
                   .HasMaxLength(250);

            builder.Property(m => m.ContentUrl)
                   .IsRequired()
                   .HasMaxLength(500);

            builder.Property(m => m.Description)
                   .HasMaxLength(500)
                   .IsRequired(false);

            // Auditable fields
            builder.Property(m => m.CreatedAt)
                   .IsRequired();

            builder.Property(m => m.CreatedBy)
                   .HasMaxLength(100)
                   .IsRequired(false);

            builder.Property(m => m.UpdatedAt)
                   .IsRequired(false);

            builder.Property(m => m.UpdatedBy)
                   .HasMaxLength(100)
                   .IsRequired(false);

            // Relationships
            builder.HasOne(m => m.Course)
                   .WithMany(c => c.Materials)
                   .HasForeignKey(m => m.CourseId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(m => m.Folder)
                   .WithMany(f => f.Materials)
                   .HasForeignKey(m => m.FolderId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Indexes to optimize lookups
            builder.HasIndex(m => m.CourseId);
            builder.HasIndex(m => new { m.CourseId, m.FolderId });
        }
    }
}
