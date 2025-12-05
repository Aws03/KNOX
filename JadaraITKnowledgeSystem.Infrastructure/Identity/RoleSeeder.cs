using Microsoft.AspNetCore.Identity;

namespace JadaraITKnowledgeSystem.Infrastructure.Identity
{
    public class RoleSeeder
    {
        private readonly RoleManager<ApplicationRole> _roleManager;

        private static readonly string[] Roles = new[]
        {
            "SuperAdmin",
            "Admin",
            "Writer",
            "User"
        };

        public RoleSeeder(RoleManager<ApplicationRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task SeedAsync()
        {
            foreach (var role in Roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new ApplicationRole { Name = role });
                }
            }
        }
    }
}

