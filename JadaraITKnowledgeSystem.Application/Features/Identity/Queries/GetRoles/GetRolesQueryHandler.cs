using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using JadaraITKnowledgeSystem.Application.Interfaces.Services;

namespace JadaraITKnowledgeSystem.Application.Features.Identity.Queries.GetRoles;

public sealed class GetRolesQueryHandler
    (IIdentityRoleService roleService, ILogger<GetRolesQueryHandler> logger)
    : IRequestHandler<GetRolesQuery, Result<List<string>>>
{
    private readonly IIdentityRoleService _roleService = roleService;
    private readonly ILogger<GetRolesQueryHandler> _logger = logger;

    public async Task<Result<List<string>>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Retrieving roles");
        var roles = await _roleService.GetRolesAsync(cancellationToken);
        return roles;
    }
}
