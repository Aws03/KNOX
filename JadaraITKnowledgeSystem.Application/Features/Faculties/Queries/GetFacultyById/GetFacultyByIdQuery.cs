using JadaraITKnowledgeSystem.Application.Features.Faculties.Dtos;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;

namespace JadaraITKnowledgeSystem.Application.Features.Faculties.Queries.GetFacultyById;

public sealed record GetFacultyByIdQuery(int facultyId) : IRequest<Result<FacultyDto>>;
