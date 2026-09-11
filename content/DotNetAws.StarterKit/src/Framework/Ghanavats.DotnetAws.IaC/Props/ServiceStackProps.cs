using Amazon.CDK;
using Amazon.CDK.AWS.DynamoDB;

namespace Ghanavats.DotnetAws.IaC.Props;

public sealed class ServiceStackProps : IStackProps
{
    public required EnvironmentSettings Settings { get; init; }
    public required ITableV2 Table { get; init; }
}
