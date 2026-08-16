using Ghanavats.DotnetAws.Api.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Ghanavats.DotnetAws.Api.Tests.HealthChecksTests;

public class StartupHealthCheckTests
{
    [Fact]
    public async Task ShouldReturnHealthy_WhenStartupCompletedIsSetToTrue()
    {
        //Arrange
        var sut = new StartupHealthCheck
        {
            StartupCompleted = true
        };

        //Act
        var actual = await sut.CheckHealthAsync(new HealthCheckContext(), TestContext.Current.CancellationToken);
        
        //Assert
        Assert.Equal(HealthStatus.Healthy, actual.Status);
    }
    
    [Fact]
    public async Task ShouldReturnUnhealthy_WhenStartupCompletedIsSetToFalse()
    {
        //Arrange
        var sut = new StartupHealthCheck
        {
            StartupCompleted = false
        };

        //Act
        var actual = await sut.CheckHealthAsync(new HealthCheckContext(), TestContext.Current.CancellationToken);
        
        //Assert
        Assert.Equal(HealthStatus.Unhealthy, actual.Status);
    }
}
