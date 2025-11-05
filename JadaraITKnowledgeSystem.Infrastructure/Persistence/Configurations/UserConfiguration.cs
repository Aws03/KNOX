using JadaraITKnowledgeSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JadaraITKnowledgeSystem.Infrastructure.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");
            builder.HasKey(u => u.Id);

            // Value objects mapping
            builder.OwnsOne(u => u.Name, name =>
            {
                name.Property(n => n.Value)
                    .HasColumnName("Name")
                    .IsRequired()
                    .HasMaxLength(200);
            });

            builder.OwnsOne(u => u.Email, email =>
            {
                email.Property(e => e.Address)
                    .HasColumnName("Email")
                    .IsRequired()
                    .HasMaxLength(254);
            });

            builder.Property(u => u.PermissionLevel)
                .IsRequired();

            builder.Property(u => u.DateJoined)
                .IsRequired();

            // Auditable fields inherited (if used separately)
            builder.Property<DateTime>("CreatedAt").IsRequired();
            builder.Property<string>("CreatedBy").HasMaxLength(100).IsRequired(false);
            builder.Property<DateTime?>("UpdatedAt").IsRequired(false);
            builder.Property<string?>("UpdatedBy").HasMaxLength(100).IsRequired(false);
        }
    }
}