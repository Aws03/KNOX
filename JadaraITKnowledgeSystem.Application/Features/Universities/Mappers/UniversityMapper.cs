using JadaraITKnowledgeSystem.Application.Features.Universities.Dtos;
using JadaraITKnowledgeSystem.Domain.Universities;

namespace JadaraITKnowledgeSystem.Application.Features.Universities.Mappers;

public static class UniversityMapper
{
    public static UniversityDto ToDto(this University university)
    {
        ArgumentNullException.ThrowIfNull(university);
        return new UniversityDto
        {
            Id = university.Id,
            Name = university.Name
        };
    }

    public static List<UniversityDto> ToDtos(this List<University> universities)
    {
        return universities.Select(universitie => universitie.ToDto()).ToList();
    }

}
