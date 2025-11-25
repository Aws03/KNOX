using JadaraITKnowledgeSystem.Application.DTOs;
using JadaraITKnowledgeSystem.Application.Fetures.Courses.Mappers;
using JadaraITKnowledgeSystem.Application.Interfaces;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using JadaraITKnowledgeSystem.Domain.Courses.Entites;
using MediatR;
using Microsoft.Extensions.Logging;


namespace JadaraITKnowledgeSystem.Application.Fetures.Courses.Commands.CreateCourseMaterial;

public sealed class CreateCourseMaterialCommandHandler(
IApplicationDbContext context,ILogger<CreateCourseMaterialCommandHandler> logger)
: IRequestHandler<CreateCourseMaterialCommand, Result<CourseMaterialDto>>
{

    private readonly ILogger<CreateCourseMaterialCommandHandler> _logger = logger;
    private readonly IApplicationDbContext _context = context;
    public async Task<Result<CourseMaterialDto>> Handle(
        CreateCourseMaterialCommand request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Adding Course material -> courseId : {courseId}",
            request.CourseId);

        var entityResult = CourseMaterial.Create(
            request.Title,
            request.ContemtUrl,
            request.CourseId,
            request.FolderId,
            request.Description);

        if (entityResult.IsError)
        {
            _logger.LogWarning("Error happen while attempt to adding course material --> courseId : {courseId}, errors : {Errors}",
                request.CourseId, entityResult.Errors);
            return entityResult.Errors;
        }

        var entity = entityResult.Value;

        _context.CourseMaterials.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return entity.ToDto();
    }
}
