using JadaraITKnowledgeSystem.Application.Common.Models;
using JadaraITKnowledgeSystem.Application.Common.Queries;
using JadaraITKnowledgeSystem.Application.Features.Users.Dtos;
using JadaraITKnowledgeSystem.Domain.Common.Results;

namespace JadaraITKnowledgeSystem.Application.Features.Users.Queries.GetUsersWithDetails;

public sealed record GetUsersWithDetailsQuery(
    int? UniversityId = null,
    int? FacultyId = null,
    int? MajorId = null,
    string? Email = null,
    int? Id = null,
    bool? IsActive = null,
    bool? IsVerified = null,
    int PageNumber = 1,
    int PageSize = 10
) : PaginatedQuery<Result<PaginatedList<UserDetailsDto>>>(PageNumber, PageSize);
