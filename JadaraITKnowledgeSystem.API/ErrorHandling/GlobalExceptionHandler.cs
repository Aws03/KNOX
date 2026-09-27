using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JadaraITKnowledgeSystem.API.ErrorHandling;

/// <summary>
/// The single boundary for unhandled exceptions. Expected failures never reach it (handlers
/// return them as Result errors); what does is logged once and returned as problem details
/// without leaking internal messages, except for argument errors raised by domain invariants.
/// </summary>
public sealed class GlobalExceptionHandler(IProblemDetailsService problemDetailsService, ILogger<GlobalExceptionHandler> logger)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is OperationCanceledException && httpContext.RequestAborted.IsCancellationRequested)
            return true; // the client went away; nobody to answer

        var (status, title, detail) = exception switch
        {
            BadHttpRequestException bad => (bad.StatusCode, "Bad request", bad.Message),
            DbUpdateConcurrencyException => (StatusCodes.Status409Conflict, "Concurrent update",
                "The resource was changed by another request. Please retry."),
            DbUpdateException => (StatusCodes.Status409Conflict, "Conflict",
                "The change conflicts with existing data."),
            FileNotFoundException => (StatusCodes.Status400BadRequest, "Bad request",
                "A referenced file does not exist."),
            ArgumentException argument => (StatusCodes.Status400BadRequest, "Bad request", argument.Message),
            UnauthorizedAccessException => (StatusCodes.Status403Forbidden, "Forbidden", null),
            _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred.", null)
        };

        if (status >= StatusCodes.Status500InternalServerError)
            logger.LogError(exception, "Unhandled exception for {Method} {Path}", httpContext.Request.Method, httpContext.Request.Path);
        else
            logger.LogWarning(exception, "Request failed with {StatusCode} for {Method} {Path}", status, httpContext.Request.Method, httpContext.Request.Path);

        httpContext.Response.StatusCode = status;
        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails { Status = status, Title = title, Detail = detail }
        });
    }
}
