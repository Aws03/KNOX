using JadaraITKnowledgeSystem.Application.Features.Courses.Dtos;
using JadaraITKnowledgeSystem.Domain.Courses.Entites;

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
}
