using JadaraITKnowledgeSystem.Application.Features.Courses.Commands.DeleteCourseMaterial;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JadaraITKnowledgeSystem.API.Controllers;

[Route("api/materials")]
[ApiController]
[Produces("application/json")]
public class MaterialsController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpDelete("{id}")]
    [Authorize(Roles = "Writer,Admin,SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        int id,
        CancellationToken cancellationToken = default)
    {
        var command = new DeleteCourseMaterialCommand(id);
        var result = await _mediator.Send(command, cancellationToken);

        return result.Match<IActionResult>(
            onValue: _ => NoContent(),
            onError: errors =>
            {
                var top = errors.FirstOrDefault();
                if (top.Code == "CourseMaterial.NotFound")
                    return NotFound(new { errors });
                return BadRequest(new { errors });
            });
    }
}
