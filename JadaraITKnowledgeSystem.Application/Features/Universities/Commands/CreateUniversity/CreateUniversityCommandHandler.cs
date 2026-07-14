using JadaraITKnowledgeSystem.Application.Features.Universities.Dtos;
using JadaraITKnowledgeSystem.Application.Features.Universities.Mappers;
using JadaraITKnowledgeSystem.Application.Interfaces;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using JadaraITKnowledgeSystem.Domain.Universities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace JadaraITKnowledgeSystem.Application.Features.Universities.Commands.CreateUniversity;

public sealed class CreateUniversityCommandHandler(IApplicationDbContext applicationDbContext, ILogger<CreateUniversityCommandHandler> logger)
    : IRequestHandler<CreateUniversityCommand, Result<UniversityDto>>
{

    private readonly IApplicationDbContext _applicationDbContext = applicationDbContext;
    private readonly ILogger<CreateUniversityCommandHandler> _logger = logger;

    public async Task<Result<UniversityDto>> Handle(CreateUniversityCommand request, CancellationToken ct)
    {

        _logger.LogInformation("Creating a new university with name: {UniversityName}", request.Name);

        var university = University.Create(request.Name);

        if(!university.IsSuccess)
        {
            _logger.LogWarning("Could not create University Object with name {UniversityName} , erros : {Errors}",
                request.Name,university.Errors.ToString());
            return university.Errors;
        }

        _applicationDbContext.Universities.Add(university.Value);
        await _applicationDbContext.SaveChangesAsync(ct);

        _logger.LogInformation("{UniversityName} University created successfully with ID: {UniversityId}",
            request.Name, university.Value.Id);

        var universityDto = university.Value.ToDto();
        return universityDto;
    }
}
