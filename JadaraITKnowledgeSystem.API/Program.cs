using JadaraITKnowledgeSystem.API.Extensions;
using JadaraITKnowledgeSystem.Application;
using JadaraITKnowledgeSystem.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplication()
    .AddInfrastructure()
    .AddApi(builder.Configuration);

var app = builder.Build();

// Deployment step: "dotnet JadaraITKnowledgeSystem.API.dll migrate" applies migrations, provisions the
// API's SQL login and seeds, then exits. Run it with an owner connection before starting the new version.
if (args.Contains("migrate", StringComparer.OrdinalIgnoreCase))
{
    await app.MigrateDatabaseAsync();
    return;
}

app.UseApiPipeline();
await app.InitializeDatabaseAsync();
await app.RunAsync();

// Exposes the entry point to WebApplicationFactory in the integration tests.
public partial class Program;
