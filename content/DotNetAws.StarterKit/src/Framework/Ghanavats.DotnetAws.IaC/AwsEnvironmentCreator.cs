using Amazon;
using Environment = Amazon.CDK.Environment;

namespace Ghanavats.DotnetAws.IaC;

public static class AwsEnvironmentCreator
{
    public static Environment SetEnvironment()
    {
        /*
            Env = new Amazon.CDK.Environment
            {
                Account = System.Environment.GetEnvironmentVariable("CDK_DEFAULT_ACCOUNT"),
                Region = System.Environment.GetEnvironmentVariable("CDK_DEFAULT_REGION"),
            }
        */
        
        return new Environment
        {
            Account = "YourAWSAccountId",
            Region = RegionEndpoint.EUWest1.SystemName
        };
    }
}
