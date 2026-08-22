using Amazon.CDK;
using Amazon.CDK.AWS.APIGateway;
using Constructs;
using Ghanavats.DotnetAws.IaC.LambdaStack;
using StageProps = Amazon.CDK.AWS.APIGateway.StageProps;

namespace Ghanavats.DotnetAws.IaC.ApiGatewayStack;

public class ApiGatewayDeploymentStack : Stack
{
    internal ApiGatewayDeploymentStack(Construct scope, string id, IStackProps? props = null) : base(scope, id, props)
    {
        var apiGateway = new RestApi(this, "DotNetAws_StarterKit_Api", new RestApiProps
        {
            RestApiName = "DotNetAwsApi",
            Description = "API Gateway to interact with Lambda Rest API",
            ApiKeySourceType = ApiKeySourceType.HEADER,
            EndpointTypes = [EndpointType.REGIONAL],
            DefaultMethodOptions = new MethodOptions
            {
                ApiKeyRequired = true,
                OperationName = "GetPersonDetails"
            },
            Deploy = true,
            DeployOptions = new StageProps
            {
                StageName = "dev",
                Description = "Development stage"
            }
        });

        if (LambdaDeploymentStack.DotnetAwsLambda is null)
        {
            throw new InvalidOperationException("Lambda function is not initialised. Ensure that the LambdaDeploymentStack is deployed before the ApiGatewayDeploymentStack.");
        }
        
        apiGateway.Root.AddProxy(new ProxyResourceOptions
        {
            AnyMethod = true,
            DefaultIntegration = new LambdaIntegration(LambdaDeploymentStack.DotnetAwsLambda.CurrentVersion, new LambdaIntegrationOptions
            {
                AllowTestInvoke = false,
                PassthroughBehavior = PassthroughBehavior.WHEN_NO_TEMPLATES
            })
        });

        var usagePlan = apiGateway.AddUsagePlan("usagePlan", new UsagePlanProps
        {
            Name = "DotNetAws_UsagePlan",
            Description = "Usage Plan for the API",
            Throttle = new ThrottleSettings
            {
                RateLimit = 10,
                BurstLimit = 2
            },
            ApiStages =
            [
                new UsagePlanPerApiStage
                {
                    Api = apiGateway,
                    Stage = apiGateway.DeploymentStage
                }
            ]
        });

        var apiKey = apiGateway.AddApiKey("apiKey", new ApiKeyOptions
        {
            ApiKeyName = "dotnetaws_starter_kit_apikey",
            // Value: Not setting a value for the Value property will create the API Key with auto generated value. Ideal.
        });

        usagePlan.AddApiKey(apiKey);
    }
}
