using System.Threading;
using System.Threading.Tasks;
using JadaraITKnowledgeSystem.Application.Fetures.Dashboard.Queries.GetSystemStatistics;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace JadaraITKnowledgeSystem.API.Controllers;

[Route("api/[controller]")]
[ApiController]
//[Authorize(Roles = "SuperAdmin")] 
public class DashboardController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(IMediator mediator, ILogger<DashboardController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet("statistics")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetStatistics([FromQuery] int Months = 6, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("[Dashboard] Get statistics for last {Months} months", Months);

        var result = await _mediator.Send(new GetSystemStatisticsQuery(Months), cancellationToken);

        return result.Match<IActionResult>(
            onValue: dto => Ok(dto),
            onError: errors => BadRequest(new { errors })
        );
    }
}
