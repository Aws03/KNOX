using JadaraITKnowledgeSystem.Domain.Quizzes.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace JadaraITKnowledgeSystem.Infrastructure.Persistence.Configurations
{
    public class ChoiceConfiguration : IEntityTypeConfiguration<Choice>
    {
        public void Configure(EntityTypeBuilder<Choice> builder)
        {
            builder.ToTable("Choices");
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Text)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(c => c.IsCorrect)
                .IsRequired();

            builder.Property(c => c.ImageUrl)
                .IsRequired(false);

            builder.HasOne(c => c.Question)
                .WithMany(q => q.Choices)
                .HasForeignKey(c => c.QuestionId)
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