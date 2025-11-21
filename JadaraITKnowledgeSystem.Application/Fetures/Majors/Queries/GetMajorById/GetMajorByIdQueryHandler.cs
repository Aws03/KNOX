using JadaraITKnowledgeSystem.Application.Fetures.Majors.Dtos;
using JadaraITKnowledgeSystem.Application.Fetures.Majors.Mappers;
using JadaraITKnowledgeSystem.Application.Interfaces;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JadaraITKnowledgeSystem.Application.Fetures.Majors.Queries.GetMajorById;

public sealed class GetMajorByIdQueryHandler
    (IApplicationDbContext context, ILogger<GetMajorByIdQueryHandler> logger)
    : IRequestHandler<GetMajorByIdQuery, Result<MajorDto>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly ILogger<GetMajorByIdQueryHandler> _logger = logger;

    public async Task<Result<MajorDto>> Handle(
        GetMajorByIdQuery request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Handling GetMajorByIdQuery: MajorId = {MajorId}",
            request.MajorId);

        var major = await _context.Majors
            .AsNoTracking()
            .Where(m => m.Id == request.MajorId)
            .Select(m => m.ToDto())
            .FirstOrDefaultAsync(cancellationToken);

        if (major is null)
        {
            return Error.NotFound(
                "Major.NotFound",
                $"No major found with ID {request.MajorId}");
        }

        return major;
    }
}
