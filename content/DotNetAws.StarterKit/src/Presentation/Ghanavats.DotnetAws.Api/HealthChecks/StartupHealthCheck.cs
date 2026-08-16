using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Ghanavats.DotnetAws.Api.HealthChecks;

/// <summary>
/// A simple example readiness and liveness health check
/// </summary>
public sealed class StartupHealthCheck : IHealthCheck
{
    private volatile bool _isReady;

    public bool StartupCompleted
    {
        get => _isReady;
        set => _isReady = value;
    }

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new())
    {
        return Task.FromResult(StartupCompleted
            ? HealthCheckResult.Healthy("The startup task has completed.")
            : HealthCheckResult.Unhealthy("That startup task is still running."));
    }
}
