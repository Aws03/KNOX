using JadaraITKnowledgeSystem.Application.Fetures.Faculties.Commands.CreateFaculty;
using JadaraITKnowledgeSystem.Application.Fetures.Faculties.Queries.GetFacultiesByUniversityId;
using JadaraITKnowledgeSystem.Application.Fetures.Faculties.Queries.GetFacultyById;
using JadaraITKnowledgeSystem.Application.Fetures.Majors.Commands.CreateMajor;
using JadaraITKnowledgeSystem.Application.Fetures.Majors.Queries.GetMajorByFacultyId;
using JadaraITKnowledgeSystem.Application.Fetures.Majors.Queries.GetMajorById;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JadaraITKnowledgeSystem.API.Controllers
{
    [Route("api/majors")]
    [ApiController]
    [Produces("application/json")]

    public class MajorsController(IMediator mediator) : ControllerBase
    {
        private readonly IMediator _mediator = mediator;

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(
            CreateMajorCommand command,
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
                new GetMajorByIdQuery(id),
                cancellationToken);

            return result.Match<IActionResult>(
                onValue: major => Ok(major),
                onError: errors => NotFound(new { errors })
            );
        }

        [HttpGet("by-faculty/{facultyId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetByFacultyId(
            int facultyId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(
                new GetMajorsByFacultyIdQuery(facultyId, pageNumber, pageSize),
                cancellationToken);

            return result.Match<IActionResult>(
                onValue: faculties => Ok(faculties),
                onError: errors => BadRequest(new { errors })
            );
        }
    }
}
