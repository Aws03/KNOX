using JadaraITKnowledgeSystem.Application.Features.Universities.Dtos;
using JadaraITKnowledgeSystem.Application.Features.Universities.Mappers;
using JadaraITKnowledgeSystem.Application.Interfaces;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JadaraITKnowledgeSystem.Application.Features.Universities.Commands.UpdateUniversity;

public sealed class UpdateUniversityCommandHandler
    (IApplicationDbContext context, ILogger<UpdateUniversityCommandHandler> logger)
    : IRequestHandler<UpdateUniversityCommand, Result<UniversityDto>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly ILogger<UpdateUniversityCommandHandler> _logger = logger;

    public async Task<Result<UniversityDto>> Handle(
        UpdateUniversityCommand request,
        CancellationToken cancellationToken)
    {
        var university = await _context.Universities
            .FirstOrDefaultAsync(u => u.Id == request.Id, cancellationToken);

        if (university is null)
        {
            _logger.LogWarning("University with ID {UniversityId} not found.", request.Id);
            return Error.NotFound("University.NotFound", $"University with ID {request.Id} was not found.");
        }

        university.UpdateName(request.Name);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("University {UniversityId} updated successfully.", request.Id);

        return university.ToDto();
    }
}
