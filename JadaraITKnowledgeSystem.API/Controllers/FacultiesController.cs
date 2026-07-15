using JadaraITKnowledgeSystem.Application.Features.Faculties.Commands.CreateFaculty;
using JadaraITKnowledgeSystem.Application.Features.Faculties.Queries.GetFacultiesByUniversityId;
using JadaraITKnowledgeSystem.Application.Features.Faculties.Queries.GetFacultyById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JadaraITKnowledgeSystem.API.Controllers;

[Route("api/faculties")]
[ApiController]
[Produces("application/json")]
public class FacultiesController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpPost]
    [Authorize(Roles = "SuperAdmin,Admin")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        CreateFacultyCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);

        return result.Match<IActionResult>(
            onValue: faculty => CreatedAtAction(nameof(GetById), new { id = faculty.Id }, faculty),
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
            new GetFacultyByIdQuery(id),
            cancellationToken);

        return result.Match<IActionResult>(
            onValue: faculty => Ok(faculty),
            onError: errors => NotFound(new { errors })
        );
    }

    [HttpGet("by-university/{universityId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetByUniversityId(
        int universityId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new GetFacultiesByUniversityIdQuery(universityId, pageNumber, pageSize),
            cancellationToken);

        return result.Match<IActionResult>(
            onValue: faculties => Ok(faculties),
            onError: errors => BadRequest(new { errors })
        );
    }

}
