using JadaraITKnowledgeSystem.Application.Common.Security;
using JadaraITKnowledgeSystem.Application.Features.Courses.Commands.DeleteFolder;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JadaraITKnowledgeSystem.API.Controllers;

[Route("api/folders")]
public sealed class FoldersController(ISender sender) : ApiControllerBase(sender)
{
    /// <summary>Deletes a folder; with deleteContents=true sub-folders go too and materials move to the course root.</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = Roles.WriterOrAbove)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, [FromQuery] bool deleteContents = false, CancellationToken cancellationToken = default) =>
        NoContentOrProblem(await Sender.Send(new DeleteFolderCommand(id, deleteContents), cancellationToken));
}
