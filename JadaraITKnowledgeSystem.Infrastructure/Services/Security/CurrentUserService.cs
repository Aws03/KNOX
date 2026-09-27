using System.Security.Claims;
using JadaraITKnowledgeSystem.Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace JadaraITKnowledgeSystem.Infrastructure.Services.Security
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

        public int? UserId =>
            ParseInt(User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User?.FindFirst(ClaimTypes.Sid)?.Value);

        // Tokens issued before the domain_user_id claim existed only carry the identity id;
        // fall back to it so those sessions keep their previous behaviour until they expire.
        public int? DomainUserId =>
            ParseInt(User?.FindFirst(CustomClaimTypes.DomainUserId)?.Value) ?? UserId;

        public string? Email => User?.FindFirst(ClaimTypes.Email)?.Value;

        public IReadOnlyList<string> Roles =>
            User?.FindAll(ClaimTypes.Role).Select(r => r.Value).ToList() ?? [];

        private static int? ParseInt(string? value) => int.TryParse(value, out var parsed) ? parsed : null;
    }
}
