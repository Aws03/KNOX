using JadaraITKnowledgeSystem.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace JadaraITKnowledgeSystem.Application.Common.Behaviours
{
    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;
        //private readonly ICurrentUserService _currentUser;

        public LoggingBehavior(
            ILogger<LoggingBehavior<TRequest, TResponse>> logger
            //ICurrentUserService currentUser
            )
        {
            _logger = logger;
            //_currentUser = currentUser;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;
            //var userId = _currentUser.UserId ?? 0;
            //var userName = _currentUser.UserName ?? "Anonymous";

            _logger.LogInformation(
                "Handling {RequestName} for User {UserId} ({UserName})",
                requestName, 0, 0); //TODO : change later when implement user service

            var stopwatch = Stopwatch.StartNew();

            try
            {
                var response = await next();
                stopwatch.Stop();

                _logger.LogInformation(
                    "Handled {RequestName} in {ElapsedMs}ms",
                    requestName, stopwatch.ElapsedMilliseconds);

                return response;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();

                _logger.LogError(ex,
                    "Error handling {RequestName} after {ElapsedMs}ms",
                    requestName, stopwatch.ElapsedMilliseconds);

                throw;
            }
        }
    }
}
