using JadaraITKnowledgeSystem.Application.Common.Models;
using JadaraITKnowledgeSystem.Application.Common.Queries;
using JadaraITKnowledgeSystem.Application.Features.Quizzes.Dtos;
using JadaraITKnowledgeSystem.Application.Features.Universities.Dtos;
using JadaraITKnowledgeSystem.Domain.Common.Results;

namespace JadaraITKnowledgeSystem.Application.Features.Universities.Queries.GetUniversities;

public sealed record GetUniversitiesQuery(
int PageNumber = 1,
int PageSize = 10
) : PaginatedQuery<Result<PaginatedList<UniversityDto>>>(PageNumber, PageSize);
