using JadaraITKnowledgeSystem.Application.DTOs;
using JadaraITKnowledgeSystem.Domain.Courses.Entites;

namespace JadaraITKnowledgeSystem.Application.Fetures.Courses.Mappers;

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
            FolderId = courseMaterial.FolderId,
        };
    }

    public static List<CourseMaterialDto> ToDtos(this IEnumerable<CourseMaterial> courseMaterials)
    {
        return courseMaterials.Select(cm => cm.ToDto()).ToList();
    }

    public static CourseMaterial ToEntity(this CourseMaterialDto courseMaterialDto)
    {
        ArgumentNullException.ThrowIfNull(courseMaterialDto);

        return CourseMaterial.Create(courseMaterialDto.Title, courseMaterialDto.ContentUrl, courseMaterialDto.CourseId,courseMaterialDto.FolderId, courseMaterialDto.Description).Value;
    }

    public static List<CourseMaterial> ToEntities(this IEnumerable<CourseMaterialDto> courseMaterialDtos)
    {
        return courseMaterialDtos.Select(cmd => cmd.ToEntity()).ToList();
    }

}
