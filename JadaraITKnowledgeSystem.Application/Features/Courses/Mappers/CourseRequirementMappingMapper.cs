using JadaraITKnowledgeSystem.Application.DTOs;
using JadaraITKnowledgeSystem.Domain.Courses.Entites;

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

    public static CourseRequirementMapping ToEntity(this CourseRequirementMappingDto crmd)
    {
        ArgumentNullException.ThrowIfNull(crmd);

        return CourseRequirementMapping.Create(crmd.CourseId, crmd.MajorId, crmd.RequirementType, crmd.RequirementNature).Value;
    }

    public static List<CourseRequirementMapping> ToEntities(this IEnumerable<CourseRequirementMappingDto> crmds)
    {
        return crmds.Select(crmd => crmd.ToEntity()).ToList();
    }
}
