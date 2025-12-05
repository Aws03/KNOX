using JadaraITKnowledgeSystem.Application.Interfaces.Services;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using JadaraITKnowledgeSystem.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace JadaraITKnowledgeSystem.Infrastructure.Identity
{
    public class IdentityUserService : IIdentityUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public IdentityUserService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
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
