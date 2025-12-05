using JadaraITKnowledgeSystem.Domain.Common.Results;

namespace JadaraITKnowledgeSystem.Application.Interfaces.Services
{
    public interface IIdentityUserService
    {
        Task<Result<(int identityUserId, IEnumerable<Error> errors)>> CreateAsync(string email, string fullName, int domainUserId, string? password);
        Task<Result<Success>> AddToRoleAsync(int identityUserId, string role);
        Task<Result<Success>> ResetPasswordAsync(string email, string newPassword);
    }
}
