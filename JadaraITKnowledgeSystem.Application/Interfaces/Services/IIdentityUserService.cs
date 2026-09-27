using JadaraITKnowledgeSystem.Domain.Common.Results;

namespace JadaraITKnowledgeSystem.Application.Interfaces.Services
{
    public interface IIdentityUserService
    {
        /// <summary>Creates the identity account linked to <paramref name="domainUserId"/> and returns its id.</summary>
        Task<Result<int>> CreateAsync(string email, string fullName, int domainUserId, string? password);
        Task<Result<Success>> AddToRoleAsync(int identityUserId, string role);
        Task<Result<Success>> SetSingleRoleAsync(int identityUserId, string role);
        Task<Result<Success>> ResetPasswordAsync(string email, string newPassword);
        Task<Result<Success>> ChangePasswordAsync(int identityUserId, string currentPassword, string newPassword);

        Task<IdentityUserInfo?> FindByIdAsync(int identityUserId);
        Task<IdentityUserInfo?> FindByEmailAsync(string email);
        Task<IdentityUserInfo?> FindByDomainUserIdAsync(int domainUserId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<string>> GetRolesAsync(int identityUserId);

        /// <summary>Checks the password, counting failures towards account lockout.</summary>
        Task<bool> CheckPasswordAsync(int identityUserId, string password);
    }

    public sealed record IdentityUserInfo(int Id, int DomainUserId, string FullName, string? Email);
}
