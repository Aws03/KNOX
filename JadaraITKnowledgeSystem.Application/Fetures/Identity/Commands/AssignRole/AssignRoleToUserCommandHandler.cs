using JadaraITKnowledgeSystem.Application.Interfaces;
using JadaraITKnowledgeSystem.Application.Interfaces.Services;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JadaraITKnowledgeSystem.Application.Fetures.Identity.Commands.AssignRole;

public sealed class AssignRoleToUserCommandHandler
    (IApplicationDbContext context, IIdentityUserService identityUserService, ILogger<AssignRoleToUserCommandHandler> logger)
    : IRequestHandler<AssignRoleToUserCommand, Result<Success>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IIdentityUserService _identityUserService = identityUserService;
    private readonly ILogger<AssignRoleToUserCommandHandler> _logger = logger;

    public async Task<Result<Success>> Handle(AssignRoleToUserCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Assigning role {Role} to user {UserId}", request.RoleName, request.UserId);

        var domainUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);
        if (domainUser is null)
            return Error.NotFound("Users.NotFound", $"User {request.UserId} not found.");

        var identityUser = await _context.RefreshTokens
            .Where(rt => rt.UserId == domainUser.Id) // ensure identity exists by any related record; fallback to identity service requires mapping
            .Select(rt => rt.UserId)
            .FirstOrDefaultAsync(cancellationToken);

        // We don't have identity Id directly; use IdentityUserService to add role by email
        var res = await _identityUserService.AddToRoleAsync(identityUserId: domainUser.Id, role: request.RoleName);
        if (!res.IsSuccess)
            return res.Errors;

        _logger.LogInformation("Assigned role {Role} to user {UserId}", request.RoleName, request.UserId);
        return Result.Success;
    }
}
