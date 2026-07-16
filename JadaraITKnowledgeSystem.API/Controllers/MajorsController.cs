using JadaraITKnowledgeSystem.Application.Features.Majors.Commands.CreateMajor;
using JadaraITKnowledgeSystem.Application.Features.Majors.Commands.UpdateMajor;
using JadaraITKnowledgeSystem.Application.Features.Majors.Queries.GetMajorByFacultyId;
using JadaraITKnowledgeSystem.Application.Features.Majors.Queries.GetMajorById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JadaraITKnowledgeSystem.API.Controllers;

[Route("api/majors")]
[ApiController]
[Produces("application/json")]

public class MajorsController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpPost]
    [Authorize(Roles = "SuperAdmin,Admin")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        CreateMajorCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);

        return result.Match<IActionResult>(
            onValue: major => CreatedAtAction(nameof(GetById), new { id = major.Id }, major),
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
            new GetMajorByIdQuery(id),
            cancellationToken);

        return result.Match<IActionResult>(
            onValue: major => Ok(major),
            onError: errors => NotFound(new { errors })
        );
    }

    [HttpGet("by-faculty/{facultyId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetByFacultyId(
        int facultyId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new GetMajorsByFacultyIdQuery(facultyId, pageNumber, pageSize),
            cancellationToken);

        return result.Match<IActionResult>(
            onValue: faculties => Ok(faculties),
            onError: errors => BadRequest(new { errors })
        );
    }

    public sealed record UpdateMajorRequest(string Name, int FacultyId);

    [HttpPut("{id}")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdateMajorRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new UpdateMajorCommand(id, request.Name, request.FacultyId);
        var result = await _mediator.Send(command, cancellationToken);

        return result.Match<IActionResult>(
            onValue: major => Ok(major),
            onError: errors =>
            {
                var top = errors.FirstOrDefault();
                if (top.Code == "Major.NotFound")
                    return NotFound(new { errors });
                return BadRequest(new { errors });
            });
    }
}
