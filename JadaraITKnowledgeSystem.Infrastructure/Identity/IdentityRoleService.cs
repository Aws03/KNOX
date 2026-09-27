using JadaraITKnowledgeSystem.Application.Interfaces.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace JadaraITKnowledgeSystem.Infrastructure.Identity
{
    public class IdentityRoleService : IIdentityRoleService
    {
        private readonly RoleManager<ApplicationRole> _roleManager;

        public IdentityRoleService(RoleManager<ApplicationRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<List<string>> GetRolesAsync(CancellationToken cancellationToken)
        {
            return await _roleManager.Roles.Select(r => r.Name!).ToListAsync(cancellationToken);
        }
    }
}
