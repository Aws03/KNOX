using JadaraITKnowledgeSystem.Application.Common.Security;
using JadaraITKnowledgeSystem.Application.Interfaces;
using JadaraITKnowledgeSystem.Application.Interfaces.Services;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using JadaraITKnowledgeSystem.Domain.Users;
using JadaraITKnowledgeSystem.Domain.Users.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JadaraITKnowledgeSystem.Application.Features.Users.Commands.CreateUser
{
    public sealed class CreateUserCommandHandler
        (IApplicationDbContext context, IIdentityUserService identityService, ILogger<CreateUserCommandHandler> logger)
        : IRequestHandler<CreateUserCommand, Result<CreateUserResultDto>>
    {
        private const string DefaultRole = Roles.User;

        private readonly IApplicationDbContext _context = context;
        private readonly IIdentityUserService _identityService = identityService;
        private readonly ILogger<CreateUserCommandHandler> _logger = logger;

        public async Task<Result<CreateUserResultDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("[CreateUser] Starting for MajorId={MajorId}", request.MajorId);

            var majorExists = await _context.Majors.AsNoTracking().AnyAsync(m => m.Id == request.MajorId, cancellationToken);
            if (!majorExists)
            {
                _logger.LogWarning("[CreateUser] Major not found: {MajorId}", request.MajorId);
                return Error.Validation(code: "Major.NotFound", description: "The specified Major does not exist.");
            }

            Result<User> domainUserResult;
            try
            {
                domainUserResult = User.Create(new FullName(request.FullName), new Email(request.Email), request.MajorId);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "[CreateUser] Invalid user data");
                return Error.Validation(description: "Invalid user data.");
            }

            if (domainUserResult.IsError)
                return domainUserResult.Errors;

            var domainUser = domainUserResult.Value;

            await _context.Users.AddAsync(domainUser, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("[CreateUser] Domain user persisted with Id={DomainUserId}", domainUser.Id);

            try
            {
                var identityCreate = await _identityService.CreateAsync(request.Email, request.FullName, domainUser.Id, request.Password);
                if (identityCreate.IsError)
                {
                    _logger.LogWarning("[CreateUser] Identity creation failed for DomainUserId={DomainUserId}: {Errors}",
                        domainUser.Id, string.Join("; ", identityCreate.Errors.Select(e => e.Description)));
                    await RemoveDomainUserAsync(domainUser, cancellationToken);
                    return identityCreate.Errors;
                }

                var identityUserId = identityCreate.Value;

                var roleResult = await _identityService.AddToRoleAsync(identityUserId, DefaultRole);
                if (roleResult.IsError)
                {
                    _logger.LogWarning("[CreateUser] Adding role '{Role}' failed for IdentityUserId={IdentityUserId}: {Errors}",
                        DefaultRole, identityUserId, string.Join("; ", roleResult.Errors.Select(e => e.Description)));
                    await RemoveDomainUserAsync(domainUser, cancellationToken);
                    return roleResult.Errors;
                }

                _logger.LogInformation("[CreateUser] User created. DomainUserId={DomainUserId}, IdentityUserId={IdentityUserId}",
                    domainUser.Id, identityUserId);

                return new CreateUserResultDto(
                    DomainUserId: domainUser.Id,
                    IdentityUserId: identityUserId,
                    Email: request.Email,
                    AssignedRole: DefaultRole);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[CreateUser] Unexpected error. Rolling back DomainUser Id={DomainUserId}", domainUser.Id);
                await RemoveDomainUserAsync(domainUser, cancellationToken);
                return Error.Unexpected(description: "Unexpected error during user creation.");
            }
        }

        // The command's transaction commits even when a failure Result is returned,
        // so the half-created domain user has to be removed explicitly.
        private async Task RemoveDomainUserAsync(User domainUser, CancellationToken cancellationToken)
        {
            _context.Users.Remove(domainUser);
            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("[CreateUser] Rolled back DomainUser Id={DomainUserId}", domainUser.Id);
        }
    }
}
