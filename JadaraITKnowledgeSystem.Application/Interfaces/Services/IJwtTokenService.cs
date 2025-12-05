namespace JadaraITKnowledgeSystem.Application.Interfaces.Services
{
    public interface IJwtTokenService
    {
        Task<string> GenerateJwtTokenAsync(int userId, string? fullName, string? email, IEnumerable<string> roles);
    }
}
