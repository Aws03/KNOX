using JadaraITKnowledgeSystem.Application.Common.Security;
using JadaraITKnowledgeSystem.Application.Interfaces;
using JadaraITKnowledgeSystem.Application.Interfaces.Services;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace JadaraITKnowledgeSystem.Application.Features.Identity.Commands.AssignRole;

public sealed class AssignRoleToUserCommandHandler(
    IIdentityUserService identityUserService,
    ICurrentUserService currentUser,
    ILogger<AssignRoleToUserCommandHandler> logger)
    : IRequestHandler<AssignRoleToUserCommand, Result<Success>>
{
    public async Task<Result<Success>> Handle(AssignRoleToUserCommand request, CancellationToken cancellationToken)
    {
        var role = Roles.Normalize(request.RoleName);
        if (role is null)
            return Error.Validation("Role.Invalid", $"Role '{request.RoleName}' does not exist.");

        var target = request.UserIdIsIdentityId
            ? await identityUserService.FindByIdAsync(request.UserId)
            : await identityUserService.FindByDomainUserIdAsync(request.UserId, cancellationToken);

        if (target is null)
            return Error.NotFound("Users.NotFound", $"User {request.UserId} not found.");

        // An administrator demoting themselves would lock themselves out of the admin screens.
        if (currentUser.UserId == target.Id)
            return Error.Forbidden("Role.CannotChangeOwn", "You cannot change your own role; ask another administrator.");

        // Only a SuperAdmin may create another SuperAdmin or change an existing one's role;
        // otherwise any Admin could promote themselves to the top of the hierarchy.
        var callerIsSuperAdmin = currentUser.Roles.Contains(Roles.SuperAdmin, StringComparer.OrdinalIgnoreCase);
        if (!callerIsSuperAdmin)
        {
            var targetRoles = await identityUserService.GetRolesAsync(target.Id);
            if (role == Roles.SuperAdmin || Roles.Highest(targetRoles) == Roles.SuperAdmin)
            {
                logger.LogWarning("User {CallerId} attempted to change SuperAdmin privileges of user {TargetId}",
                    currentUser.UserId, target.Id);
                return Error.Forbidden("Role.AssignmentForbidden", "Only a SuperAdmin can grant or revoke the SuperAdmin role.");
            }
        }

        var result = await identityUserService.SetSingleRoleAsync(target.Id, role);
        if (result.IsError)
            return result.Errors;

        logger.LogInformation("Assigned role {Role} to identity user {IdentityUserId}", role, target.Id);
        return Result.Success;
    }
}
