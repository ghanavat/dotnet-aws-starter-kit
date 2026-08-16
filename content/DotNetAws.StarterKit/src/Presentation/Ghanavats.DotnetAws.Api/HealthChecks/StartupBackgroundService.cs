using System.Diagnostics.CodeAnalysis;

namespace Ghanavats.DotnetAws.Api.HealthChecks;

/// <summary>
/// A simple example background service that simulates a long-running task.
/// It is useful when a long-running task is being executed to prevent further requests while the long-running task is running.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class StartupBackgroundService : BackgroundService
{
    private readonly StartupHealthCheck _healthCheck;
    
    public StartupBackgroundService(StartupHealthCheck healthCheck)
    {
        _healthCheck = healthCheck;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Simulate the effect of a long-running task.
        await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);

        _healthCheck.StartupCompleted = true;
    }
}
