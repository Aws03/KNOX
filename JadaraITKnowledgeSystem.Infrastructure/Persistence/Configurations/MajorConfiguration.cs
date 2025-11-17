using JadaraITKnowledgeSystem.Domain.Universities.Entities;
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