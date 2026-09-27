using JadaraITKnowledgeSystem.Application.Common.Models;
using JadaraITKnowledgeSystem.Application.Features.Faculties.Dtos;
using JadaraITKnowledgeSystem.Application.Features.Faculties.Mappers;
using JadaraITKnowledgeSystem.Application.Interfaces;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JadaraITKnowledgeSystem.Application.Features.Faculties.Queries.GetFacultiesByUniversityId;

public sealed class GetFacultiesByUniversityIdQueryHandler
    (IApplicationDbContext context, ILogger<GetFacultiesByUniversityIdQueryHandler> logger)
    : IRequestHandler<GetFacultiesByUniversityIdQuery, Result<PaginatedList<FacultyDto>>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly ILogger<GetFacultiesByUniversityIdQueryHandler> _logger = logger;

    public async Task<Result<PaginatedList<FacultyDto>>> 
        Handle(GetFacultiesByUniversityIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
           "Handling GetFacultiesByUniversityIdQuery: UniversityId {UniversityId}, Page {Page}, PageSize {PageSize}",
           request.UniversityId, request.PageNumber, request.PageSize);

        var query = _context.Faculties
            .AsNoTracking()
            .Where(f => f.UniversityId == request.UniversityId)
            .Where(f => string.IsNullOrWhiteSpace(request.Name) || f.Name.Contains(request.Name.Trim()))
            .OrderBy(f => f.Name)
            .Select(f => f.ToDto());

        // Use the factory method to create the paginated list
        var paginatedList = await PaginatedList<FacultyDto>.CreateAsync(
            query,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        _logger.LogInformation(
            "Retrieved {Count} Faculties for Page {Page}",
            paginatedList.Items.Count, request.PageNumber);

        return paginatedList;
    }
}
