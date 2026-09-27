using Amazon.CDK;
using Amazon.CDK.AWS.IAM;
using Constructs;
using Ghanavats.DotnetAws.IaC.Props;

namespace Ghanavats.DotnetAws.IaC.Constructs;

public class IamPermissionPolicyConstruct : Construct
{
    public IamPermissionPolicyConstruct(Construct scope, string id, IdentityStackProps props) : base(scope, id)
    {
        var role = new Role(this, $"{id}_Role", new RoleProps
        {
            RoleName = $"{id}_IAM_Role",
            Description = "An IAM Role used to assume Permission Policy which is used to control access to the API.",
            AssumedBy = new ArnPrincipal(props.TrustedClientPrincipalArn)
        });

        role.AddToPolicy(new PolicyStatement(new PolicyStatementProps
        {
            Sid = "AllowGetPersonDetails",
            Effect = Effect.ALLOW,
            Actions = ["execute-api:Invoke"],
            Resources =
            [
                $"arn:{Aws.PARTITION}:execute-api:{props.Region}:{props.AccountId}:{props.ApiId}/{props.Stage}/api/GET/*"
            ]
        }));
    }
}
