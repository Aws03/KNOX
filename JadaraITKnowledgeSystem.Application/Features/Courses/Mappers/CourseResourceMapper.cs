using JadaraITKnowledgeSystem.Application.Features.Courses.Dtos;
using JadaraITKnowledgeSystem.Domain.Courses.Entities;

namespace JadaraITKnowledgeSystem.Application.Features.Courses.Mappers;

public static class CourseResourceMapper
{
    public static CourseResourceDto ToDto(this CourseResource resource)
    {
        ArgumentNullException.ThrowIfNull(resource);

        return new CourseResourceDto
        {
            Id = resource.Id,
            CourseInfoId = resource.CourseInfoId,
            Title = resource.Title,
            Type = resource.Type,
            Url = resource.Url,
            Description = resource.Description,
            DemonstrationVideoUrl = resource.DemonstrationVideoUrl,
            HasDemonstrationVideo = resource.HasDemonstrationVideo()
        };
    }

    public static List<CourseResourceDto> ToDtos(this IEnumerable<CourseResource> resources)
    {
        return resources.Select(r => r.ToDto()).ToList();
    }
}
