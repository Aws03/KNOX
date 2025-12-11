using JadaraITKnowledgeSystem.Application.Fetures.Courses.Commands.CreateCourse;
using JadaraITKnowledgeSystem.Application.Fetures.Courses.Commands.CreateCourseMaterial;
using JadaraITKnowledgeSystem.Application.Fetures.Courses.Commands.CreateFolder;
using JadaraITKnowledgeSystem.Application.Fetures.Courses.Queries.GetCourseById;
using JadaraITKnowledgeSystem.Application.Fetures.Courses.Queries.GetCoursesByMajorId;
using JadaraITKnowledgeSystem.Application.Fetures.Courses.Queries.GetCourseContentByFolder;
using JadaraITKnowledgeSystem.Domain.Courses.Enums; // added for enums
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace JadaraITKnowledgeSystem.API.Controllers;

[Route("api/courses")]
[ApiController]
[Produces("application/json")]
public class CoursesController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        CreateCourseCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);

        return result.Match<IActionResult>(
            onValue: course => CreatedAtAction(nameof(GetById), new { id = course.Id }, course),
            onError: errors => BadRequest(new { errors })
        );
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetCourseByIdQuery(id),
            cancellationToken);

        return result.Match<IActionResult>(
            onValue: course => Ok(course),
            onError: errors => NotFound(new { errors })
        );
    }

    [HttpGet("by-major/{majorId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetByMajorId(
        int majorId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] RequirementNature? requirementNature = null,
        [FromQuery] RequirementType? requirementType = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new GetCoursesByMajorIdQuery(majorId, pageNumber, pageSize, requirementNature, requirementType),
            cancellationToken);

        return result.Match<IActionResult>(
            onValue: summaries => Ok(summaries),
            onError: errors => BadRequest(new { errors })
        );
    }

    // ===== Course Materials =====
    public sealed record CreateMaterialRequest(string Title, string ContemtUrl, int? FolderId, string? Description);

    [HttpPost("{courseId}/materials")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateMaterial(
        int courseId,
        [FromBody] CreateMaterialRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new CreateCourseMaterialCommand(
            request.Title,
            request.ContemtUrl,
            courseId,
            request.FolderId,
            request.Description);

        var result = await _mediator.Send(command, cancellationToken);

        return result.Match<IActionResult>(
            onValue: material => CreatedAtAction(nameof(GetById), new { id = material.CourseId }, material),
            onError: errors => BadRequest(new { errors })
        );
    }

    // ===== Folders =====
    public sealed record CreateFolderRequest(string Name, int? ParentFolderId, string? Description);

    [HttpPost("{courseId}/folders")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateFolder(
        int courseId,
        [FromBody] CreateFolderRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new CreateFolderCommand(
            request.Name,
            courseId,
            request.ParentFolderId,
            request.Description);

        var result = await _mediator.Send(command, cancellationToken);

        return result.Match<IActionResult>(
            onValue: folder => CreatedAtAction(nameof(GetById), new { id = folder.CourseId }, folder),
            onError: errors => BadRequest(new { errors })
        );
    }

    // ===== Course Contents (folders + materials at a level) =====
    [HttpGet("{courseId}/contents")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetContents(
        int courseId,
        [FromQuery] int? folderId,
        CancellationToken cancellationToken = default)
    {
        var query = new GetCourseContentsQuery(courseId, folderId);
        var result = await _mediator.Send(query, cancellationToken);

        return result.Match<IActionResult>(
            onValue: contents => Ok(contents),
            onError: errors =>
            {
                var top = errors.FirstOrDefault();
                if (top.Code == "Course.NotFound")
                    return NotFound(new { errors });
                return BadRequest(new { errors });
            });
    }
}
