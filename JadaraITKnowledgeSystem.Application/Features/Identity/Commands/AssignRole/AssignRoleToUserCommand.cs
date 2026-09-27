using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;

namespace JadaraITKnowledgeSystem.Application.Features.Identity.Commands.AssignRole;

/// <summary>
/// Replaces the user's role with <paramref name="RoleName"/> (users hold exactly one role).
/// <paramref name="UserId"/> is the domain user id unless <paramref name="UserIdIsIdentityId"/> is set.
/// </summary>
public sealed record AssignRoleToUserCommand(int UserId, string RoleName, bool UserIdIsIdentityId = false)
    : IRequest<Result<Success>>;
