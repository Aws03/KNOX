using JadaraITKnowledgeSystem.Application.Common.Models;
using JadaraITKnowledgeSystem.Application.Fetures.Faculties.Dtos;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;

namespace JadaraITKnowledgeSystem.Application.Fetures.Faculties.Queries.GetFacultiesByUniversityId;

public sealed record GetFacultiesByUniversityIdQuery(
    int UniversityId,
    int PageNumber,
    int PageSize) : IRequest<Result<PaginatedList<FacultyDto>>>;

