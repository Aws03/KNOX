using JadaraITKnowledgeSystem.Application.Features.Majors.Dtos;
using JadaraITKnowledgeSystem.Application.Features.Majors.Mappers;
using JadaraITKnowledgeSystem.Application.Features.Universities.Commands.CreateUniversity;
using JadaraITKnowledgeSystem.Application.Features.Universities.Mappers;
using JadaraITKnowledgeSystem.Application.Interfaces;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using JadaraITKnowledgeSystem.Domain.Universities.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace JadaraITKnowledgeSystem.Application.Features.Majors.Commands.CreateMajor;

public sealed class CreateMajorCommandHandler
    (IApplicationDbContext context, ILogger<CreateUniversityCommandHandler> logger) : IRequestHandler<CreateMajorCommand, Result<MajorDto>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly ILogger<CreateUniversityCommandHandler> _logger = logger;
    public async Task<Result<MajorDto>> Handle(CreateMajorCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating a new major with name: {MajorName} , facultyId: {FacultyId}", request.Name, request.FacultyId);

        var major = Major.Create(request.Name, request.FacultyId);

        if (major.IsError)
        {
            _logger.LogWarning("Failed to create major. Errors: {Errors}", string.Join(", ", major.Errors));
            return major.Errors;
        }

        await _context.Majors.AddAsync(major.Value);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully created major with ID: {MajorId}", major.Value.Id);

        var majorDto = major.Value.ToDto();
        return majorDto;
    }
}
