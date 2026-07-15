using System.Threading;
using System.Threading.Tasks;
using JadaraITKnowledgeSystem.Application.Features.Quizzes.Commands.GenerateQuizFromMaterial;
using JadaraITKnowledgeSystem.Application.Features.Quizzes.Dtos;
using JadaraITKnowledgeSystem.Application.Features.Quizzes.Queries.GetQuizGenerationJobStatus;
using JadaraITKnowledgeSystem.Application.Features.Quizzes.Queries.GetQuizGenerationJobsByMaterial;
using JadaraITKnowledgeSystem.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JadaraITKnowledgeSystem.API.Controllers;

[ApiController]
[Route("api/quiz-generation")]
[Authorize]
public class QuizGenerationController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUser;

    public QuizGenerationController(IMediator mediator, ICurrentUserService currentUser)
    {
        _mediator = mediator;
        _currentUser = currentUser;
    }

    /// <summary>
    /// Generate quiz from a course material document
    /// </summary>
    [HttpPost("materials/{materialId}")]
    [ProducesResponseType(typeof(QuizGenerationJobDto), StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GenerateQuizFromMaterial(
        int materialId,
        [FromBody] QuizGenerationOptionsDto options,
        CancellationToken cancellationToken)
    {
        var command = new GenerateQuizFromMaterialCommand(
            materialId,
            _currentUser.UserId ?? 0,
            options);

        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsError)
            return BadRequest(result.Errors);

        return AcceptedAtAction(
            nameof(GetJobStatus),
            new { jobId = result.Value.Id },
            result.Value);
    }

    /// <summary>
    /// Get the status of a quiz generation job
    /// </summary>
    [HttpGet("jobs/{jobId}")]
    [ProducesResponseType(typeof(QuizGenerationJobDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetJobStatus(int jobId, CancellationToken cancellationToken)
    {
        var query = new GetQuizGenerationJobStatusQuery(jobId);
        var result = await _mediator.Send(query, cancellationToken);

        if (result.IsError)
            return NotFound(result.Errors);

        return Ok(result.Value);
    }

    /// <summary>
    /// Get all quiz generation jobs for a specific material
    /// </summary>
    [HttpGet("materials/{materialId}/jobs")]
    [ProducesResponseType(typeof(System.Collections.Generic.List<QuizGenerationJobDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMaterialJobs(int materialId, CancellationToken cancellationToken)
    {
        var query = new GetQuizGenerationJobsByMaterialQuery(materialId);
        var result = await _mediator.Send(query, cancellationToken);

        if (result.IsError)
            return BadRequest(result.Errors);

        return Ok(result.Value);
    }
}
