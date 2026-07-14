using JadaraITKnowledgeSystem.Application.Interfaces;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JadaraITKnowledgeSystem.Application.Features.Users.Commands.BlockUser;

public sealed class BlockUserCommandHandler
    (IApplicationDbContext context, ILogger<BlockUserCommandHandler> logger)
    : IRequestHandler<BlockUserCommand, Result<Success>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly ILogger<BlockUserCommandHandler> _logger = logger;

    public async Task<Result<Success>> Handle(BlockUserCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Blocking user {UserId}", request.UserId);

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);
        if (user is null)
            return Error.NotFound("Users.NotFound", $"User {request.UserId} not found.");

        var res = user.BlockAccount();
        if (!res.IsSuccess)
            return res.Errors; // propagate errors

        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Blocked user {UserId}", request.UserId);
        return Result.Success;
    }
}
