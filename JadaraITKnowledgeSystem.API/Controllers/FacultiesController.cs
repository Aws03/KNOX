using JadaraITKnowledgeSystem.API.Contracts;
using JadaraITKnowledgeSystem.Application.Common.Models;
using JadaraITKnowledgeSystem.Application.Common.Security;
using JadaraITKnowledgeSystem.Application.Features.Faculties.Commands.CreateFaculty;
using JadaraITKnowledgeSystem.Application.Features.Faculties.Commands.UpdateFaculty;
using JadaraITKnowledgeSystem.Application.Features.Faculties.Dtos;
using JadaraITKnowledgeSystem.Application.Features.Faculties.Queries.GetFacultiesByUniversityId;
using JadaraITKnowledgeSystem.Application.Features.Faculties.Queries.GetFacultyById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JadaraITKnowledgeSystem.API.Controllers;

[Route("api/faculties")]
public sealed class FacultiesController(ISender sender) : ApiControllerBase(sender)
{
    /// <summary>Creates a faculty in a university.</summary>
    [HttpPost]
    [Authorize(Roles = Roles.AdminOrAbove)]
    [ProducesResponseType<FacultyDto>(StatusCodes.Status201Created)]
    public async Task<IActionResult> Create(CreateFacultyCommand command, CancellationToken cancellationToken) =>
        ResultOrProblem(await Sender.Send(command, cancellationToken),
            faculty => CreatedAtAction(nameof(GetById), new { id = faculty.Id }, faculty));

    /// <summary>Gets a faculty by id.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType<FacultyDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken) =>
        OkOrProblem(await Sender.Send(new GetFacultyByIdQuery(id), cancellationToken));

    /// <summary>Pages through a university's faculties; <c>name</c> filters by a part of the name.</summary>
    [HttpGet("by-university/{universityId:int}")]
    [ProducesResponseType<PaginatedList<FacultyDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByUniversityId(
        int universityId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? name = null, CancellationToken cancellationToken = default) =>
        OkOrProblem(await Sender.Send(new GetFacultiesByUniversityIdQuery(universityId, pageNumber, pageSize, name), cancellationToken));

    /// <summary>Renames a faculty.</summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = Roles.AdminOrAbove)]
    [ProducesResponseType<FacultyDto>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateFacultyRequest request, CancellationToken cancellationToken) =>
        OkOrProblem(await Sender.Send(new UpdateFacultyCommand(id, request.Name, request.UniversityId), cancellationToken));
}
