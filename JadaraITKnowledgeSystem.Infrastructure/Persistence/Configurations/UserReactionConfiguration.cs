using JadaraITKnowledgeSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace JadaraITKnowledgeSystem.Infrastructure.Persistence.Configurations
{
    public class UserReactionConfiguration : IEntityTypeConfiguration<UserReaction>
    {
        public void Configure(EntityTypeBuilder<UserReaction> builder)
        {
            builder.ToTable("UserReactions");
            builder.HasKey(ur => ur.Id);

            builder.Property(ur => ur.IsLike).IsRequired();

            builder.HasOne(ur => ur.User)
                .WithMany()
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(ur => ur.Quiz)
                .WithMany(q => q.Reactions)
                .HasForeignKey(ur => ur.QuizId)
                .OnDelete(DeleteBehavior.Cascade);

            // Auditable fields
            builder.Property<DateTime>("CreatedAt").IsRequired();
            builder.Property<string>("CreatedBy").HasMaxLength(100).IsRequired(false);
            builder.Property<DateTime?>("UpdatedAt").IsRequired(false);
            builder.Property<string?>("UpdatedBy").HasMaxLength(100).IsRequired(false);
        }
    }
}