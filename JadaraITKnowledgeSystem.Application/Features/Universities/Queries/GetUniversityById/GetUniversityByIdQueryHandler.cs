using JadaraITKnowledgeSystem.Application.Features.Universities.Dtos;
using JadaraITKnowledgeSystem.Application.Features.Universities.Mappers;
using JadaraITKnowledgeSystem.Application.Interfaces;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JadaraITKnowledgeSystem.Application.Features.Universities.Queries.GetUniversityById;

public sealed class GetUniversityByIdQueryHandler
    (IApplicationDbContext context, ILogger<GetUniversityByIdQueryHandler> logger)
    : IRequestHandler<GetUniversityByIdQuery, Result<UniversityDto>>
{
    private readonly ILogger<GetUniversityByIdQueryHandler> _logger = logger;
    private readonly IApplicationDbContext _context = context;

    public async Task<Result<UniversityDto>> Handle(
        GetUniversityByIdQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling GetUniversityByIdQuery for UniversityId: {UniversityId}", request.UniversityId);

        var university = await _context.Universities
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == request.UniversityId, cancellationToken);

        if (university == null)
        {
            // TODO : use application error insted of domain error later ...
            _logger.LogWarning("University with ID {UniversityId} not found.", request.UniversityId);
            return Error.NotFound(
                "University.NotFound",
                $"University with ID {request.UniversityId} was not found.");
        }

        var dto = university.ToDto();

        _logger.LogInformation("University with ID {UniversityId} retrieved successfully.", request.UniversityId);

        return dto;
    }
}
