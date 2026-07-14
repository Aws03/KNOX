using JadaraITKnowledgeSystem.Application.Common.Models;
using JadaraITKnowledgeSystem.Application.Features.Faculties.Dtos;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;

namespace JadaraITKnowledgeSystem.Application.Features.Faculties.Queries.GetFacultiesByUniversityId;

public sealed record GetFacultiesByUniversityIdQuery(
    int UniversityId,
    int PageNumber,
    int PageSize) : IRequest<Result<PaginatedList<FacultyDto>>>;

