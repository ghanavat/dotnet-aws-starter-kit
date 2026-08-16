using Amazon.CDK;
using Ghanavats.DotnetAws.IaC.ApiGatewayStack;
using Ghanavats.DotnetAws.IaC.DynamoDbStack;
using Ghanavats.DotnetAws.IaC.LambdaStack;

namespace Ghanavats.DotnetAws.IaC;

public static class StackInitializer
{
    public static void Apply(App app)
    {
        _ = new LambdaDeploymentStack(app, "LambdaDeploymentStack", new StackProps
        {
            Env = AwsEnvironmentCreator.SetEnvironment()
        });
        _ = new DynamoDbDeploymentStack(app, "DynamoDbStack", new StackProps
        {
            Env = AwsEnvironmentCreator.SetEnvironment()
        });
        _ = new ApiGatewayDeploymentStack(app, "ApiGatewayDeploymentStack", new StackProps
        {
            Env = AwsEnvironmentCreator.SetEnvironment()
        });
    }
}
