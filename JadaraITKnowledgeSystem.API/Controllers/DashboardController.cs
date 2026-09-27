using JadaraITKnowledgeSystem.Application.Common.Security;
using JadaraITKnowledgeSystem.Application.Features.Dashboard.Dtos;
using JadaraITKnowledgeSystem.Application.Features.Dashboard.Queries.GetSystemStatistics;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JadaraITKnowledgeSystem.API.Controllers;

[Route("api/dashboard")]
[Authorize(Roles = Roles.SuperAdmin)]
public sealed class DashboardController(ISender sender) : ApiControllerBase(sender)
{
    /// <summary>Platform statistics with monthly growth for the last `months` months.</summary>
    [HttpGet("statistics")]
    [ProducesResponseType<SystemStatisticsDto>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStatistics([FromQuery] int months = 6, CancellationToken cancellationToken = default) =>
        OkOrProblem(await Sender.Send(new GetSystemStatisticsQuery(months), cancellationToken));
}
