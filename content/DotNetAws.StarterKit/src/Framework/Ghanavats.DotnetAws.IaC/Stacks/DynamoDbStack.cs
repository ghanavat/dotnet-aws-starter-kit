using Amazon.CDK;
using Amazon.CDK.AWS.DynamoDB;
using Constructs;
using Attribute = Amazon.CDK.AWS.DynamoDB.Attribute;

namespace Ghanavats.DotnetAws.IaC.Stacks;

public class DynamoDbStack : Stack
{
    public ITableV2 PeopleTable { get; }

    public DynamoDbStack(Construct scope, string id,
        EnvironmentSettings settings,
        IStackProps? props = null)
        : base(scope, id, props)
    {
        PeopleTable = new TableV2(this, "people_table", new TablePropsV2
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
            PointInTimeRecoverySpecification = new PointInTimeRecoverySpecification
            {
                PointInTimeRecoveryEnabled = settings.DynamoDbPointInTimeRecovery
            },
            RemovalPolicy = settings.IsProduction
                ? RemovalPolicy.RETAIN
                : RemovalPolicy.DESTROY,
            DeletionProtection = settings.DynamoDbDeletionProtection
        });
    }
}
