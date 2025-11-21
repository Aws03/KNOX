using JadaraITKnowledgeSystem.Application.Common.Models;
using JadaraITKnowledgeSystem.Application.Fetures.Majors.Dtos;
using JadaraITKnowledgeSystem.Application.Fetures.Majors.Mappers;
using JadaraITKnowledgeSystem.Application.Fetures.Majors.Queries.GetMajorByFacultyId;
using JadaraITKnowledgeSystem.Application.Interfaces;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JadaraITKnowledgeSystem.Application.Fetures.Majors.Queries.GetMajorsByFacultyId;

public sealed class GetMajorsByFacultyIdQueryHandler
    (IApplicationDbContext context, ILogger<GetMajorsByFacultyIdQueryHandler> logger)
    : IRequestHandler<GetMajorsByFacultyIdQuery, Result<PaginatedList<MajorDto>>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly ILogger<GetMajorsByFacultyIdQueryHandler> _logger = logger;

    public async Task<Result<PaginatedList<MajorDto>>> Handle(
        GetMajorsByFacultyIdQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Handling GetMajorsByFacultyIdQuery: FacultyId {FacultyId}, Page {Page}, PageSize {PageSize}",
            request.FacultyId, request.PageNumber, request.PageSize);

        // Query majors filtered by faculty id
        var query = _context.Majors
            .AsNoTracking()
            .Where(m => m.FacultyId == request.FacultyId)
            .OrderBy(m => m.Name)
            .Select(m => m.ToDto());

        var paginatedList = await PaginatedList<MajorDto>.CreateAsync(
            query,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        _logger.LogInformation(
            "Retrieved {Count} Majors for FacultyId {FacultyId} on Page {Page}",
            paginatedList.Items.Count, request.FacultyId, request.PageNumber);

        return paginatedList;
    }
}
