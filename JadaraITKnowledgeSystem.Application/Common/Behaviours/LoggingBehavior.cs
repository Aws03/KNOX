using JadaraITKnowledgeSystem.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

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
        var userEmail = _currentUser.Email ?? "Anonymous";
        var userRoles = _currentUser.Roles != null && _currentUser.Roles.Any() 
            ? string.Join(", ", _currentUser.Roles) 
            : "None";

        _logger.LogInformation(
            "Handling {RequestName} for User {UserId} ({UserEmail}) with Roles [{UserRoles}]",
            requestName, userId, userEmail, userRoles);

        var stopwatch = Stopwatch.StartNew();

        try
        {
            var response = await next();
            stopwatch.Stop();

            _logger.LogInformation(
                "Handled {RequestName} successfully in {ElapsedMs}ms for User {UserId}",
                requestName, stopwatch.ElapsedMilliseconds, userId);

            return response;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            _logger.LogError(ex,
                "Error handling {RequestName} after {ElapsedMs}ms for User {UserId} ({UserEmail}): {ErrorMessage}",
                requestName, stopwatch.ElapsedMilliseconds, userId, userEmail, ex.Message);

            throw;
        }
    }
}
