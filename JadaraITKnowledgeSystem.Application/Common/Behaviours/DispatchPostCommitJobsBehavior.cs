using JadaraITKnowledgeSystem.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace JadaraITKnowledgeSystem.Application.Common.Behaviours;

/// <summary>
/// Registered so it wraps (executes outside) TransactionBehavior in the pipeline.
/// Only after `next()` returns - which for a *Command means TransactionBehavior has
/// already committed - does it drain anything handlers staged via
/// IPostCommitDispatcher and hand it to the real IBackgroundJobQueue, so background
/// work never runs against rows that might still be rolled back.
/// A nested command that joined an outer transaction leaves its staged work in place;
/// the outermost request drains it once that transaction has committed.
/// </summary>
public class DispatchPostCommitJobsBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IPostCommitDispatcher _dispatcher;
    private readonly IBackgroundJobQueue _jobQueue;
    private readonly IApplicationDbContext _context;
    private readonly ILogger<DispatchPostCommitJobsBehavior<TRequest, TResponse>> _logger;

    public DispatchPostCommitJobsBehavior(
        IPostCommitDispatcher dispatcher,
        IBackgroundJobQueue jobQueue,
        IApplicationDbContext context,
        ILogger<DispatchPostCommitJobsBehavior<TRequest, TResponse>> logger)
    {
        _dispatcher = dispatcher;
        _jobQueue = jobQueue;
        _context = context;
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var response = await next();

        if (_context.Database.CurrentTransaction is not null)
            return response;

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
