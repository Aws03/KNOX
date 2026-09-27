using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace JadaraITKnowledgeSystem.API.OpenApi;

/// <summary>
/// Describes authentication in the OpenAPI document: declares the JWT bearer scheme (so Swagger UI's
/// "Authorize" button works) and marks each operation from its real [Authorize]/[AllowAnonymous]
/// metadata, appending the roles it requires to the description.
/// </summary>
internal sealed class AuthorizationTransformer : IOpenApiDocumentTransformer, IOpenApiOperationTransformer
{
    private const string SchemeName = "Bearer";

    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
        document.Components.SecuritySchemes[SchemeName] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Description = "Paste the accessToken returned by POST /api/auth/login (without the \"Bearer \" prefix)."
        };
        return Task.CompletedTask;
    }

    public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
    {
        var metadata = context.Description.ActionDescriptor.EndpointMetadata;
        // Action-level [AllowAnonymous] wins over a controller-level [Authorize], as it does at runtime.
        if (metadata.OfType<IAllowAnonymous>().Any() || !metadata.OfType<IAuthorizeData>().Any())
            return Task.CompletedTask;

        operation.Security ??= [];
        operation.Security.Add(new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference(SchemeName, context.Document)] = []
        });

        // The most specific [Authorize(Roles = ...)] applies (action attributes come after the controller's).
        var roles = metadata.OfType<IAuthorizeData>().LastOrDefault(a => !string.IsNullOrEmpty(a.Roles))?.Roles;
        var access = roles is null ? "Requires a signed-in user." : $"Requires role: {roles.Replace(",", ", ")}.";
        operation.Description = string.IsNullOrEmpty(operation.Description) ? access : $"{operation.Description}\n\n{access}";

        return Task.CompletedTask;
    }
}
