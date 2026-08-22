using Amazon;
using Environment = Amazon.CDK.Environment;

namespace Ghanavats.DotnetAws.IaC;

public static class AwsEnvironmentCreator
{
    public static Environment SetEnvironment()
    {
        return new Environment
        {
            Account = "YourAWSAccountId",
            Region = RegionEndpoint.EUWest1.SystemName
        };
    }
}
