using JadaraITKnowledgeSystem.Application.Common.Models;
using JadaraITKnowledgeSystem.Application.Common.Queries;
using JadaraITKnowledgeSystem.Application.Features.Universities.Dtos;
using JadaraITKnowledgeSystem.Domain.Common.Results;

namespace JadaraITKnowledgeSystem.Application.Features.Universities.Queries.GetUniversities;

public sealed record GetUniversitiesQuery(
    int PageNumber = 1,
    int PageSize = 10,
    string? Name = null
) : PaginatedQuery<Result<PaginatedList<UniversityDto>>>(PageNumber, PageSize);
