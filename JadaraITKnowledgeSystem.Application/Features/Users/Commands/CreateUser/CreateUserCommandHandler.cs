using JadaraITKnowledgeSystem.Application.Features.Users.Commands.CreateUser;
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
        private readonly IApplicationDbContext _context = context;
        private readonly IIdentityUserService _identityService = identityService;
        private readonly ILogger<CreateUserCommandHandler> _logger = logger;

        public async Task<Result<CreateUserResultDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("[CreateUser] Starting for Email={Email}, MajorId={MajorId}", request.Email, request.MajorId);

            var majorExists = await _context.Majors.AsNoTracking().AnyAsync(m => m.Id == request.MajorId, cancellationToken);
            if (!majorExists)
            {
                _logger.LogWarning("[CreateUser] Major not found: {MajorId}", request.MajorId);
                return Error.Validation(code: "Major.NotFound", description: "The specified Major does not exist.");
            }

            Result<User> domainUserResult;
            try
            {
                var nameVo = new FullName(request.FullName);
                var emailVo = new Email(request.Email);
                domainUserResult = User.Create(nameVo, emailVo, request.MajorId);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "[CreateUser] Invalid value objects for Email={Email}", request.Email);
                return Error.Validation(description: "Invalid user data.");
            }

            if (!domainUserResult.IsSuccess)
            {
                _logger.LogWarning("[CreateUser] Domain creation failed: {Errors}", string.Join("; ", domainUserResult.Errors.Select(e => e.Description)));
                return domainUserResult.Errors;
            }

            var domainUser = domainUserResult.Value;

            await _context.Users.AddAsync(domainUser, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("[CreateUser] Domain user persisted with Id={DomainUserId}", domainUser.Id);

            try
            {
                var identityCreate = await _identityService.CreateAsync(request.Email, request.FullName, domainUser.Id, request.Password);
                if (!identityCreate.IsSuccess)
                {
                    _logger.LogWarning("[CreateUser] Identity creation failed for DomainUserId={DomainUserId}: {Errors}", domainUser.Id, string.Join("; ", identityCreate.Errors.Select(e => e.Description)));

                    _context.Users.Remove(domainUser);
                    await _context.SaveChangesAsync(cancellationToken);
                    _logger.LogInformation("[CreateUser] Rolled back DomainUser Id={DomainUserId}", domainUser.Id);

                    return identityCreate.Errors;
                }

                var identityUserId = identityCreate.Value.identityUserId;
                _logger.LogInformation("[CreateUser] Identity user created Id={IdentityUserId} for DomainUserId={DomainUserId}", identityUserId, domainUser.Id);

                const string defaultRole = "User";
                var roleResult = await _identityService.AddToRoleAsync(identityUserId, defaultRole);
                if (!roleResult.IsSuccess)
                {
                    _logger.LogWarning("[CreateUser] Adding role '{Role}' failed for IdentityUserId={IdentityUserId}: {Errors}", defaultRole, identityUserId, string.Join("; ", roleResult.Errors.Select(e => e.Description)));

                    _context.Users.Remove(domainUser);
                    await _context.SaveChangesAsync(cancellationToken);
                    _logger.LogInformation("[CreateUser] Rolled back DomainUser Id={DomainUserId} after role failure", domainUser.Id);

                    return roleResult.Errors;
                }

                _logger.LogInformation("[CreateUser] User created successfully. DomainUserId={DomainUserId}, IdentityUserId={IdentityUserId}, Role={Role}", domainUser.Id, identityUserId, defaultRole);

                var dto = new CreateUserResultDto(
                    DomainUserId: domainUser.Id,
                    IdentityUserId: identityUserId,
                    Email: request.Email,
                    AssignedRole: defaultRole);

                return dto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[CreateUser] Unexpected error. Rolling back DomainUser Id={DomainUserId}", domainUser.Id);
                _context.Users.Remove(domainUser);
                await _context.SaveChangesAsync(cancellationToken);
                return Error.Unexpected(description: "Unexpected error during user creation.");
            }
        }
    }
}
