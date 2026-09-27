using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace JadaraITKnowledgeSystem.API.Controllers;

/// <summary>
/// Base for all API controllers: translates Application <see cref="Result{TValue}"/> failures into
/// RFC 7807 problem details, so every endpoint reports errors the same way:
/// <c>{ type, title, status, detail, code, traceId }</c>, or for validation failures
/// <c>{ ..., errors: { field: [messages] } }</c>.
/// </summary>
[ApiController]
public abstract class ApiControllerBase(ISender sender) : ControllerBase
{
    protected ISender Sender { get; } = sender;

    protected string ClientIpAddress => HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

    protected IActionResult OkOrProblem<T>(Result<T> result) =>
        result.IsSuccess ? Ok(result.Value) : Problem(result.Errors);

    protected IActionResult NoContentOrProblem<T>(Result<T> result) =>
        result.IsSuccess ? NoContent() : Problem(result.Errors);

    protected IActionResult ResultOrProblem<T>(Result<T> result, Func<T, IActionResult> onSuccess) =>
        result.IsSuccess ? onSuccess(result.Value) : Problem(result.Errors);

    protected IActionResult Problem(IReadOnlyList<Error> errors)
    {
        if (errors.Count == 0)
            return Problem(statusCode: StatusCodes.Status500InternalServerError);

        if (errors.All(e => e.Type == ErrorKind.Validation))
        {
            var modelState = new ModelStateDictionary();
            foreach (var error in errors)
                modelState.AddModelError(error.Code, error.Description);

            return ValidationProblem(
                detail: string.Join(" ", errors.Select(e => e.Description)),
                modelStateDictionary: modelState);
        }

        var first = errors[0];
        var problem = ProblemDetailsFactory.CreateProblemDetails(HttpContext, StatusCodeFor(first.Type), detail: first.Description);
        problem.Extensions["code"] = first.Code;

        return new ObjectResult(problem) { StatusCode = problem.Status };
    }

    private static int StatusCodeFor(ErrorKind kind) => kind switch
    {
        ErrorKind.Validation or ErrorKind.Failure => StatusCodes.Status400BadRequest,
        ErrorKind.Unauthorized => StatusCodes.Status401Unauthorized,
        ErrorKind.Forbidden => StatusCodes.Status403Forbidden,
        ErrorKind.NotFound => StatusCodes.Status404NotFound,
        ErrorKind.Conflict => StatusCodes.Status409Conflict,
        _ => StatusCodes.Status500InternalServerError
    };
}
