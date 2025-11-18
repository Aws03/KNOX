using JadaraITKnowledgeSystem.Application.Fetures.Courses.Commands.CreateCourse;
using JadaraITKnowledgeSystem.Application.Fetures.Courses.Queries.GetCourseById;
using JadaraITKnowledgeSystem.Application.Fetures.Courses.Queries.GetCoursesByMajorId;
using JadaraITKnowledgeSystem.Application.Fetures.Faculties.Queries.GetFacultiesByUniversityId;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace JadaraITKnowledgeSystem.API.Controllers
{
    [Route("api/courses")]
    [ApiController]
    [Produces("application/json")]

    public class CoursesController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(
            CreateCourseCommand command,
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
                new GetCourseByIdQuery(id),
                cancellationToken);

            return result.Match<IActionResult>(
                onValue: course => Ok(course),
                onError: errors => NotFound(new { errors })
            );
        }

        [HttpGet("by-major/{majorId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetByMajorId(
            int majorId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(
                new GetCoursesByMajorIdQuery(majorId, pageNumber, pageSize),
                cancellationToken);

            return result.Match<IActionResult>(
                onValue: courses => Ok(courses),
                onError: errors => BadRequest(new { errors })
            );
        }
    }
}
