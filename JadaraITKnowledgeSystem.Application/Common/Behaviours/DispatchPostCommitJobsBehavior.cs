using JadaraITKnowledgeSystem.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace JadaraITKnowledgeSystem.Application.Common.Behaviours;

/// <summary>
/// Registered so it wraps (executes outside) TransactionBehavior in the pipeline.
/// Only after `next()` returns - which for a *Command means TransactionBehavior has
/// already committed - does it drain anything handlers staged via
/// IPostCommitDispatcher and hand it to the real IBackgroundJobQueue. This is what
/// replaces the old "Task.Run + Task.Delay(100) and hope the transaction committed
/// by then" pattern: the work is only ever queued once the commit has genuinely
/// happened, so no arbitrary delay is needed.
/// </summary>
public class DispatchPostCommitJobsBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IPostCommitDispatcher _dispatcher;
    private readonly IBackgroundJobQueue _jobQueue;
    private readonly ILogger<DispatchPostCommitJobsBehavior<TRequest, TResponse>> _logger;

    public DispatchPostCommitJobsBehavior(
        IPostCommitDispatcher dispatcher,
        IBackgroundJobQueue jobQueue,
        ILogger<DispatchPostCommitJobsBehavior<TRequest, TResponse>> logger)
    {
        _dispatcher = dispatcher;
        _jobQueue = jobQueue;
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var response = await next();

        var pendingWork = _dispatcher.DrainPendingWork();
        if (pendingWork.Count > 0)
        {
            _logger.LogInformation(
                "Dispatching {Count} post-commit job(s) for {RequestName}",
                pendingWork.Count, typeof(TRequest).Name);

            foreach (var workItem in pendingWork)
            {
                _jobQueue.QueueBackgroundWorkItem(workItem);
            }
        }

        return response;
    }
}
