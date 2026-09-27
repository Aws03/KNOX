using JadaraITKnowledgeSystem.Application.Features.Majors.Dtos;
using JadaraITKnowledgeSystem.Domain.Universities.Entities;

namespace JadaraITKnowledgeSystem.Application.Features.Majors.Mappers;

public static class MajorMapper
{
    public static MajorDto ToDto(this Major major)
    {
        ArgumentNullException.ThrowIfNull(major);

        return new MajorDto
        {
            Id = major.Id,
            Name = major.Name,
            FacultyId = major.FacultyId
        };
    }

    public static List<MajorDto> ToDtos(this IEnumerable<Major> majors)
    {
        return majors.Select(major => major.ToDto()).ToList();
    }
}
