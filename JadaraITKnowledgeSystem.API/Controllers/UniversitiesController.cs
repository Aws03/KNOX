using JadaraITKnowledgeSystem.API.Contracts;
using JadaraITKnowledgeSystem.Application.Common.Models;
using JadaraITKnowledgeSystem.Application.Common.Security;
using JadaraITKnowledgeSystem.Application.Features.Universities.Commands.CreateUniversity;
using JadaraITKnowledgeSystem.Application.Features.Universities.Commands.UpdateUniversity;
using JadaraITKnowledgeSystem.Application.Features.Universities.Dtos;
using JadaraITKnowledgeSystem.Application.Features.Universities.Queries.GetUniversities;
using JadaraITKnowledgeSystem.Application.Features.Universities.Queries.GetUniversityById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JadaraITKnowledgeSystem.API.Controllers;

[Route("api/universities")]
public sealed class UniversitiesController(ISender sender) : ApiControllerBase(sender)
{
    /// <summary>Creates a university.</summary>
    [HttpPost]
    [Authorize(Roles = Roles.AdminOrAbove)]
    [ProducesResponseType<UniversityDto>(StatusCodes.Status201Created)]
    public async Task<IActionResult> Create(CreateUniversityCommand command, CancellationToken cancellationToken) =>
        ResultOrProblem(await Sender.Send(command, cancellationToken),
            university => CreatedAtAction(nameof(GetById), new { id = university.Id }, university));

    /// <summary>Gets a university by id.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType<UniversityDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken) =>
        OkOrProblem(await Sender.Send(new GetUniversityByIdQuery(id), cancellationToken));

    /// <summary>Pages through universities; <c>name</c> filters by a part of the name.</summary>
    [HttpGet]
    [ProducesResponseType<PaginatedList<UniversityDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? name = null, CancellationToken cancellationToken = default) =>
        OkOrProblem(await Sender.Send(new GetUniversitiesQuery(pageNumber, pageSize, name), cancellationToken));

    /// <summary>Renames a university.</summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = Roles.AdminOrAbove)]
    [ProducesResponseType<UniversityDto>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateUniversityRequest request, CancellationToken cancellationToken) =>
        OkOrProblem(await Sender.Send(new UpdateUniversityCommand(id, request.Name), cancellationToken));
}
