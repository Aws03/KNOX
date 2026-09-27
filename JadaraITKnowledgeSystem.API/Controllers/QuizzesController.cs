using JadaraITKnowledgeSystem.API.Contracts;
using JadaraITKnowledgeSystem.Application.Common.Models;
using JadaraITKnowledgeSystem.Application.Common.Security;
using JadaraITKnowledgeSystem.Application.Features.Quizzes.Commands.AddReaction;
using JadaraITKnowledgeSystem.Application.Features.Quizzes.Commands.CreateQuiz;
using JadaraITKnowledgeSystem.Application.Features.Quizzes.Commands.SubmitQuizAttempt;
using JadaraITKnowledgeSystem.Application.Features.Quizzes.Dtos;
using JadaraITKnowledgeSystem.Application.Features.Quizzes.Queries.GetQuizById;
using JadaraITKnowledgeSystem.Application.Features.Quizzes.Queries.GetQuizzesByCourseId;
using JadaraITKnowledgeSystem.Application.Features.Quizzes.Queries.GetQuizzesByWriterId;
using JadaraITKnowledgeSystem.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JadaraITKnowledgeSystem.API.Controllers;

[Route("api/quizzes")]
public sealed class QuizzesController(ISender sender, ICurrentUserService currentUser) : ApiControllerBase(sender)
{
    /// <summary>Creates a quiz authored by the caller.</summary>
    [HttpPost]
    [Authorize(Roles = Roles.WriterOrAbove)]
    [ProducesResponseType<QuizDto>(StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateQuizRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateQuizCommand(
            request.Title, currentUser.DomainUserId ?? 0, request.CourseId, request.Description, request.Questions, request.Tags);

        return ResultOrProblem(await Sender.Send(command, cancellationToken),
            quiz => CreatedAtAction(nameof(GetById), new { id = quiz.Id }, quiz));
    }

    /// <summary>Gets a quiz with its questions and choices.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType<QuizDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken) =>
        OkOrProblem(await Sender.Send(new GetQuizByIdQuery(id), cancellationToken));

    /// <summary>Quizzes of a course; for a signed-in caller each item carries their last score.</summary>
    [HttpGet("by-course/{courseId:int}")]
    [ProducesResponseType<PaginatedList<QuizSummaryDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByCourseId(
        int courseId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default) =>
        OkOrProblem(await Sender.Send(new GetQuizzesByCourseIdQuery(courseId, pageNumber, pageSize), cancellationToken));

    /// <summary>Quizzes of a course written by the caller.</summary>
    [HttpGet("by-course/{courseId:int}/my-quizzes")]
    [Authorize(Roles = Roles.WriterOrAbove)]
    [ProducesResponseType<PaginatedList<QuizSummaryDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByCourseIdAndWriter(
        int courseId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default) =>
        OkOrProblem(await Sender.Send(new GetQuizzesByWriterIdQuery(courseId, pageNumber, pageSize), cancellationToken));

    /// <summary>Likes or dislikes a quiz, or switches the caller's reaction; repeating the same reaction is a 409 Conflict.</summary>
    [HttpPost("{quizId:int}/reactions")]
    [Authorize]
    [ProducesResponseType<ReactionResultDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AddReaction(int quizId, [FromBody] AddReactionDto dto, CancellationToken cancellationToken) =>
        OkOrProblem(await Sender.Send(new AddReactionCommand(quizId, dto.ReactionType), cancellationToken));

    /// <summary>Records (or replaces) the caller's score for a quiz.</summary>
    [HttpPost("{quizId:int}/attempts")]
    [Authorize]
    [ProducesResponseType<QuizAttemptDto>(StatusCodes.Status200OK)]
    public async Task<IActionResult> SubmitAttempt(int quizId, [FromBody] SubmitQuizAttemptDto dto, CancellationToken cancellationToken) =>
        OkOrProblem(await Sender.Send(new SubmitQuizAttemptCommand(quizId, dto.Score), cancellationToken));
}
