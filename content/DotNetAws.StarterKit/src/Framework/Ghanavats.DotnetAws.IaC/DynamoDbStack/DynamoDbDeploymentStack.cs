using Amazon.CDK;
using Amazon.CDK.AWS.DynamoDB;
using Constructs;
using Ghanavats.DotnetAws.IaC.LambdaStack;
using Attribute = Amazon.CDK.AWS.DynamoDB.Attribute;

namespace Ghanavats.DotnetAws.IaC.DynamoDbStack;

public class DynamoDbDeploymentStack : Stack
{
    public static TableV2 DynamoDbTableObject { get; private set; }
    
    public DynamoDbDeploymentStack(Construct scope, string id, IStackProps props = null) 
        : base(scope, id, props)
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
                MaxReadRequestUnits =  5,
                MaxWriteRequestUnits = 5
            }),
            RemovalPolicy = RemovalPolicy.DESTROY,
            DeletionProtection = true
        });

        dynamoDbTable.GrantReadWriteData(LambdaDeploymentStack.CleanArchitectureLambda);
        LambdaDeploymentStack.CleanArchitectureLambda.AddEnvironment("PEOPLE_TABLE_NAME", dynamoDbTable.TableName);
        
        DynamoDbTableObject = dynamoDbTable;
    }
}
