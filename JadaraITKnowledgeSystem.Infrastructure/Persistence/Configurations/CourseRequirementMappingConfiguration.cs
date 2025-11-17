using JadaraITKnowledgeSystem.Domain.Courses.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace JadaraITKnowledgeSystem.Infrastructure.Persistence.Configurations
{
    public class CourseRequirementMappingConfiguration : IEntityTypeConfiguration<CourseRequirementMapping>
    {
        public void Configure(EntityTypeBuilder<CourseRequirementMapping> builder)
        {
            builder.ToTable("CourseRequirementMappings");
            builder.HasKey(m => m.Id);

            builder.Property(m => m.RequirementType)
                .IsRequired();

            builder.Property(m => m.RequirementNature)
                .IsRequired();

            builder.HasOne(m => m.Course)
                .WithMany(c => c.Requirements)
                .HasForeignKey(m => m.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(m => m.Major)
                .WithMany()
                .HasForeignKey(m => m.MajorId)
                .OnDelete(DeleteBehavior.Restrict);

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
        }
    }
}