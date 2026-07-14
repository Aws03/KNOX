using JadaraITKnowledgeSystem.Application.Features.Faculties.Dtos;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;


namespace JadaraITKnowledgeSystem.Application.Features.Faculties.Commands.CreateFaculty;

public sealed record CreateFacultyCommand(string Name, int UniversityId) : IRequest<Result<FacultyDto>>;

