using JadaraITKnowledgeSystem.Application.Fetures.Faculties.Dtos;

namespace JadaraITKnowledgeSystem.Application.Fetures.Universities.Dtos;

public sealed record UniversityDto
{
    public int Id { get; init; }
    public string Name { get; init; } = String.Empty;
    public IReadOnlyList<FacultyDto>? Faculties { get; init; }
}
