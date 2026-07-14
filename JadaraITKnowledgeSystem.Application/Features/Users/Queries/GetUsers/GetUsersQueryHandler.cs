using JadaraITKnowledgeSystem.Application.Common.Models;
using JadaraITKnowledgeSystem.Application.Features.Users.Dtos;
using JadaraITKnowledgeSystem.Application.Features.Users.Mappings;
using JadaraITKnowledgeSystem.Application.Features.Users.Queries.GetUsers;
using JadaraITKnowledgeSystem.Application.Interfaces;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JadaraITKnowledgeSystem.Application.Features.Users.Queries.GetUsers;

public sealed class GetUsersQueryHandler
    (IApplicationDbContext context, ILogger<GetUsersQueryHandler> logger)
    : IRequestHandler<GetUsersQuery, Result<PaginatedList<UserDto>>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly ILogger<GetUsersQueryHandler> _logger = logger;

    public async Task<Result<PaginatedList<UserDto>>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Handling GetUsersQuery: Page {Page}, PageSize {PageSize}",
            request.PageNumber, request.PageSize);

        var query = _context.Users
            .AsNoTracking()
            .Include(u => u.Major)
                .ThenInclude(m => m.Faculty)
                    .ThenInclude(f => f.University)
            .AsQueryable();

        if (request.Id is int id && id > 0)
            query = query.Where(u => u.Id == id);

        if (request.Email is string email)
            query = query.Where(u => u.Email.Address.Contains(email));

        if (request.MajorId is int majorId)
            query = query.Where(u => u.MajorId == majorId);

        if (request.FacultyId is int facultyId)
            query = query.Where(u => u.Major.FacultyId == facultyId);

        if (request.UniversityId is int universityId)
            query = query.Where(u => u.Major.Faculty.UniversityId == universityId);

        query = query.OrderBy(u => u.Name.Value);

        var projected = query.Select(u => u.ToDto());

        var paginatedList = await PaginatedList<UserDto>.CreateAsync(
            projected,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        _logger.LogInformation(
            "Retrieved {Count} users for Page {Page}",
            paginatedList.Items.Count, request.PageNumber);

        return paginatedList;
    }
}
