using JadaraITKnowledgeSystem.Application.Features.Users.Dtos;
using JadaraITKnowledgeSystem.Application.Interfaces;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

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
                .FirstOrDefaultAsync(u => u.Email.Address == email, cancellationToken);

            if (domainUser is null)
            {
                _logger.LogWarning("[GetCurrentUserProfile] Domain user not found for email {Email}", email);
                return Error.NotFound("Users.DomainNotFound", "Domain user not found.");
            }

            var role = _currentUser.Roles.FirstOrDefault() ?? "User";

            var dto = new UserProfileDto
            {
                IdentityUserId = identityUserId.Value,
                DomainUserId = domainUser.Id,
                Email = email,
                FullName = domainUser.Name.ToString(),
                DateJoined = domainUser.CreatedAt.UtcDateTime,
                Role = role,
                IsActive = domainUser.IsActive,
                IsVerified = domainUser.IsVerified,
                VerificationDate = domainUser.VerificationDate,
                ProfilePictureUrl = domainUser.ProfilePictureUrl,
                UniversityId = domainUser.Major.Faculty.University.Id,
                UniversityName = domainUser.Major.Faculty.University.Name,
                FacultyId = domainUser.Major.Faculty.Id,
                FacultyName = domainUser.Major.Faculty.Name,
                MajorId = domainUser.Major.Id,
                MajorName = domainUser.Major.Name
            };

            return dto;
        }
    }
}
