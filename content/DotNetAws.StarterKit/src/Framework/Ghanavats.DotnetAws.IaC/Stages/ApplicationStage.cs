using Amazon.CDK;
using Constructs;
using Ghanavats.DotnetAws.IaC.Stacks;
using Environment = Amazon.CDK.Environment;

namespace Ghanavats.DotnetAws.IaC.Stages;

public sealed class ApplicationStage : Stage
{
    public ApplicationStage(Construct scope, 
        string id,
        Environment environment,
        EnvironmentSettings settings) 
        : base(scope, id, new StageProps
        {
            Env = environment
        })
    {
        var dynamoDbStack = new DynamoDbStack(this, "Ghanavats.DotnetAws_DynamoDbStack", settings, new StackProps
        {
            StackName = $"application-{settings.Name}-dynamodb",
            TerminationProtection = settings.StackTerminationProtection
        });
        
        _ = new ApiServiceStack(this, "Ghanavats.DotnetAws_ServiceStack", settings, dynamoDbStack.PeopleTable, props: new StackProps
        {
            StackName = $"application-{settings.Name}-api",
            TerminationProtection = settings.StackTerminationProtection
        });
    }
}
