using JadaraITKnowledgeSystem.Application.Fetures.Faculties.Dtos;
using JadaraITKnowledgeSystem.Application.Fetures.Faculties.Mappers;
using JadaraITKnowledgeSystem.Application.Interfaces;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JadaraITKnowledgeSystem.Application.Fetures.Faculties.Queries.GetFacultyById;

public sealed class GetFacultyByIdQueryHandler(IApplicationDbContext context, ILogger<GetFacultyByIdQueryHandler> logger)
    : IRequestHandler<GetFacultyByIdQuery, Result<FacultyDto>>
{

    private readonly ILogger<GetFacultyByIdQueryHandler> _logger = logger;
    private readonly IApplicationDbContext _context = context;

    public async Task<Result<FacultyDto>> Handle
        (GetFacultyByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling CreateFacultyCommand for UniversityId: {UniversityId}", request.facultyId);

        var faculty = await _context.Faculties
           .AsNoTracking()
           .FirstOrDefaultAsync(u => u.Id == request.facultyId, cancellationToken);

        if (faculty == null)
        {
            // TODO : use application error insted of domain error later ...
            _logger.LogWarning("Faculty with ID {FacultyId} not found.", request.facultyId);
            return Error.NotFound(
                "Faculty.NotFound",
                $"Faculty with ID {request.facultyId} was not found.");
        }

        var dto = faculty.ToDto();

        _logger.LogInformation("Faculty with ID {FacultyId} retrieved successfully.", request.facultyId);

        return dto;
    }
}
