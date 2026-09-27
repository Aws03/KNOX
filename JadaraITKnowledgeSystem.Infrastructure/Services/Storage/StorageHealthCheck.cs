using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace JadaraITKnowledgeSystem.Infrastructure.Services.Storage;

/// <summary>Ready only when object storage answers: uploads and material URLs depend on it.</summary>
public sealed class StorageHealthCheck(S3StorageService storage) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            timeout.CancelAfter(TimeSpan.FromSeconds(5));
            await storage.CheckAvailabilityAsync(timeout.Token);
            return HealthCheckResult.Healthy();
        }
        catch (Exception ex)
        {
            return new HealthCheckResult(context.Registration.FailureStatus, "Object storage is unreachable.", ex);
        }
    }
}
