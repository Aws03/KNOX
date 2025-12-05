namespace JadaraITKnowledgeSystem.Application.Interfaces.Services;

public interface IIdentityRoleService
{
    Task<List<string>> GetRolesAsync(CancellationToken cancellationToken);
}
