using JadaraITKnowledgeSystem.Infrastructure.Persistence;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace JadaraITKnowledgeSystem.API.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication UseApiPipeline(this WebApplication app)
    {
        if (app.Configuration.GetValue<bool>("ForwardedHeaders:Enabled"))
            app.UseForwardedHeaders();

        app.UseExceptionHandler();
        app.UseStatusCodePages();

        app.Use((context, next) =>
        {
            var headers = context.Response.Headers;
            headers.XContentTypeOptions = "nosniff";
            headers.XFrameOptions = "DENY";
            headers["Referrer-Policy"] = "no-referrer";
            return next(context);
        });

        app.UseCors(ServiceCollectionExtensions.FrontendCorsPolicy);
        app.UseRateLimiter();
        app.UseAuthentication();
        app.UseAuthorization();

        if (app.Configuration.GetValue("OpenApi:Enabled", app.Environment.IsDevelopment()))
        {
            app.MapOpenApi();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/openapi/v1.json", "KNOX API v1");
                options.RoutePrefix = "swagger";
            });
        }

        app.MapHealthChecks("/health/live", new HealthCheckOptions { Predicate = _ => false });
        app.MapHealthChecks("/health/ready", new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains(ServiceCollectionExtensions.ReadyHealthTag)
        });

        app.MapControllers();
        return app;
    }

    public static async Task InitializeDatabaseAsync(this WebApplication app)
    {
        await using var scope = app.Services.CreateAsyncScope();
        await scope.ServiceProvider.GetRequiredService<DatabaseInitializer>().InitializeAsync(app.Lifetime.ApplicationStopping);
    }

    public static async Task MigrateDatabaseAsync(this WebApplication app)
    {
        await using var scope = app.Services.CreateAsyncScope();
        await scope.ServiceProvider.GetRequiredService<DatabaseInitializer>().MigrateAsync();
    }
}
