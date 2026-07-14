using JadaraITKnowledgeSystem.Application.Features.Courses.Dtos;
using JadaraITKnowledgeSystem.Domain.Courses.Entites;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JadaraITKnowledgeSystem.Application.Features.Courses.Mappers;

public static class CourseMaterialMapper
{
    public static CourseMaterialDto ToDto(this CourseMaterial courseMaterial)
    {
        ArgumentNullException.ThrowIfNull(courseMaterial);

        return new CourseMaterialDto
        {
            Id = courseMaterial.Id,
            ContentUrl = courseMaterial.ContentUrl,
            Title = courseMaterial.Title,
            Description = courseMaterial.Description,
            CourseId = courseMaterial.CourseId,
            FolderId = courseMaterial.FolderId,
            Tags = courseMaterial.Tags.ToList()
        };
    }

    public static List<CourseMaterialDto> ToDtos(this IEnumerable<CourseMaterial> courseMaterials)
    {
        return courseMaterials.Select(cm => cm.ToDto()).ToList();
    }

    public static CourseMaterial ToEntity(this CourseMaterialDto courseMaterialDto)
    {
        ArgumentNullException.ThrowIfNull(courseMaterialDto);

        return CourseMaterial.Create(
            courseMaterialDto.Title,
            courseMaterialDto.ContentUrl,
            courseMaterialDto.CourseId,
            courseMaterialDto.FolderId,
            courseMaterialDto.Description,
            courseMaterialDto.Tags
        ).Value;
    }

    public static List<CourseMaterial> ToEntities(this IEnumerable<CourseMaterialDto> courseMaterialDtos)
    {
        return courseMaterialDtos.Select(cmd => cmd.ToEntity()).ToList();
    }
}
