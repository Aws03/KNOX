using JadaraITKnowledgeSystem.Application.Features.Faculties.Dtos;
using JadaraITKnowledgeSystem.Domain.Universities.Entities;

namespace JadaraITKnowledgeSystem.Application.Features.Faculties.Mappers;

public static class FacultyMapper
{
    public static FacultyDto ToDto(this Faculty faculty)
    {
        ArgumentNullException.ThrowIfNull(faculty);
        return new FacultyDto
        {
            Id = faculty.Id,
            Name = faculty.Name,
            UniversityId = faculty.UniversityId
        };

    }

    public static List<FacultyDto> ToDtos(this IEnumerable<Faculty> faculties)
    {
        return faculties.Select(faculty => faculty.ToDto()).ToList();
    }

    public static Faculty ToEntity(this FacultyDto facultyDto)
    {
        ArgumentNullException.ThrowIfNull(facultyDto);

        return Faculty.Create(facultyDto.Name, facultyDto.UniversityId).Value;
    }

    public static List<Faculty> ToEntities(this IEnumerable<FacultyDto> facultyDtos)
    {
        return facultyDtos.Select(facultyDto => facultyDto.ToEntity()).ToList();
    }
}
