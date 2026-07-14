using JadaraITKnowledgeSystem.Application.Features.Courses.Dtos;
using JadaraITKnowledgeSystem.Domain.Courses;

namespace JadaraITKnowledgeSystem.Application.Features.Courses.Mappers;

public static class CourseMapper
{
    public static CourseDto ToDto(this Course course)
    {
        ArgumentNullException.ThrowIfNull(course);

        return new CourseDto
        {
            Id = course.Id,
            CourseName = course.CourseName,
            Description = course.Description,
            CourseCode = course.CourseCode,
            Credits = course.Credits,
            CourseRequirementMappings = (course.Requirements != null) ? course.Requirements.ToDtos() : new()
        };

    }

    public static List<CourseDto> ToDtos(this IEnumerable<Course> courses)
    {
        return courses.Select(course => course.ToDto()).ToList();
    }

    public static Course ToEntity(this CourseDto courseDto)
    {
        ArgumentNullException.ThrowIfNull(courseDto);

        return Course.Create(courseDto.CourseName, courseDto.Credits, courseDto.Description, courseDto.CourseCode).Value;
    }

    public static List<Course> ToEntities(this IEnumerable<CourseDto> courseDtos)
    {
        return courseDtos.Select(courseDto => courseDto.ToEntity()).ToList();
    }
}
