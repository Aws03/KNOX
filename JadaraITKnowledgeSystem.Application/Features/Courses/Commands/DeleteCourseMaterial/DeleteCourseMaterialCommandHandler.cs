using JadaraITKnowledgeSystem.Application.Interfaces;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JadaraITKnowledgeSystem.Application.Features.Courses.Commands.DeleteCourseMaterial;

public sealed class DeleteCourseMaterialCommandHandler
    (IApplicationDbContext context, ILogger<DeleteCourseMaterialCommandHandler> logger)
    : IRequestHandler<DeleteCourseMaterialCommand, Result<Success>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly ILogger<DeleteCourseMaterialCommandHandler> _logger = logger;

    public async Task<Result<Success>> Handle(
        DeleteCourseMaterialCommand request,
        CancellationToken cancellationToken)
    {
        var material = await _context.CourseMaterials
            .FirstOrDefaultAsync(m => m.Id == request.MaterialId, cancellationToken);

        if (material is null)
        {
            _logger.LogWarning("CourseMaterial with ID {MaterialId} not found.", request.MaterialId);
            return Error.NotFound("CourseMaterial.NotFound", $"Material with ID {request.MaterialId} was not found.");
        }

        _context.CourseMaterials.Remove(material);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("CourseMaterial {MaterialId} deleted successfully.", request.MaterialId);

        return Result.Success;
    }
}
