using JadaraITKnowledgeSystem.Application.Common.Security;
using JadaraITKnowledgeSystem.Application.Features.Quizzes.Commands.GenerateQuizFromMaterial;
using JadaraITKnowledgeSystem.Application.Features.Quizzes.Dtos;
using JadaraITKnowledgeSystem.Application.Features.Quizzes.Queries.GetQuizGenerationJobStatus;
using JadaraITKnowledgeSystem.Application.Features.Quizzes.Queries.GetQuizGenerationJobsByMaterial;
using JadaraITKnowledgeSystem.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JadaraITKnowledgeSystem.API.Controllers;

[Route("api/quiz-generation")]
[Authorize(Roles = Roles.WriterOrAbove)]
public sealed class QuizGenerationController(ISender sender, ICurrentUserService currentUser) : ApiControllerBase(sender)
{
    /// <summary>Starts AI quiz generation for a PDF/DOCX/PPTX material; poll the returned job.</summary>
    [HttpPost("materials/{materialId:int}")]
    [ProducesResponseType<QuizGenerationJobDto>(StatusCodes.Status202Accepted)]
    public async Task<IActionResult> GenerateQuizFromMaterial(
        int materialId, [FromBody] QuizGenerationOptionsDto options, CancellationToken cancellationToken) =>
        ResultOrProblem(
            await Sender.Send(new GenerateQuizFromMaterialCommand(materialId, currentUser.DomainUserId ?? 0, options), cancellationToken),
            job => AcceptedAtAction(nameof(GetJobStatus), new { jobId = job.Id }, job));

    /// <summary>Status of a quiz generation job (Pending, Extracting, GeneratingQuizzes, Completed, Failed).</summary>
    [HttpGet("jobs/{jobId:int}")]
    [ProducesResponseType<QuizGenerationJobDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetJobStatus(int jobId, CancellationToken cancellationToken) =>
        OkOrProblem(await Sender.Send(new GetQuizGenerationJobStatusQuery(jobId), cancellationToken));

    /// <summary>All quiz generation jobs for a material, newest first.</summary>
    [HttpGet("materials/{materialId:int}/jobs")]
    [ProducesResponseType<List<QuizGenerationJobDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMaterialJobs(int materialId, CancellationToken cancellationToken) =>
        OkOrProblem(await Sender.Send(new GetQuizGenerationJobsByMaterialQuery(materialId), cancellationToken));
}
