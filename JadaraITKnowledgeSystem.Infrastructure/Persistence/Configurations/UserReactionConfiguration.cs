using JadaraITKnowledgeSystem.Domain.Quizzes.Entites;
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

            builder.Property(ur => ur.ReactionType).IsRequired();

            builder.HasOne(ur => ur.User)
                .WithMany()
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(ur => ur.Quiz)
                .WithMany(q => q.Reactions)
                .HasForeignKey(ur => ur.QuizId)
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