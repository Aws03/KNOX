using JadaraITKnowledgeSystem.Application.Common.Models;
using JadaraITKnowledgeSystem.Application.Fetures.Universities.Dtos;
using JadaraITKnowledgeSystem.Application.Fetures.Universities.Mappers;
using JadaraITKnowledgeSystem.Application.Interfaces;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace JadaraITKnowledgeSystem.Application.Fetures.Universities.Queries.GetUniversities;

public sealed class GetUniversitiesQueryHandler
    (IApplicationDbContext context, ILogger<GetUniversitiesQueryHandler> logger)
    : IRequestHandler<GetUniversitiesQuery, Result<PaginatedList<UniversityDto>>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly ILogger<GetUniversitiesQueryHandler> _logger = logger;

    public async Task<Result<PaginatedList<UniversityDto>>> Handle(
        GetUniversitiesQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Handling GetUniversitiesQuery: Page {Page}, PageSize {PageSize}",
            request.PageNumber, request.PageSize);

        // Project the query to UniversityDto before pagination
        var query = _context.Universities
            .AsNoTracking()
            .OrderBy(u => u.Name)
            .Select(u => u.ToDto());

        // Use the factory method to create the paginated list
        var paginatedList = await PaginatedList<UniversityDto>.CreateAsync(
            query,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        _logger.LogInformation(
            "Retrieved {Count} universities for Page {Page}",
            paginatedList.Items.Count, request.PageNumber);

        return paginatedList;
    }
}
