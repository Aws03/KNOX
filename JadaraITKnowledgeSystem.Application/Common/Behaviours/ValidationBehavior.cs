using System.Collections.Concurrent;
using System.Reflection;
using FluentValidation;
using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;

namespace JadaraITKnowledgeSystem.Application.Common.Behaviours;

// Every command's MediatR response is Result<TValue>, but a behavior generic over
// TValue directly (i.e. IPipelineBehavior<TRequest, Result<TValue>>) can never
// actually run: the plain ASP.NET Core DI container resolves open generics by
// substituting the *closed service type's* arguments positionally into the open
// implementation type, so IPipelineBehavior<Cmd, Result<Dto>> would construct
// ValidationBehavior<Cmd, Result<Dto>> (TValue = Result<Dto>, not Dto) - which
// fails its own `where TRequest : IRequest<Result<TValue>>` constraint and gets
// silently dropped from the pipeline. No exception, no log - it just never runs.
// (Confirmed directly: a diagnostic log at the top of Handle() never fired for
// any command, with or without MediatR's AddOpenBehavior - same underlying
// resolution.) So this behaves like every other behavior generically over
// TRequest/TResponse directly, and builds the failed Result<TValue> via
// reflection using Result<TValue>.Failure once TResponse's closed runtime type
// is known.
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private static readonly ConcurrentDictionary<Type, MethodInfo> FailureFactoryCache = new();

    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f is not null)
            .ToList();

        if (failures.Count == 0)
        {
            return await next();
        }

        var errors = failures
            .Select(f => Error.Validation(f.PropertyName, f.ErrorMessage))
            .ToList();

        var failureFactory = FailureFactoryCache.GetOrAdd(typeof(TResponse), responseType =>
            responseType.GetMethod("Failure", BindingFlags.Public | BindingFlags.Static)
                ?? throw new InvalidOperationException(
                    $"{responseType} must expose a public static Failure(List<Error>) factory " +
                    "to be used as a MediatR response type alongside ValidationBehavior."));

        return (TResponse)failureFactory.Invoke(null, [errors])!;
    }
}
