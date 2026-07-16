using FluentValidation;
using JadaraITKnowledgeSystem.Application.Common.Behaviours;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

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

            // All 5 behaviours are open generics over TRequest/TResponse directly
            // (see ValidationBehavior's own comment for why that constraint matters -
            // it used to be generic over TValue with TResponse hardcoded to
            // Result<TValue>, which the DI container could never actually resolve).
            cfg.AddOpenBehavior(typeof(ExceptionHandlingBehavior<,>));
            cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            // Registered before TransactionBehavior so its post-`next()` code runs
            // strictly after the transaction below has committed.
            cfg.AddOpenBehavior(typeof(DispatchPostCommitJobsBehavior<,>));
            cfg.AddOpenBehavior(typeof(TransactionBehavior<,>));
        });

        // FluentValidation
        services.AddValidatorsFromAssembly(assembly);

        return services;
    }
}
