using JadaraITKnowledgeSystem.Domain.Common.Results;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;

namespace JadaraITKnowledgeSystem.API.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, error) = exception switch
        {
            ValidationException validationEx =>
                (HttpStatusCode.BadRequest,
                 Error.Validation("Validation.Failed", validationEx.Message)),

            UnauthorizedAccessException _ =>
                (HttpStatusCode.Unauthorized,
                 Error.Unauthorized("Auth.Unauthorized", "Unauthorized access.")),

            KeyNotFoundException notFoundEx =>
                (HttpStatusCode.NotFound,
                 Error.NotFound("Resource.NotFound", notFoundEx.Message)),

            InvalidOperationException invalidOpEx =>
                (HttpStatusCode.BadRequest,
                 Error.Failure("Operation.Invalid", invalidOpEx.Message)),

            _ =>
                (HttpStatusCode.InternalServerError,
                 Error.Unexpected("Error.Unexpected", "An unexpected error occurred."))
        };

        context.Response.StatusCode = (int)statusCode;

        var problemDetails = new ProblemDetails
        {
            Status = (int)statusCode,
            Title = error.Code,
            Detail = error.Description,
            Instance = context.Request.Path
        };

        var response = JsonSerializer.Serialize(problemDetails);
        await context.Response.WriteAsync(response);
    }
}
