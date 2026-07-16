using JadaraITKnowledgeSystem.Application.Features.Faculties.Dtos;
using JadaraITKnowledgeSystem.Application.Features.Faculties.Mappers;
using JadaraITKnowledgeSystem.Application.Interfaces;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JadaraITKnowledgeSystem.Application.Features.Faculties.Commands.UpdateFaculty;

public sealed class UpdateFacultyCommandHandler
    (IApplicationDbContext context, ILogger<UpdateFacultyCommandHandler> logger)
    : IRequestHandler<UpdateFacultyCommand, Result<FacultyDto>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly ILogger<UpdateFacultyCommandHandler> _logger = logger;

    public async Task<Result<FacultyDto>> Handle(
        UpdateFacultyCommand request,
        CancellationToken cancellationToken)
    {
        var faculty = await _context.Faculties
            .FirstOrDefaultAsync(f => f.Id == request.Id, cancellationToken);

        if (faculty is null)
        {
            _logger.LogWarning("Faculty with ID {FacultyId} not found.", request.Id);
            return Error.NotFound("Faculty.NotFound", $"Faculty with ID {request.Id} was not found.");
        }

        faculty.UpdateName(request.Name);
        faculty.UpdateUniversity(request.UniversityId);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Faculty {FacultyId} updated successfully.", request.Id);

        return faculty.ToDto();
    }
}
