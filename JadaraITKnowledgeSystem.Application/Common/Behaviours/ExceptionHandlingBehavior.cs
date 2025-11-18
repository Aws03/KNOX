using JadaraITKnowledgeSystem.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;

namespace JadaraITKnowledgeSystem.Application.Common.Behaviours
{
    public class ExceptionHandlingBehavior<TRequest, TResponse>
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
        where TResponse : IResults
    {
        private readonly ILogger<ExceptionHandlingBehavior<TRequest, TResponse>> _logger;

        public ExceptionHandlingBehavior(
            ILogger<ExceptionHandlingBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            try
            {
                return await next();
            }
            catch (Exception ex)
            {
                var requestName = typeof(TRequest).Name;
                _logger.LogError(ex,
                    "Exception occurred while handling {RequestName}: {ExceptionMessage}",
                    requestName,
                    ex.Message);

                var error = ex switch
                {
                    ValidationException validationEx =>
                        Error.Validation("Validation.Failed", validationEx.Message),
                    UnauthorizedAccessException _ =>
                        Error.Unauthorized("Auth.Unauthorized", "You are not authorized to perform this action."),
                    KeyNotFoundException notFoundEx =>
                        Error.NotFound("Resource.NotFound", notFoundEx.Message),
                    InvalidOperationException invalidOpEx =>
                        Error.Failure("Operation.Invalid", invalidOpEx.Message),
                    DbUpdateException dbEx =>
                        Error.Conflict("Database.Conflict", "A database conflict occurred."),
                    _ => Error.Unexpected("Error.Unexpected", "An unexpected error occurred.")
                };

                // Create a Result<T> dynamically
                var resultType = typeof(TResponse);
                if (resultType.IsGenericType && resultType.GetGenericTypeDefinition() == typeof(Result<>))
                {
                    var genericArg = resultType.GetGenericArguments()[0];
                    var resultGenericType = typeof(Result<>).MakeGenericType(genericArg);

                    // Try to find the Failure method with better error handling
                    var failureMethod = resultGenericType
                        .GetMethods(BindingFlags.Public | BindingFlags.Static)
                        .FirstOrDefault(m =>
                            m.Name == "Failure" &&
                            m.GetParameters().Length == 1 &&
                            m.GetParameters()[0].ParameterType == typeof(Error));

                    if (failureMethod == null)
                    {
                        _logger.LogError(
                            "Could not find Failure method on {ResultType}. Available methods: {Methods}",
                            resultGenericType.Name,
                            string.Join(", ", resultGenericType.GetMethods(BindingFlags.Public | BindingFlags.Static).Select(m => m.Name)));
                        throw;
                    }

                    var result = failureMethod.Invoke(null, new object[] { error });
                    return (TResponse)result;
                }

                throw; // fallback if TResponse is not Result<T>
            }
        }
    }
}