using JadaraITKnowledgeSystem.Application.DTOs;
using JadaraITKnowledgeSystem.Domain.Courses.Entites;
using System;
using System.Collections.Generic;
using System.Text;

namespace JadaraITKnowledgeSystem.Application.Fetures.Courses.Mappers
{
    public static class CourseMaterialMapper
    {
        public static CourseMaterialDto  ToDto(this CourseMaterial courseMaterial)
        {
            ArgumentNullException.ThrowIfNull(courseMaterial);

            return new CourseMaterialDto
            {
                Id = courseMaterial.Id,
                ContentUrl = courseMaterial.ContentUrl,
                Title = courseMaterial.Title,
                Description = courseMaterial.Description,
                CourseId = courseMaterial.CourseId,
            };
        }

        public static List<CourseMaterialDto> ToDtos(this IEnumerable<CourseMaterial> courseMaterials)
        {
            return courseMaterials.Select(cm => cm.ToDto()).ToList();
        }

        public static CourseMaterial ToEntity(this CourseMaterialDto courseMaterialDto)
        {
            ArgumentNullException.ThrowIfNull(courseMaterialDto);

            return CourseMaterial.Create(courseMaterialDto.Title, courseMaterialDto.ContentUrl, courseMaterialDto.CourseId, courseMaterialDto.Description).Value;
        }

        public static List<CourseMaterial> ToEntities(this IEnumerable<CourseMaterialDto> courseMaterialDtos)
        {
            return courseMaterialDtos.Select(cmd => cmd.ToEntity()).ToList();
        }

    }
}
