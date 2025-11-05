using JadaraITKnowledgeSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JadaraITKnowledgeSystem.Infrastructure.Persistence.Configurations
{
    public class FacultyConfiguration : IEntityTypeConfiguration<Faculty>
    {
        public void Configure(EntityTypeBuilder<Faculty> builder)
        {
            builder.ToTable("Faculties");
            builder.HasKey(f => f.Id);

            builder.Property(f => f.Name)
                .IsRequired()
                .HasMaxLength(120);

            builder.HasOne(f => f.University)
                .WithMany(u => u.Faculties)
                .HasForeignKey(f => f.UniversityId)
                .OnDelete(DeleteBehavior.Restrict);

            // Auditable fields
            builder.Property<DateTime>("CreatedAt").IsRequired();
            builder.Property<string>("CreatedBy").HasMaxLength(100).IsRequired(false);
            builder.Property<DateTime?>("UpdatedAt").IsRequired(false);
            builder.Property<string?>("UpdatedBy").HasMaxLength(100).IsRequired(false);
        }
    }
}