using JadaraITKnowledgeSystem.Application.Common.Security;
using JadaraITKnowledgeSystem.Application.Features.Courses.Commands.DeleteCourseMaterial;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JadaraITKnowledgeSystem.API.Controllers;

[Route("api/materials")]
public sealed class MaterialsController(ISender sender) : ApiControllerBase(sender)
{
    /// <summary>Deletes a material; its file is removed from storage once the deletion is committed.</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = Roles.WriterOrAbove)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken) =>
        NoContentOrProblem(await Sender.Send(new DeleteCourseMaterialCommand(id), cancellationToken));
}
