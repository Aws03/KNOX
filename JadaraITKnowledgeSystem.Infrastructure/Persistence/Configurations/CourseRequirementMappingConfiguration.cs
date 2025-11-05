using JadaraITKnowledgeSystem.Domain.Entities;
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
            builder.Property<DateTime>("CreatedAt").IsRequired();
            builder.Property<string>("CreatedBy").HasMaxLength(100).IsRequired(false);
            builder.Property<DateTime?>("UpdatedAt").IsRequired(false);
            builder.Property<string?>("UpdatedBy").HasMaxLength(100).IsRequired(false);
        }
    }
}