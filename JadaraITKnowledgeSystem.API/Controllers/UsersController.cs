using JadaraITKnowledgeSystem.API.Contracts;
using JadaraITKnowledgeSystem.Application.Common.Models;
using JadaraITKnowledgeSystem.Application.Common.Security;
using JadaraITKnowledgeSystem.Application.Features.Identity.Commands.AssignRole;
using JadaraITKnowledgeSystem.Application.Features.Identity.Queries.GetRoles;
using JadaraITKnowledgeSystem.Application.Features.Users.Commands.ActivateUser;
using JadaraITKnowledgeSystem.Application.Features.Users.Commands.BlockUser;
using JadaraITKnowledgeSystem.Application.Features.Users.Commands.DeleteProfilePicture;
using JadaraITKnowledgeSystem.Application.Features.Users.Commands.UpdateProfilePicture;
using JadaraITKnowledgeSystem.Application.Features.Users.Commands.UpdateUserProfile;
using JadaraITKnowledgeSystem.Application.Features.Users.Dtos;
using JadaraITKnowledgeSystem.Application.Features.Users.Queries.GetCurrentUserProfile;
using JadaraITKnowledgeSystem.Application.Features.Users.Queries.GetUsers;
using JadaraITKnowledgeSystem.Application.Features.Users.Queries.GetUsersWithDetails;
using JadaraITKnowledgeSystem.Application.Features.Users.Queries.GetWriterStatistics;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JadaraITKnowledgeSystem.API.Controllers;

[Route("api/users")]
public sealed class UsersController(ISender sender) : ApiControllerBase(sender)
{
    /// <summary>The caller's profile, role and academic affiliation.</summary>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType<UserProfileDto>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMe(CancellationToken cancellationToken) =>
        OkOrProblem(await Sender.Send(new GetCurrentUserProfileQuery(), cancellationToken));

    /// <summary>Updates the caller's full name and/or major.</summary>
    [HttpPut("me")]
    [Authorize]
    [ProducesResponseType<UserProfileDto>(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request, CancellationToken cancellationToken) =>
        OkOrProblem(await Sender.Send(new UpdateUserProfileCommand(request.FullName, request.MajorId), cancellationToken));

    /// <summary>Uploads or replaces the caller's profile picture (max 5 MB; jpg, png, gif, webp).</summary>
    [HttpPost("me/profile-picture")]
    [Authorize]
    [RequestSizeLimit(6_000_000)]
    [ProducesResponseType<UserProfileDto>(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateProfilePicture(IFormFile image, CancellationToken cancellationToken)
    {
        await using var stream = image.OpenReadStream();
        return OkOrProblem(await Sender.Send(new UpdateProfilePictureCommand(stream, image.FileName), cancellationToken));
    }

    /// <summary>Removes the caller's profile picture.</summary>
    [HttpDelete("me/profile-picture")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteProfilePicture(CancellationToken cancellationToken) =>
        NoContentOrProblem(await Sender.Send(new DeleteProfilePictureCommand(), cancellationToken));

    /// <summary>Pages through users, optionally filtered by university, faculty, major, email or id.</summary>
    [HttpGet]
    [Authorize(Roles = Roles.AdminOrAbove)]
    [ProducesResponseType<PaginatedList<UserDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUsers(
        [FromQuery] int? universityId,
        [FromQuery] int? facultyId,
        [FromQuery] int? majorId,
        [FromQuery] string? email,
        [FromQuery] int? id,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default) =>
        OkOrProblem(await Sender.Send(
            new GetUsersQuery(universityId, facultyId, majorId, email, id, pageNumber, pageSize), cancellationToken));

    /// <summary>Pages through users with academic details and status (same filters plus isActive/isVerified).</summary>
    [HttpGet("details")]
    [Authorize(Roles = Roles.AdminOrAbove)]
    [ProducesResponseType<PaginatedList<UserDetailsDto>>(StatusCodes.Status200OK)]
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
        CancellationToken cancellationToken = default) =>
        OkOrProblem(await Sender.Send(
            new GetUsersWithDetailsQuery(universityId, facultyId, majorId, email, id, isActive, isVerified, pageNumber, pageSize),
            cancellationToken));

    /// <summary>Blocks a user: they can no longer sign in or refresh their session.</summary>
    [HttpPost("{id:int}/block")]
    [Authorize(Roles = Roles.AdminOrAbove)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> BlockUser(int id, CancellationToken cancellationToken) =>
        NoContentOrProblem(await Sender.Send(new BlockUserCommand(id), cancellationToken));

    /// <summary>Re-activates a blocked user.</summary>
    [HttpPost("{id:int}/activate")]
    [Authorize(Roles = Roles.AdminOrAbove)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> ActivateUser(int id, CancellationToken cancellationToken) =>
        NoContentOrProblem(await Sender.Send(new ActivateUserCommand(id), cancellationToken));

    /// <summary>The roles that can be assigned.</summary>
    [HttpGet("roles")]
    [Authorize(Roles = Roles.AdminOrAbove)]
    [ProducesResponseType<List<string>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRoles(CancellationToken cancellationToken) =>
        OkOrProblem(await Sender.Send(new GetRolesQuery(), cancellationToken));

    /// <summary>Replaces the user's role (body: the role name as a JSON string). Only a SuperAdmin can grant or revoke SuperAdmin.</summary>
    [HttpPost("{id:int}/assign-role")]
    [Authorize(Roles = Roles.AdminOrAbove)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> AssignRole(int id, [FromBody] string roleName, CancellationToken cancellationToken) =>
        NoContentOrProblem(await Sender.Send(new AssignRoleToUserCommand(id, roleName), cancellationToken));

    /// <summary>Dashboard statistics for a writer (writers can only see their own).</summary>
    [HttpGet("{id:int}/writer-statistics")]
    [Authorize(Roles = Roles.WriterOrAbove)]
    [ProducesResponseType<WriterStatisticsDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetWriterStatistics(int id, CancellationToken cancellationToken) =>
        OkOrProblem(await Sender.Send(new GetWriterStatisticsQuery(id), cancellationToken));
}
