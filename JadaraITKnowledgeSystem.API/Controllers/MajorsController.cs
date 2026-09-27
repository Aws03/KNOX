using JadaraITKnowledgeSystem.API.Contracts;
using JadaraITKnowledgeSystem.Application.Common.Models;
using JadaraITKnowledgeSystem.Application.Common.Security;
using JadaraITKnowledgeSystem.Application.Features.Majors.Commands.CreateMajor;
using JadaraITKnowledgeSystem.Application.Features.Majors.Commands.UpdateMajor;
using JadaraITKnowledgeSystem.Application.Features.Majors.Dtos;
using JadaraITKnowledgeSystem.Application.Features.Majors.Queries.GetMajorById;
using JadaraITKnowledgeSystem.Application.Features.Majors.Queries.GetMajorsByFacultyId;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JadaraITKnowledgeSystem.API.Controllers;

[Route("api/majors")]
public sealed class MajorsController(ISender sender) : ApiControllerBase(sender)
{
    [HttpPost]
    [Authorize(Roles = Roles.AdminOrAbove)]
    [ProducesResponseType<MajorDto>(StatusCodes.Status201Created)]
    public async Task<IActionResult> Create(CreateMajorCommand command, CancellationToken cancellationToken) =>
        ResultOrProblem(await Sender.Send(command, cancellationToken),
            major => CreatedAtAction(nameof(GetById), new { id = major.Id }, major));

    [HttpGet("{id:int}")]
    [ProducesResponseType<MajorDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken) =>
        OkOrProblem(await Sender.Send(new GetMajorByIdQuery(id), cancellationToken));

    [HttpGet("by-faculty/{facultyId:int}")]
    [ProducesResponseType<PaginatedList<MajorDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByFacultyId(
        int facultyId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default) =>
        OkOrProblem(await Sender.Send(new GetMajorsByFacultyIdQuery(facultyId, pageNumber, pageSize), cancellationToken));

    [HttpPut("{id:int}")]
    [Authorize(Roles = Roles.AdminOrAbove)]
    [ProducesResponseType<MajorDto>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateMajorRequest request, CancellationToken cancellationToken) =>
        OkOrProblem(await Sender.Send(new UpdateMajorCommand(id, request.Name, request.FacultyId), cancellationToken));
}
