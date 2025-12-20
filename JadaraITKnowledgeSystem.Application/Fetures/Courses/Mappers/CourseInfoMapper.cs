using JadaraITKnowledgeSystem.Application.Fetures.Courses.Dtos;
using JadaraITKnowledgeSystem.Domain.Courses.Entities;

namespace JadaraITKnowledgeSystem.Application.Fetures.Courses.Mappers;

public static class CourseInfoMapper
{
    public static CourseInfoDto ToDto(this CourseInfo courseInfo)
    {
        ArgumentNullException.ThrowIfNull(courseInfo);

        return new CourseInfoDto
        {
            Id = courseInfo.Id,
            CourseId = courseInfo.CourseId,
            DifficultyLevel = courseInfo.DifficultyLevel,
            Description = courseInfo.Description,
            DemonstrationVideoUrl = courseInfo.DemonstrationVideoUrl,
            DemonstrationVideoTitle = courseInfo.DemonstrationVideoTitle,
            Resources = courseInfo.Resources.ToDtos(),
            ResourceCount = courseInfo.ResourceCount
        };
    }

    public static List<CourseInfoDto> ToDtos(this IEnumerable<CourseInfo> courseInfos)
    {
        return courseInfos.Select(ci => ci.ToDto()).ToList();
    }
}
