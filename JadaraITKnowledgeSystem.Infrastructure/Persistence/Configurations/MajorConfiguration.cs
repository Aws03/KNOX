using JadaraITKnowledgeSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JadaraITKnowledgeSystem.Infrastructure.Persistence.Configurations
{
    public class MajorConfiguration : IEntityTypeConfiguration<Major>
    {
        public void Configure(EntityTypeBuilder<Major> builder)
        {
            builder.ToTable("Majors");
            builder.HasKey(m => m.Id);

            builder.Property(m => m.Name)
                .IsRequired()
                .HasMaxLength(120);

            builder.HasOne(m => m.Faculty)
                .WithMany()
                .HasForeignKey(m => m.FacultyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(m => m.CourseRequirements)
                .WithOne(cr => cr.Major)
                .HasForeignKey(cr => cr.MajorId)
                .OnDelete(DeleteBehavior.Cascade);

            // Auditable fields
            builder.Property<DateTime>("CreatedAt").IsRequired();
            builder.Property<string>("CreatedBy").HasMaxLength(100).IsRequired(false);
            builder.Property<DateTime?>("UpdatedAt").IsRequired(false);
            builder.Property<string?>("UpdatedBy").HasMaxLength(100).IsRequired(false);
        }
    }
}