using JadaraITKnowledgeSystem.Application.Fetures.Faculties.Dtos;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;


namespace JadaraITKnowledgeSystem.Application.Fetures.Faculties.Commands.CreateFaculty;

public sealed record CreateFacultyCommand(string Name, int UniversityId) : IRequest<Result<FacultyDto>>;

