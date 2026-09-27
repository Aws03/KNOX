using JadaraITKnowledgeSystem.Application.Common.Security;
using JadaraITKnowledgeSystem.Application.Features.Users.Dtos;
using JadaraITKnowledgeSystem.Application.Features.Users.Mappers;
using JadaraITKnowledgeSystem.Application.Interfaces;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using EmailAddress = JadaraITKnowledgeSystem.Domain.Users.ValueObjects.Email;

namespace JadaraITKnowledgeSystem.Application.Features.Users.Queries.GetCurrentUserProfile
{
    public sealed class GetCurrentUserProfileQueryHandler
        (ICurrentUserService currentUser, IApplicationDbContext db, ILogger<GetCurrentUserProfileQueryHandler> logger)
        : IRequestHandler<GetCurrentUserProfileQuery, Result<UserProfileDto>>
    {
        private readonly ICurrentUserService _currentUser = currentUser;
        private readonly IApplicationDbContext _db = db;
        private readonly ILogger<GetCurrentUserProfileQueryHandler> _logger = logger;

        public async Task<Result<UserProfileDto>> Handle(GetCurrentUserProfileQuery request, CancellationToken cancellationToken)
        {
            var identityUserId = _currentUser.UserId;
            var email = _currentUser.Email;
            if (identityUserId is null || string.IsNullOrWhiteSpace(email))
            {
                _logger.LogWarning("[GetCurrentUserProfile] Missing identity context (id or email)");
                return Error.Unauthorized("Auth.Unauthorized", "User is not authenticated.");
            }

            var domainUser = await _db.Users
                .Include(u => u.Major)
                    .ThenInclude(m => m.Faculty)
                        .ThenInclude(f => f.University)
                .FirstOrDefaultAsync(u => u.Email.Address == EmailAddress.Normalize(email), cancellationToken);

            if (domainUser is null)
            {
                _logger.LogWarning("[GetCurrentUserProfile] Domain user not found for email {Email}", email);
                return Error.NotFound("Users.DomainNotFound", "Domain user not found.");
            }

            var dto = domainUser.ToProfileDto(identityUserId.Value, email, Roles.Highest(_currentUser.Roles));

            return dto;
        }
    }
}
