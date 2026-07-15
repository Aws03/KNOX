using System;
using System.Threading;
using System.Threading.Tasks;

namespace JadaraITKnowledgeSystem.Application.Interfaces;

/// <summary>
/// The real, process-wide background job queue backing a hosted BackgroundService.
/// Not injected directly into command handlers - use IPostCommitDispatcher instead,
/// so work is only ever handed off here once the current request's transaction has
/// actually committed. See DispatchPostCommitJobsBehavior.
/// </summary>
public interface IBackgroundJobQueue
{
    void QueueBackgroundWorkItem(Func<IServiceProvider, CancellationToken, Task> workItem);
}
