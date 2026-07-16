using JadaraITKnowledgeSystem.Application.Features.Majors.Dtos;
using JadaraITKnowledgeSystem.Application.Features.Majors.Mappers;
using JadaraITKnowledgeSystem.Application.Interfaces;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JadaraITKnowledgeSystem.Application.Features.Majors.Commands.UpdateMajor;

public sealed class UpdateMajorCommandHandler
    (IApplicationDbContext context, ILogger<UpdateMajorCommandHandler> logger)
    : IRequestHandler<UpdateMajorCommand, Result<MajorDto>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly ILogger<UpdateMajorCommandHandler> _logger = logger;

    public async Task<Result<MajorDto>> Handle(
        UpdateMajorCommand request,
        CancellationToken cancellationToken)
    {
        var major = await _context.Majors
            .FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken);

        if (major is null)
        {
            _logger.LogWarning("Major with ID {MajorId} not found.", request.Id);
            return Error.NotFound("Major.NotFound", $"Major with ID {request.Id} was not found.");
        }

        major.UpdateName(request.Name);
        major.UpdateFaculty(request.FacultyId);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Major {MajorId} updated successfully.", request.Id);

        return major.ToDto();
    }
}
