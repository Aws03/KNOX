using JadaraITKnowledgeSystem.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace JadaraITKnowledgeSystem.Infrastructure.Services.Security
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int? UserId
        {
            get
            {
                var user = _httpContextAccessor.HttpContext?.User;
                var id = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                    ?? user?.FindFirst(ClaimTypes.Sid)?.Value;
                return int.TryParse(id, out var parsed) ? parsed : (int?)null;
            }
        }

        public string? Email
        {
            get
            {
                var user = _httpContextAccessor.HttpContext?.User;
                return user?.FindFirst(ClaimTypes.Email)?.Value;
            }
        }

        public IReadOnlyList<string> Roles
        {
            get
            {
                var user = _httpContextAccessor.HttpContext?.User;
                var roles = user?.FindAll(ClaimTypes.Role).Select(r => r.Value).ToList() ?? new List<string>();
                return roles;
            }
        }
    }
}
