using JadaraITKnowledgeSystem.Application.Fetures.Universities.Commands.CreateUniversity;
using JadaraITKnowledgeSystem.Application.Fetures.Universities.Queries.GetUniversityById;
using JadaraITKnowledgeSystem.Application.Fetures.Universities.Queries.GetUniversities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace JadaraITKnowledgeSystem.API.Controllers
{
    [ApiController]
    [Route("api/universities")]
    [Produces("application/json")]
    public class UniversitiesController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(
            CreateUniversityCommand command,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);

            return result.Match<IActionResult>(
                onValue: id => CreatedAtAction(nameof(GetById), new { id }, new { id }),
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
                new GetUniversityByIdQuery(id),
                cancellationToken);

            return result.Match<IActionResult>(
                onValue: university => Ok(university),
                onError: errors => NotFound(new { errors })
            );
        }

        /// <summary>
        /// Retrieves a paginated list of universities
        /// </summary>
        /// <param name="pageNumber">Page number (default 1)</param>
        /// <param name="pageSize">Page size (default 10)</param>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(
                new GetUniversitiesQuery(pageNumber, pageSize),
                cancellationToken);

            return result.Match<IActionResult>(
                onValue: universities => Ok(universities),
                onError: errors => BadRequest(new { errors })
            );
        }
    }
}
