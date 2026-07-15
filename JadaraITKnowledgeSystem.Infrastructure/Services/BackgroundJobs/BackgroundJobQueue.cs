using System.Threading.Channels;
using JadaraITKnowledgeSystem.Application.Interfaces;

namespace JadaraITKnowledgeSystem.Infrastructure.Services.BackgroundJobs;

/// <summary>
/// Singleton, process-wide queue backing QueuedBackgroundService. Unbounded so a
/// producer (a request handling a command) is never blocked waiting for the
/// consumer - queued jobs here are already known to be safely runnable (their
/// owning transaction has committed), so the only cost of unbounded growth is
/// memory, not correctness.
/// </summary>
public class BackgroundJobQueue : IBackgroundJobQueue
{
    private readonly Channel<Func<IServiceProvider, CancellationToken, Task>> _queue =
        Channel.CreateUnbounded<Func<IServiceProvider, CancellationToken, Task>>();

    public void QueueBackgroundWorkItem(Func<IServiceProvider, CancellationToken, Task> workItem)
    {
        ArgumentNullException.ThrowIfNull(workItem);
        _queue.Writer.TryWrite(workItem);
    }

    public IAsyncEnumerable<Func<IServiceProvider, CancellationToken, Task>> DequeueAllAsync(
        CancellationToken cancellationToken) =>
        _queue.Reader.ReadAllAsync(cancellationToken);
}
