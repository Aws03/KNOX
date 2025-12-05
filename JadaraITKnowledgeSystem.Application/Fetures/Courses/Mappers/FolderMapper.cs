using JadaraITKnowledgeSystem.Application.Fetures.Courses.Dtos;
using JadaraITKnowledgeSystem.Domain.Courses.Entites;
using System;
using System.Collections.Generic;
using System.Text;

namespace JadaraITKnowledgeSystem.Application.Fetures.Courses.Mappers
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

        public static Folder ToEntity(this FolderDto folderDto)
        {
            ArgumentNullException.ThrowIfNull(folderDto, nameof(folderDto));
            return Folder.Create(folderDto.Name, folderDto.CourseId, folderDto.ParentFolderId, folderDto.Description).Value;
        }

        public static List<Folder> ToEntities(this IEnumerable<FolderDto> folderDtos)
        {
            ArgumentNullException.ThrowIfNull(folderDtos, nameof(folderDtos));
            var folders = new List<Folder>();
            foreach (var folderDto in folderDtos)
            {
                folders.Add(folderDto.ToEntity());
            }
            return folders;
        }
    }
}
