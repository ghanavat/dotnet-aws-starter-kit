using Constructs;
using Ghanavats.DotnetAws.IaC.Props;

namespace Ghanavats.DotnetAws.IaC.Constructs;

public class IamPermissionPolicyConstruct : Construct
{
    public IamPermissionPolicyConstruct(Construct scope, string id, IdentityStackProps props) : base(scope, id)
    {
    }
}
