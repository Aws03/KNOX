using JadaraITKnowledgeSystem.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace JadaraITKnowledgeSystem.Infrastructure.Services.FileManagement;

public class TempFileCleanupJob : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<TempFileCleanupJob> _logger;
    private readonly TimeSpan _fileMaxAge;
    private int _executionCount;

    public TempFileCleanupJob(
        IServiceProvider serviceProvider,
        ILogger<TempFileCleanupJob> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _fileMaxAge = TimeSpan.FromHours(48);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Temp file cleanup service running.");

        // Run cleanup immediately on startup (after 2 min delay)
        await Task.Delay(TimeSpan.FromMinutes(2), stoppingToken);
        await DoWorkAsync(stoppingToken);

        // Then run every 24 hours using PeriodicTimer
        using PeriodicTimer timer = new(TimeSpan.FromHours(24));

        try
        {
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                await DoWorkAsync(stoppingToken);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Temp file cleanup service is stopping.");
        }
    }

    private async Task DoWorkAsync(CancellationToken stoppingToken)
    {
        int count = Interlocked.Increment(ref _executionCount);
        var startTime = DateTime.UtcNow;

        _logger.LogInformation(
            "Starting temp file cleanup (Execution #{Count}) at {Time}",
            count,
            startTime);

        using var scope = _serviceProvider.CreateScope();
        var fileManager = scope.ServiceProvider.GetRequiredService<IFileManager>();

        var deletedCount = await fileManager.DeleteOldTempFilesAsync(
            _fileMaxAge,
            stoppingToken);

        var duration = DateTime.UtcNow - startTime;

        _logger.LogInformation(
            "Temp file cleanup completed in {Duration:mm\\:ss}. Deleted {DeletedCount} file(s)",
            duration,
            deletedCount);
    }
}