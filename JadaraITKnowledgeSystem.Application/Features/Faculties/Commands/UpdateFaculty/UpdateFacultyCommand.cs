using JadaraITKnowledgeSystem.Application.Features.Faculties.Dtos;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;

namespace JadaraITKnowledgeSystem.Application.Features.Faculties.Commands.UpdateFaculty;

public sealed record UpdateFacultyCommand(int Id, string Name, int UniversityId) : IRequest<Result<FacultyDto>>;
