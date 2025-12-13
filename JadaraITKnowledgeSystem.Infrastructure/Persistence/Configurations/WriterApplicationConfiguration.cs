using JadaraITKnowledgeSystem.Domain.Users.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JadaraITKnowledgeSystem.Infrastructure.Persistence.Configurations
{
    public class WriterApplicationConfiguration : IEntityTypeConfiguration<WriterApplication>
    {
        public void Configure(EntityTypeBuilder<WriterApplication> builder)
        {
            builder.ToTable("WriterApplications");
            builder.HasKey(w => w.Id);

            builder.Property(w => w.PhoneNumber)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(w => w.UserId)
                .IsRequired();

            builder.HasOne(w => w.User)
                .WithMany()
                .HasForeignKey(w => w.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Auditable fields
            builder.Property(w => w.CreatedAt)
                .IsRequired();

            builder.Property(w => w.CreatedBy)
                .HasMaxLength(100)
                .IsRequired(false);

            builder.Property(w => w.UpdatedAt)
                .IsRequired(false);

            builder.Property(w => w.UpdatedBy)
                .HasMaxLength(100)
                .IsRequired(false);

            // Index for efficient queries
            builder.HasIndex(w => w.UserId);
        }
    }
}
