using Amazon.CDK;
using Constructs;
using Ghanavats.DotnetAws.IaC.Stacks;

namespace Ghanavats.DotnetAws.IaC.Stages;

public sealed class ApplicationStage : Stage
{
    public ApplicationStage(Construct scope, 
        string id,
        Amazon.CDK.Environment environment,
        EnvironmentSettings settings) 
        : base(scope, id, new StageProps
        {
            Env = environment
        })
    {
        var dynamoDbStack = new DynamoDbStack(this, "DynamoDbStack", settings, new StackProps
        {
            StackName = $"application-{settings.Name}-dynamodb",
            TerminationProtection = settings.StackTerminationProtection
        });
        
        _ = new ApiServiceStack(this, "ApiServiceStack", settings, 
            dynamoDbStack.PeopleTable, new StackProps
            {
                StackName = $"application-{settings.Name}-api",
                TerminationProtection = settings.StackTerminationProtection
            });
    }
}
