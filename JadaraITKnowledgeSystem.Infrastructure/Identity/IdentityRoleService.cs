using JadaraITKnowledgeSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Identity;

namespace JadaraITKnowledgeSystem.Infrastructure.Identity
{
    public class IdentityRoleService : IIdentityRoleService
    {
        private readonly RoleManager<ApplicationRole> _roleManager;

        public IdentityRoleService(RoleManager<ApplicationRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public Task<List<string>> GetRolesAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(_roleManager.Roles.Select(r => r.Name!).ToList());
        }
    }
}
