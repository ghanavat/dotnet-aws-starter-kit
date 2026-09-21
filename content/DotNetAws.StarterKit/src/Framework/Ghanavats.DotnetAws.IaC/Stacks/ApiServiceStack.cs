using Amazon.CDK;
using Amazon.CDK.AWS.DynamoDB;
using Constructs;
using Ghanavats.DotnetAws.IaC.Constructs;
using Ghanavats.DotnetAws.IaC.Props;

namespace Ghanavats.DotnetAws.IaC.Stacks;

internal class ApiServiceStack : Stack
{
    public string ApiId { get;}
    
    internal ApiServiceStack(Construct scope, string id,
        EnvironmentSettings settings,
        ITableV2 table,
        IStackProps? props = null)
        : base(scope, id, props)
    {
        var apiServiceConstruct = new ApiServiceConstruct(this, id, new ServiceStackProps
        {
            Settings = settings,
            Table = table
        });
        ApiId = apiServiceConstruct.ApiId;
    }
}
