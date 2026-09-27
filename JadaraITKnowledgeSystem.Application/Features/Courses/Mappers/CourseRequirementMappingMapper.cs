using JadaraITKnowledgeSystem.Application.Features.Courses.Dtos;
using JadaraITKnowledgeSystem.Domain.Courses.Entities;

namespace JadaraITKnowledgeSystem.Application.Features.Courses.Mappers;

public static class CourseRequirementMappingMapper
{
    public static CourseRequirementMappingDto ToDto(this CourseRequirementMapping crm)
    {
        ArgumentNullException.ThrowIfNull(crm);

        return new CourseRequirementMappingDto
        {
            Id = crm.Id,
            RequirementType = crm.RequirementType,
            CourseId = crm.CourseId,
            MajorId = crm.MajorId,
            RequirementNature = crm.RequirementNature
        };

    }

    public static List<CourseRequirementMappingDto> ToDtos(this IEnumerable<CourseRequirementMapping> crms)
    {
        return crms.Select(crm => crm.ToDto()).ToList();
    }
}
