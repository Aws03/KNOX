using JadaraITKnowledgeSystem.Application.Features.Majors.Dtos;

namespace JadaraITKnowledgeSystem.Application.Features.Faculties.Dtos;

public sealed record FacultyDto
{
    public int Id { get; init; }
    public string Name { get; init; } = String.Empty;
    public int UniversityId { get; init; }
    public List<MajorDto> Majors { get; init; } = new();
}
