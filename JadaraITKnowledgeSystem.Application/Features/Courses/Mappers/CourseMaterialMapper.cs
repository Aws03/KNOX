using JadaraITKnowledgeSystem.Application.Features.Courses.Dtos;
using JadaraITKnowledgeSystem.Application.Interfaces;
using JadaraITKnowledgeSystem.Domain.Courses.Entities;

namespace JadaraITKnowledgeSystem.Application.Features.Courses.Mappers;

public static class CourseMaterialMapper
{
    /// <summary>Maps a material with a freshly signed URL: materials are private, their files are never public.</summary>
    public static CourseMaterialDto ToDto(this CourseMaterial courseMaterial, IFileManager files)
    {
        ArgumentNullException.ThrowIfNull(courseMaterial);

        return new CourseMaterialDto
        {
            Id = courseMaterial.Id,
            ContentUrl = files.GetMaterialUrl(courseMaterial.StorageKey),
            ContentType = courseMaterial.ContentType,
            SizeBytes = courseMaterial.SizeBytes,
            Title = courseMaterial.Title,
            Description = courseMaterial.Description,
            CourseId = courseMaterial.CourseId,
            FolderId = courseMaterial.FolderId,
            Tags = courseMaterial.Tags.ToList()
        };
    }
}
