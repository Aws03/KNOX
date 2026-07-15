using JadaraITKnowledgeSystem.Application.Common.Models;
using JadaraITKnowledgeSystem.Application.Features.Users.Dtos;
using JadaraITKnowledgeSystem.Application.Features.Users.Queries.GetUsers;
using JadaraITKnowledgeSystem.Application.Features.Users.Queries.GetUsersWithDetails;
using JadaraITKnowledgeSystem.Application.Features.Users.Commands.BlockUser;
using JadaraITKnowledgeSystem.Application.Features.Users.Commands.ActivateUser;
using JadaraITKnowledgeSystem.Application.Features.Users.Commands.UpdateUserProfile;
using JadaraITKnowledgeSystem.Application.Features.Users.Commands.UpdateProfilePicture;
using JadaraITKnowledgeSystem.Application.Features.Users.Commands.DeleteProfilePicture;
using JadaraITKnowledgeSystem.Application.Features.Identity.Queries.GetRoles;
using JadaraITKnowledgeSystem.Application.Features.Identity.Commands.AssignRole;
using JadaraITKnowledgeSystem.Application.Features.Users.Queries.GetCurrentUserProfile;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JadaraITKnowledgeSystem.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Produces("application/json")]
public class UsersController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    /// <summary>
    /// Returns the current authenticated user's full profile.
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMe(CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetCurrentUserProfileQuery(), cancellationToken);
        return result.Match<IActionResult>(
            onValue: profile => Ok(profile),
            onError: errors =>
            {
                var top = errors.FirstOrDefault();
                if (top != null && top.Code == "Auth.Unauthorized") return Unauthorized(new { message = top.Description });
                return BadRequest(new { errors });
            }
        );
    }

    /// <summary>
    /// Updates the current authenticated user's profile (full name and/or major).
    /// </summary>
    [HttpPut("me")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateProfile(
        [FromBody] UpdateProfileRequest request,
        CancellationToken cancellationToken = default)
    {
        var command = new UpdateUserProfileCommand(request.FullName, request.MajorId);
        var result = await _mediator.Send(command, cancellationToken);

        return result.Match<IActionResult>(
            onValue: profile => Ok(profile),
            onError: errors =>
            {
                var top = errors.FirstOrDefault();
                if (top == null) return BadRequest(new { errors });
                
                return top.Code switch
                {
                    "User.NotAuthenticated" => Unauthorized(new { errors }),
                    "User.NotFound" or "Major.NotFound" => NotFound(new { errors }),
                    _ => BadRequest(new { errors })
                };
            }
        );
    }

    /// <summary>
    /// Uploads or updates the current user's profile picture.
    /// </summary>
    [HttpPost("me/profile-picture")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateProfilePicture(
        IFormFile image,
        CancellationToken cancellationToken = default)
    {
        if (image == null || image.Length == 0)
            return BadRequest(new { errors = new[] { new { code = "Image.Required", description = "Image file is required." } } });

        using var stream = image.OpenReadStream();
        var command = new UpdateProfilePictureCommand(stream, image.FileName);
        var result = await _mediator.Send(command, cancellationToken);

        return result.Match<IActionResult>(
            onValue: profile => Ok(profile),
            onError: errors =>
            {
                var top = errors.FirstOrDefault();
                if (top == null) return BadRequest(new { errors });
                
                return top.Code switch
                {
                    "User.NotAuthenticated" => Unauthorized(new { errors }),
                    "User.NotFound" => NotFound(new { errors }),
                    _ => BadRequest(new { errors })
                };
            }
        );
    }

    /// <summary>
    /// Deletes the current user's profile picture.
    /// </summary>
    [HttpDelete("me/profile-picture")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteProfilePicture(CancellationToken cancellationToken = default)
    {
        var command = new DeleteProfilePictureCommand();
        var result = await _mediator.Send(command, cancellationToken);

        return result.Match<IActionResult>(
            onValue: _ => NoContent(),
            onError: errors =>
            {
                var top = errors.FirstOrDefault();
                if (top == null) return BadRequest(new { errors });
                
                return top.Code switch
                {
                    "User.NotAuthenticated" => Unauthorized(new { errors }),
                    "User.NotFound" or "ProfilePicture.NotFound" => NotFound(new { errors }),
                    _ => BadRequest(new { errors })
                };
            }
        );
    }

    /// <summary>
    /// Retrieves a paginated list of users with optional filters.
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "SuperAdmin,Admin")]
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
    [Authorize(Roles = "SuperAdmin,Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetUsersWithDetails(
        [FromQuery] int? universityId,
        [FromQuery] int? facultyId,
        [FromQuery] int? majorId,
        [FromQuery] string? email,
        [FromQuery] int? id,
        [FromQuery] bool? isActive,
        [FromQuery] bool? isVerified,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new GetUsersWithDetailsQuery(universityId, facultyId, majorId, email, id, isActive, isVerified, pageNumber, pageSize),
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
    [Authorize(Roles = "SuperAdmin,Admin")]
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
    [Authorize(Roles = "SuperAdmin,Admin")]
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
    [Authorize(Roles = "SuperAdmin,Admin")]
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
    [Authorize(Roles = "SuperAdmin,Admin")]
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

    /// <summary>
    /// Returns statistics for a writer (for dashboard).
    /// </summary>
    [HttpGet("{id}/writer-statistics")]
    [Authorize(Roles = "Writer,Admin,SuperAdmin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetWriterStatistics(int id, CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new Application.Features.Users.Queries.GetWriterStatistics.GetWriterStatisticsQuery(id), cancellationToken);
        if (result == null)
            return NotFound();
        return Ok(result);
    }

    public sealed record UpdateProfileRequest(string? FullName = null, int? MajorId = null);
}
