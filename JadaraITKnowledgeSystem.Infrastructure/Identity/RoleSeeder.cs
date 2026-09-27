using JadaraITKnowledgeSystem.Application.Common.Security;
using Microsoft.AspNetCore.Identity;

namespace JadaraITKnowledgeSystem.Infrastructure.Identity
{
    public class RoleSeeder
    {
        private readonly RoleManager<ApplicationRole> _roleManager;

        public RoleSeeder(RoleManager<ApplicationRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task SeedAsync()
        {
            foreach (var role in Roles.All)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new ApplicationRole { Name = role });
                }
            }
        }
    }
}
