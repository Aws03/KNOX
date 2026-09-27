using System.Diagnostics;
using JadaraITKnowledgeSystem.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace JadaraITKnowledgeSystem.Application.Common.Behaviours;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;
    private readonly ICurrentUserService _currentUser;

    public LoggingBehavior(
        ILogger<LoggingBehavior<TRequest, TResponse>> logger,
        ICurrentUserService currentUser)
    {
        _logger = logger;
        _currentUser = currentUser;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var userId = _currentUser.UserId ?? 0;

        _logger.LogInformation("Handling {RequestName} for User {UserId}", requestName, userId);

        var stopwatch = Stopwatch.StartNew();

        try
        {
            var response = await next();

            _logger.LogInformation(
                "Handled {RequestName} in {ElapsedMs}ms for User {UserId}",
                requestName, stopwatch.ElapsedMilliseconds, userId);

            return response;
        }
        catch (Exception ex)
        {
            // Logged once more (with the stack) by the API's exception middleware;
            // this entry adds the request name and timing.
            _logger.LogWarning(
                "{RequestName} failed after {ElapsedMs}ms for User {UserId}: {ExceptionType}",
                requestName, stopwatch.ElapsedMilliseconds, userId, ex.GetType().Name);

            throw;
        }
    }
}
