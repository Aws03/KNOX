using System.Text;
using JadaraITKnowledgeSystem.Application.Features.Courses.Dtos;
using JadaraITKnowledgeSystem.Domain.Courses.Entites;

namespace JadaraITKnowledgeSystem.Application.Features.Courses.Mappers
{
    public static class FolderMapper
    {
        public static FolderDto ToDto(this Folder folder)
        {
            ArgumentNullException.ThrowIfNull(folder, nameof(folder));

            return new FolderDto
            {
                Id = folder.Id,
                Name = folder.Name,
                CourseId = folder.CourseId,
                ParentFolderId = folder.ParentFolderId,
                Description = folder.Description,
            };
        }

        public static List<FolderDto> ToDtos(this IEnumerable<Folder> folders)
        {
            ArgumentNullException.ThrowIfNull(folders, nameof(folders));
            var folderDtos = new List<FolderDto>();
            foreach (var folder in folders)
            {
                folderDtos.Add(folder.ToDto());
            }
            return folderDtos;
        }
    }
}
