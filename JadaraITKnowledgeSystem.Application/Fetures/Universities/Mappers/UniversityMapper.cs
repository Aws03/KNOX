using JadaraITKnowledgeSystem.Application.Fetures.Universities.Dtos;
using JadaraITKnowledgeSystem.Domain.Universities;

namespace JadaraITKnowledgeSystem.Application.Fetures.Universities.Mappers;

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

    public static University ToEntity(this UniversityDto universityDto)
    {
        ArgumentNullException.ThrowIfNull(universityDto);

        return University.Create(universityDto.Name).Value;
    }

    public static List<University> ToEntities(this IEnumerable<UniversityDto> universityDtos)
    {
        return universityDtos.Select(universityDto => universityDto.ToEntity()).ToList();
    }

}
