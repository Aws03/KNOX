namespace JadaraITKnowledgeSystem.Application.Interfaces.Services
{
    public interface IJwtTokenService
    {
        Task<string> GenerateJwtTokenAsync(int userId, int domainUserId, string? fullName, string? email, IEnumerable<string> roles);
    }
}
