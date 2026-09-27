using JadaraITKnowledgeSystem.Application.Interfaces.Services;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace JadaraITKnowledgeSystem.Infrastructure.Identity
{
    public class IdentityUserService : IIdentityUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public IdentityUserService(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        public async Task<Result<int>> CreateAsync(string email, string fullName, int domainUserId, string? password)
        {
            var user = new ApplicationUser
            {
                Email = email,
                UserName = email,
                FullName = fullName,
                DomainUserId = domainUserId,
                DateJoined = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, password ?? GeneratePassword());
            if (!result.Succeeded)
                return ToErrors(result);

            return user.Id;
        }

        public async Task<Result<Success>> AddToRoleAsync(int identityUserId, string role)
        {
            var user = await _userManager.FindByIdAsync(identityUserId.ToString());
            if (user is null) return Error.NotFound(description: "Identity user not found");

            var result = await _userManager.AddToRoleAsync(user, role);
            if (!result.Succeeded)
                return ToErrors(result);

            return Result.Success;
        }

        public async Task<Result<Success>> SetSingleRoleAsync(int identityUserId, string role)
        {
            var user = await _userManager.FindByIdAsync(identityUserId.ToString());
            if (user is null) return Error.NotFound(description: "Identity user not found");

            if (!await _roleManager.RoleExistsAsync(role))
                return Error.Validation("Role.Invalid", $"Role '{role}' does not exist.");

            var currentRoles = await _userManager.GetRolesAsync(user);
            if (currentRoles.Any())
            {
                var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                if (!removeResult.Succeeded)
                    return ToErrors(removeResult);
            }

            var addResult = await _userManager.AddToRoleAsync(user, role);
            if (!addResult.Succeeded)
                return ToErrors(addResult);

            return Result.Success;
        }

        public async Task<Result<Success>> ResetPasswordAsync(string email, string newPassword)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
                return Error.NotFound("User.NotFound", "User not found");

            // The caller has already proven ownership (OTP), so a reset token is minted and
            // consumed immediately rather than being emailed.
            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, resetToken, newPassword);
            if (!result.Succeeded)
                return ToErrors(result, "Password.Reset");

            return Result.Success;
        }

        public async Task<Result<Success>> ChangePasswordAsync(int identityUserId, string currentPassword, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(identityUserId.ToString());
            if (user is null)
                return Error.NotFound("User.NotFound", "User not found");

            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            if (!result.Succeeded)
                return ToErrors(result, "Password.Change");

            return Result.Success;
        }

        public async Task<IdentityUserInfo?> FindByIdAsync(int identityUserId) =>
            ToInfo(await _userManager.FindByIdAsync(identityUserId.ToString()));

        public async Task<IdentityUserInfo?> FindByEmailAsync(string email) =>
            string.IsNullOrWhiteSpace(email) ? null : ToInfo(await _userManager.FindByEmailAsync(email));

        public async Task<IdentityUserInfo?> FindByDomainUserIdAsync(int domainUserId, CancellationToken cancellationToken = default) =>
            ToInfo(await _userManager.Users.FirstOrDefaultAsync(u => u.DomainUserId == domainUserId, cancellationToken));

        public async Task<IReadOnlyList<string>> GetRolesAsync(int identityUserId)
        {
            var user = await _userManager.FindByIdAsync(identityUserId.ToString());
            if (user is null)
                return [];

            return (await _userManager.GetRolesAsync(user)).ToList();
        }

        public async Task<bool> CheckPasswordAsync(int identityUserId, string password)
        {
            var user = await _userManager.FindByIdAsync(identityUserId.ToString());
            if (user is null)
                return false;

            var result = await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: true);
            return result.Succeeded;
        }

        private static IdentityUserInfo? ToInfo(ApplicationUser? user) =>
            user is null ? null : new IdentityUserInfo(user.Id, user.DomainUserId, user.FullName, user.Email);

        private static List<Error> ToErrors(IdentityResult result, string? code = null) =>
            result.Errors.Select(e => Error.Validation(code ?? e.Code, e.Description)).ToList();

        private static string GeneratePassword()
        {
            var bytes = System.Security.Cryptography.RandomNumberGenerator.GetBytes(16);
            return Convert.ToBase64String(bytes) + "aA1";
        }
    }
}
