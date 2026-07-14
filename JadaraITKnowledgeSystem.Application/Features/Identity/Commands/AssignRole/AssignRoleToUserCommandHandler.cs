using JadaraITKnowledgeSystem.Application.Interfaces;
using JadaraITKnowledgeSystem.Application.Interfaces.Services;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JadaraITKnowledgeSystem.Application.Features.Identity.Commands.AssignRole;

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

        // Enforce single role: remove all existing, add the requested role
        var res = await _identityUserService.SetSingleRoleAsync(identityUserId: domainUser.Id, role: request.RoleName);
        if (!res.IsSuccess)
            return res.Errors;

        _logger.LogInformation("Assigned single role {Role} to user {UserId}", request.RoleName, request.UserId);
        return Result.Success;
    }
}
