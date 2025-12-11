using JadaraITKnowledgeSystem.Application.Interfaces.Services;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using JadaraITKnowledgeSystem.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace JadaraITKnowledgeSystem.Infrastructure.Identity
{
    public class IdentityUserService : IIdentityUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public IdentityUserService(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<Result<(int identityUserId, IEnumerable<Error> errors)>> CreateAsync(string email, string fullName, int domainUserId, string? password)
        {
            var user = new ApplicationUser
            {
                Email = email,
                UserName = email,
                FullName = fullName,
                DomainUserId = domainUserId,
                DateJoined = DateTime.UtcNow
            };

            var pwd = password ?? GeneratePassword();
            var result = await _userManager.CreateAsync(user, pwd);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => Error.Validation(e.Code, e.Description)).ToList();
                return (0, errors);
            }

            return (user.Id, Enumerable.Empty<Error>());
        }

        public async Task<Result<Success>> AddToRoleAsync(int identityUserId, string role)
        {
            var user = await _userManager.FindByIdAsync(identityUserId.ToString());
            if (user is null) return Error.NotFound(description: "Identity user not found");

            var result = await _userManager.AddToRoleAsync(user, role);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => Error.Validation(e.Code, e.Description)).ToList();
                return errors;
            }

            return Result.Success;
        }

        public async Task<Result<Success>> SetSingleRoleAsync(int identityUserId, string role)
        {
            var user = await _userManager.FindByIdAsync(identityUserId.ToString());
            if (user is null) return Error.NotFound(description: "Identity user not found");

            // Validate role exists
            var roleExists = await _roleManager.RoleExistsAsync(role);
            if (!roleExists)
            {
                return Error.Validation("Role.Invalid", $"Role '{role}' does not exist.");
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            if (currentRoles.Any())
            {
                var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                if (!removeResult.Succeeded)
                {
                    var errors = removeResult.Errors.Select(e => Error.Validation(e.Code, e.Description)).ToList();
                    return errors;
                }
            }

            var addResult = await _userManager.AddToRoleAsync(user, role);
            if (!addResult.Succeeded)
            {
                var errors = addResult.Errors.Select(e => Error.Validation(e.Code, e.Description)).ToList();
                return errors;
            }

            return Result.Success;
        }

        public async Task<Result<Success>> ResetPasswordAsync(string email, string newPassword)
        {
            // Find the identity user by email
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
            {
                return Error.NotFound("User.NotFound", "User not found");
            }

            // Generate a password reset token
            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

            // Reset the password using the token
            var result = await _userManager.ResetPasswordAsync(user, resetToken, newPassword);

            if (!result.Succeeded)
            {
                var errors = result.Errors
                    .Select(e => Error.Validation("Password.Reset", e.Description))
                    .ToList();
                return errors;
            }

            return Result.Success;
        }

        private static string GeneratePassword()
        {
            var bytes = System.Security.Cryptography.RandomNumberGenerator.GetBytes(16);
            var base64 = Convert.ToBase64String(bytes);
            return base64 + "aA1";
        }
    }
}
