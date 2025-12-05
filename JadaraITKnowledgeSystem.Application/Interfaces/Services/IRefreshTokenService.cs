using JadaraITKnowledgeSystem.Domain.Common.Results;
using JadaraITKnowledgeSystem.Domain.Identity;

namespace JadaraITKnowledgeSystem.Application.Interfaces.Services
{
    public interface IRefreshTokenService
    {
        Task<Result<RefreshToken>> GenerateRefreshTokenAsync(int userId, string ipAddress);
        Task<Result<RefreshToken>> GetActiveRefreshTokenAsync(string token);
        Task<Result<Success>> RevokeTokenAsync(string token, string ipAddress);
        Task<Result<Success>> RevokeAllUserTokensAsync(int userId, string ipAddress);
    }
}
