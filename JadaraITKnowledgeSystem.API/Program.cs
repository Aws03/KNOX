using JadaraITKnowledgeSystem.API.Extensions;
using JadaraITKnowledgeSystem.Application;
using JadaraITKnowledgeSystem.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplication()
    .AddInfrastructure()
    .AddApi(builder.Configuration);

var app = builder.Build();

app.UseApiPipeline();
await app.InitializeDatabaseAsync();
await app.RunAsync();

// Exposes the entry point to WebApplicationFactory in the integration tests.
public partial class Program;
