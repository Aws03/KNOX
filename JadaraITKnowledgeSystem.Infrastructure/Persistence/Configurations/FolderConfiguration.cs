using JadaraITKnowledgeSystem.Domain.Courses.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JadaraITKnowledgeSystem.Infrastructure.Persistence.Configurations
{
    internal class FolderConfiguration : IEntityTypeConfiguration<Folder>
    {
        public void Configure(EntityTypeBuilder<Folder> builder)
        {
            builder.ToTable("Folders");

            builder.HasKey(f => f.Id);

            builder.Property(f => f.Name)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(f => f.Description)
                   .HasMaxLength(500);

            builder.Property(f => f.CourseId)
                   .IsRequired();

            // Self-reference (hierarchical folders)
            builder.HasOne(f => f.ParentFolder)
                   .WithMany(f => f.SubFolders)
                   .HasForeignKey(f => f.ParentFolderId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Course relationship
            builder.HasOne(f => f.Course)
                   .WithMany(c => c.Folders)
                   .HasForeignKey(f => f.CourseId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Unique constraint: prevent duplicate names within same parent in a course
            builder.HasIndex(f => new { f.CourseId, f.ParentFolderId, f.Name })
                   .IsUnique();

            // Backing fields for collections
            builder.Navigation(f => f.SubFolders)
                   .UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.Navigation(nameof(Folder.Materials))
                   .UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}