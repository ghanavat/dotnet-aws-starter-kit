using Amazon.CDK;
using Constructs;
using Ghanavats.DotnetAws.IaC.Constructs;
using Ghanavats.DotnetAws.IaC.Props;

namespace Ghanavats.DotnetAws.IaC.Stacks;

public class IdentityStack : Stack
{
    public IdentityStack(Construct scope, string id, IdentityStackProps props) 
        : base(scope, id)
    {
        _ = new IamPermissionPolicyConstruct(this, id, new IdentityStackProps
        {
              AccountId = props.AccountId,
              Region = props.Region,
              ApiId = props.ApiId,
              Stage = props.Stage
        });
    }
}
