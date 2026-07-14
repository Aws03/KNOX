using JadaraITKnowledgeSystem.Application.Common.Models;
using JadaraITKnowledgeSystem.Application.Common.Queries;
using JadaraITKnowledgeSystem.Application.Features.Users.Dtos;
using JadaraITKnowledgeSystem.Domain.Common.Results;

namespace JadaraITKnowledgeSystem.Application.Features.Users.Queries.GetUsers;

public sealed record GetUsersQuery(
    int? UniversityId = null,
    int? FacultyId = null,
    int? MajorId = null,
    string? Email = null,
    int? Id = null,
    int PageNumber = 1,
    int PageSize = 10
) : PaginatedQuery<Result<PaginatedList<UserDto>>>(PageNumber, PageSize);
