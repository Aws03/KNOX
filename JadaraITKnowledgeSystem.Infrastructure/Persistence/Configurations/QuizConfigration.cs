using JadaraITKnowledgeSystem.Domain.Quizzes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JadaraITKnowledgeSystem.Infrastructure.Persistence.Configurations
{
    public class QuizConfiguration : IEntityTypeConfiguration<Quiz>
    {
        public void Configure(EntityTypeBuilder<Quiz> builder)
        {
            builder.ToTable("Quizzes");
            builder.HasKey(q => q.Id);

            builder.Property(q => q.Title)
                .IsRequired();

            builder.Property(q => q.Description)
                .IsRequired(false);

            builder.Property(q => q.Likes)
                .HasDefaultValue(0);

            builder.Property(q => q.Dislikes)
                .HasDefaultValue(0);

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

            // Relations
            builder.HasOne(q => q.Course)
                .WithMany()
                .HasForeignKey(q => q.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(q => q.Writer)
                .WithMany()
                .HasForeignKey(q => q.WriterId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(q => q.Questions)
                .WithOne(qs => qs.Quiz)
                .HasForeignKey(qs => qs.QuizId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(q => q.Reactions)
                .WithOne(r => r.Quiz)
                .HasForeignKey(r => r.QuizId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(q => q.Attempts)
                .WithOne(a => a.Quiz)
                .HasForeignKey(a => a.QuizId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}