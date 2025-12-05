using JadaraITKnowledgeSystem.Domain.Users.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JadaraITKnowledgeSystem.Infrastructure.Persistence.Configurations
{
    public class VerificationOTPConfiguration : IEntityTypeConfiguration<VerificationOTP>
    {
        public void Configure(EntityTypeBuilder<VerificationOTP> builder)
        {
            builder.ToTable("VerificationOTPs");
            builder.HasKey(v => v.Id);

            builder.Property(v => v.OTP)
                .IsRequired()
                .HasMaxLength(6);

            builder.Property(v => v.UserId)
                .IsRequired();

            builder.Property(v => v.ExpiresAt)
                .IsRequired();

            builder.Property(v => v.IsUsed)
                .IsRequired();

            builder.HasOne(v => v.User)
                .WithMany()
                .HasForeignKey(v => v.UserId)
                .OnDelete(DeleteBehavior.Cascade);

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

            builder.HasIndex(v => new { v.UserId, v.IsUsed });
        }
    }
}
