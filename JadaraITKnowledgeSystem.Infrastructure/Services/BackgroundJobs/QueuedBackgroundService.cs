using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace JadaraITKnowledgeSystem.Infrastructure.Services.BackgroundJobs;

/// <summary>
/// Drains BackgroundJobQueue for the lifetime of the app. Each work item gets its
/// own DI scope (a fresh IApplicationDbContext, etc.) and failures are caught and
/// logged per-item so one failing job (e.g. quiz generation for a bad upload)
/// never takes down the whole worker loop.
/// </summary>
public class QueuedBackgroundService : BackgroundService
{
    private readonly BackgroundJobQueue _jobQueue;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<QueuedBackgroundService> _logger;

    public QueuedBackgroundService(
        BackgroundJobQueue jobQueue,
        IServiceProvider serviceProvider,
        ILogger<QueuedBackgroundService> logger)
    {
        _jobQueue = jobQueue;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Queued background service running.");

        await foreach (var workItem in _jobQueue.DequeueAllAsync(stoppingToken))
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                await workItem(scope.ServiceProvider, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing queued background work item.");
            }
        }
    }
}
