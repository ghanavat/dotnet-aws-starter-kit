using Amazon;
using Amazon.CDK;
using Ghanavats.DotnetAws.IaC.Stages;
using Environment = Amazon.CDK.Environment;

namespace Ghanavats.DotnetAws.IaC;

internal static class Program
{
    public static void Main(string[] args)
    {
        var app = new App();
        
        _ = new ApplicationStage(app, "Dev", new Environment
            {
                Account = "1234567890",
                Region = RegionEndpoint.EUWest1.SystemName
            },
            new EnvironmentSettings
            {
                IsProduction = false,
                Name = "dev",
                LambdaMemorySize = 2048
            });
        
        app.Synth();
    }
}
