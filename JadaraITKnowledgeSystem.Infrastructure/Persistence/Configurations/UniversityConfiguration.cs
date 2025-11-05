using JadaraITKnowledgeSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JadaraITKnowledgeSystem.Infrastructure.Persistence.Configurations
{
    public class UniversityConfiguration : IEntityTypeConfiguration<University>
    {
        public void Configure(EntityTypeBuilder<University> builder)
        {
            builder.ToTable("Universities");
            builder.HasKey(u => u.Id);

            builder.Property(u => u.Name)
                .IsRequired()
                .HasMaxLength(120);

            builder.HasMany(u => u.Faculties)
                .WithOne(f => f.University)
                .HasForeignKey(f => f.UniversityId)
                .OnDelete(DeleteBehavior.Cascade);

            // Auditable fields
            builder.Property<DateTime>("CreatedAt").IsRequired();
            builder.Property<string>("CreatedBy").HasMaxLength(100).IsRequired(false);
            builder.Property<DateTime?>("UpdatedAt").IsRequired(false);
            builder.Property<string?>("UpdatedBy").HasMaxLength(100).IsRequired(false);
        }
    }
}

