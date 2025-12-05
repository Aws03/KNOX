using JadaraITKnowledgeSystem.Application.Common.Models;
using JadaraITKnowledgeSystem.Application.Fetures.Users.Dtos;
using JadaraITKnowledgeSystem.Application.Fetures.Users.Queries.GetUsers;
using JadaraITKnowledgeSystem.Application.Fetures.Users.Queries.GetUsersWithDetails;
using JadaraITKnowledgeSystem.Application.Fetures.Users.Commands.BlockUser;
using JadaraITKnowledgeSystem.Application.Fetures.Users.Commands.ActivateUser;
using JadaraITKnowledgeSystem.Application.Fetures.Identity.Queries.GetRoles;
using JadaraITKnowledgeSystem.Application.Fetures.Identity.Commands.AssignRole;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace JadaraITKnowledgeSystem.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class UsersController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    /// <summary>
    /// Retrieves a paginated list of users with optional filters.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetUsers(
        [FromQuery] int? universityId,
        [FromQuery] int? facultyId,
        [FromQuery] int? majorId,
        [FromQuery] string? email,
        [FromQuery] int? id,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new GetUsersQuery(universityId, facultyId, majorId, email, id, pageNumber, pageSize),
            cancellationToken);

        return result.Match<IActionResult>(
            onValue: users => Ok(users),
            onError: errors => BadRequest(new { errors })
        );
    }

    /// <summary>
    /// Retrieves a paginated list of users with detailed info and optional filters.
    /// </summary>
    [HttpGet("details")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetUsersWithDetails(
        [FromQuery] int? universityId,
        [FromQuery] int? facultyId,
        [FromQuery] int? majorId,
        [FromQuery] string? email,
        [FromQuery] int? id,
        [FromQuery] bool? isActive,
        [FromQuery] bool? isVerfied,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new GetUsersWithDetailsQuery(universityId, facultyId, majorId, email, id, isActive, isVerfied, pageNumber, pageSize),
            cancellationToken);

        return result.Match<IActionResult>(
            onValue: users => Ok(users),
            onError: errors => BadRequest(new { errors })
        );
    }

    /// <summary>
    /// Suspends (blocks) a user.
    /// </summary>
    [HttpPost("{id}/block")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> BlockUser(int id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new BlockUserCommand(id), cancellationToken);
        return result.Match<IActionResult>(
            onValue: _ => Ok(new { success = true }),
            onError: errors => NotFound(new { errors })
        );
    }

    /// <summary>
    /// Activates a user.
    /// </summary>
    [HttpPost("{id}/activate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ActivateUser(int id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new ActivateUserCommand(id), cancellationToken);
        return result.Match<IActionResult>(
            onValue: _ => Ok(new { success = true }),
            onError: errors => NotFound(new { errors })
        );
    }

    /// <summary>
    /// Gets available user roles.
    /// </summary>
    [HttpGet("roles")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRoles(CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetRolesQuery(), cancellationToken);
        return result.Match<IActionResult>(
            onValue: roles => Ok(roles),
            onError: errors => BadRequest(new { errors })
        );
    }

    /// <summary>
    /// Assigns a role to a user by domain user id.
    /// </summary>
    [HttpPost("{id}/assign-role")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AssignRole(int id, [FromBody] string roleName, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new AssignRoleToUserCommand(id, roleName), cancellationToken);
        return result.Match<IActionResult>(
            onValue: _ => Ok(new { success = true }),
            onError: errors => NotFound(new { errors })
        );
    }
}
