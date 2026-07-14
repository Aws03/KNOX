using JadaraITKnowledgeSystem.Application.Features.Faculties.Dtos;
using JadaraITKnowledgeSystem.Application.Features.Faculties.Mappers;
using JadaraITKnowledgeSystem.Application.Features.Universities.Commands.CreateUniversity;
using JadaraITKnowledgeSystem.Application.Interfaces;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using JadaraITKnowledgeSystem.Domain.Universities.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace JadaraITKnowledgeSystem.Application.Features.Faculties.Commands.CreateFaculty;

public sealed class CreateFacultyCommandHandler
    (IApplicationDbContext applicationDbContext, ILogger<CreateUniversityCommandHandler> logger)
    : IRequestHandler<CreateFacultyCommand, Result<FacultyDto>>
{
    private readonly IApplicationDbContext _context = applicationDbContext;
    private readonly ILogger<CreateUniversityCommandHandler> _logger = logger;

    public async Task<Result<FacultyDto>> Handle(CreateFacultyCommand request, CancellationToken ct)
    {
        _logger.LogInformation("Handling CreateFacultyCommand for UniversityId: {UniversityId}, Name: {Name}", request.UniversityId, request.Name);

        var faculty = Faculty.Create(request.Name, request.UniversityId);

        if (!faculty.IsSuccess)
        {
            _logger.LogWarning("Failed to create Faculty: {Errors}", string.Join(", ", faculty.Errors));
            return faculty.Errors;
        }

        await _context.Faculties.AddAsync(faculty.Value, ct);
        await _context.SaveChangesAsync(ct);

        var facultyDto = faculty.Value.ToDto();

        _logger.LogInformation("Successfully created Faculty with Id: {FacultyId}  , {Name}", facultyDto.Id, facultyDto.Name);

        return facultyDto;
    }
}
