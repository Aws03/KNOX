using JadaraITKnowledgeSystem.Application.Interfaces;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JadaraITKnowledgeSystem.Application.Fetures.Users.Commands.ActivateUser;

public sealed class ActivateUserCommandHandler
    (IApplicationDbContext context, ILogger<ActivateUserCommandHandler> logger)
    : IRequestHandler<ActivateUserCommand, Result<Success>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly ILogger<ActivateUserCommandHandler> _logger = logger;

    public async Task<Result<Success>> Handle(ActivateUserCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Activating user {UserId}", request.UserId);

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);
        if (user is null)
            return Error.NotFound("Users.NotFound", $"User {request.UserId} not found.");

        var res = user.ActivateAccount();
        if (!res.IsSuccess)
            return res.Errors;

        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Activated user {UserId}", request.UserId);
        return Result.Success;
    }
}
