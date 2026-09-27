using System.Reflection;
using FluentValidation;
using JadaraITKnowledgeSystem.Application.Common.Behaviours;
using JadaraITKnowledgeSystem.Application.Features.Auth.Services;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace JadaraITKnowledgeSystem.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // MediatR
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly);

            // All behaviours are open generics over TRequest/TResponse directly
            // (see ValidationBehavior's own comment for why that constraint matters).
            // Unhandled exceptions are left to the API's GlobalExceptionHandler.
            cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            // Registered before TransactionBehavior so its post-`next()` code runs
            // strictly after the transaction below has committed.
            cfg.AddOpenBehavior(typeof(DispatchPostCommitJobsBehavior<,>));
            cfg.AddOpenBehavior(typeof(TransactionBehavior<,>));
        });

        // FluentValidation
        services.AddValidatorsFromAssembly(assembly);

        services.AddScoped<AuthTokenIssuer>();

        return services;
    }
}
