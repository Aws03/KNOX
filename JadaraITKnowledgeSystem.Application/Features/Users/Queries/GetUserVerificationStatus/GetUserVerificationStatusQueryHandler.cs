using JadaraITKnowledgeSystem.Application.Interfaces;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace JadaraITKnowledgeSystem.Application.Features.Users.Queries.GetUserVerificationStatus;

public sealed class GetUserVerificationStatusQueryHandler : IRequestHandler<GetUserVerificationStatusQuery, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    public GetUserVerificationStatusQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<Result<bool>> Handle(GetUserVerificationStatusQuery request, CancellationToken ct)
    {
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email.Address == request.Email, ct);

        if (user is null)
            return Error.NotFound("User.NotFound", "User not found");

        return user.IsVerified;
    }
}
