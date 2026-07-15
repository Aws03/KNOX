using JadaraITKnowledgeSystem.Application.Interfaces;

namespace JadaraITKnowledgeSystem.Infrastructure.Services.BackgroundJobs;

/// <summary>
/// Scoped - one instance per request/MediatR pipeline. Handlers stage work here;
/// DispatchPostCommitJobsBehavior drains it once the request's transaction has
/// committed and forwards each item to the singleton IBackgroundJobQueue.
/// </summary>
public class PostCommitDispatcher : IPostCommitDispatcher
{
    private readonly List<Func<IServiceProvider, CancellationToken, Task>> _pendingWork = new();

    public void Enqueue(Func<IServiceProvider, CancellationToken, Task> workItem)
    {
        _pendingWork.Add(workItem);
    }

    public IReadOnlyList<Func<IServiceProvider, CancellationToken, Task>> DrainPendingWork()
    {
        if (_pendingWork.Count == 0)
            return Array.Empty<Func<IServiceProvider, CancellationToken, Task>>();

        var drained = _pendingWork.ToList();
        _pendingWork.Clear();
        return drained;
    }
}
