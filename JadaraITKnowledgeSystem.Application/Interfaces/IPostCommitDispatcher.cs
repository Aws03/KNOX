using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace JadaraITKnowledgeSystem.Application.Interfaces;

/// <summary>
/// Scoped, per-request staging area for work that must only run after the
/// current command's transaction has actually committed (e.g. kicking off
/// AI quiz generation for a row the handler just inserted). Command handlers
/// call Enqueue instead of starting a Task.Run/background job directly -
/// DispatchPostCommitJobsBehavior drains this and hands items to the real
/// IBackgroundJobQueue only once TransactionBehavior's commit has returned.
/// </summary>
public interface IPostCommitDispatcher
{
    void Enqueue(Func<IServiceProvider, CancellationToken, Task> workItem);

    IReadOnlyList<Func<IServiceProvider, CancellationToken, Task>> DrainPendingWork();
}
