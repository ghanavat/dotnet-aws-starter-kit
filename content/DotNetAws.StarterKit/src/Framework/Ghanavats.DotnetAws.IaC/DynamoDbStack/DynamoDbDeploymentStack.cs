using Amazon.CDK;
using Amazon.CDK.AWS.DynamoDB;
using Constructs;
using Ghanavats.DotnetAws.IaC.LambdaStack;
using Attribute = Amazon.CDK.AWS.DynamoDB.Attribute;

namespace Ghanavats.DotnetAws.IaC.DynamoDbStack;

public class DynamoDbDeploymentStack : Stack
{
    public DynamoDbDeploymentStack(Construct scope, string id, IStackProps? props = null) : base(scope, id, props)
    {
        var dynamoDbTable = new TableV2(this, "people_table", new TablePropsV2
        {
            TableName = "People",
            PartitionKey = new Attribute
            {
                Name = "PersonId",
                Type = AttributeType.STRING
            },
            Billing = Billing.OnDemand(new MaxThroughputProps
            {
                /*these need raising
                 for anything beyond local experimentation*/
                MaxReadRequestUnits = 5,
                MaxWriteRequestUnits = 5
            }),
            RemovalPolicy = RemovalPolicy.DESTROY
        });

        if (LambdaDeploymentStack.DotnetAwsLambda is null)
        {
            throw new InvalidOperationException("Lambda function is not initialised. Ensure that the LambdaDeploymentStack is deployed before the DynamoDbDeploymentStack.");
        }
        
        dynamoDbTable.GrantReadWriteData(LambdaDeploymentStack.DotnetAwsLambda);
        LambdaDeploymentStack.DotnetAwsLambda.AddEnvironment("PEOPLE_TABLE_NAME", dynamoDbTable.TableName);
    }
}
