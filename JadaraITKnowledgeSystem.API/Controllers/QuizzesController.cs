using JadaraITKnowledgeSystem.Application.Fetures.Quizzes.Commands.AddReaction;
using JadaraITKnowledgeSystem.Application.Fetures.Quizzes.Commands.CreateQuiz;
using JadaraITKnowledgeSystem.Application.Fetures.Quizzes.Dtos;
using JadaraITKnowledgeSystem.Application.Fetures.Quizzes.Queries.GetQuizById;
using JadaraITKnowledgeSystem.Application.Fetures.Quizzes.Queries.GetQuizzes;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace JadaraITKnowledgeSystem.API.Controllers;

[Route("api/quizzes")]
[ApiController]
[Produces("application/json")]
public class QuizzesController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        CreateQuizCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);

        return result.Match<IActionResult>(
            onValue: quiz => CreatedAtAction(nameof(GetById), new { id = quiz.Id }, quiz),
            onError: errors => BadRequest(new { errors })
        );
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetQuizByIdQuery(id),
            cancellationToken);

        return result.Match<IActionResult>(
            onValue: quiz => Ok(quiz),
            onError: errors => NotFound(new { errors })
        );
    }

    [HttpGet("by-course/{courseId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetByCourseId(
        int courseId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] int? userId = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetQuizzesByCourseIdQuery(
            CourseId: courseId,
            UserId: userId,
            PageNumber: pageNumber,
            PageSize: pageSize);

        var result = await _mediator.Send(query, cancellationToken);

        return result.Match<IActionResult>(
            onValue: quizzes => Ok(quizzes),
            onError: errors => BadRequest(new { errors })
        );
    }


    [HttpPost("{quizId}/reactions")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddReaction(
    int quizId,
    [FromBody] AddReactionDto dto,
    CancellationToken cancellationToken)
    {
        var command = new AddReactionCommand(
            quizId,
            dto.UserId,
            dto.ReactionType
        );

        var result = await _mediator.Send(command, cancellationToken);

        return result.Match<IActionResult>(
            onValue: result => Ok(result),
            onError: errors => BadRequest(new { errors })
        );
    }

}
