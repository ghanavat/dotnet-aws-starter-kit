using Amazon.CDK.AWS.IAM;
using Constructs;
using Ghanavats.DotnetAws.IaC.Props;

namespace Ghanavats.DotnetAws.IaC.Constructs;

public class IamPermissionPolicyConstruct : Construct
{
    public IamPermissionPolicyConstruct(Construct scope, string id, IdentityStackProps props) : base(scope, id)
    {
        _ = new Policy(this, "IAM_Permission_Policy", new PolicyProps
        {
            Document = new PolicyDocument(new PolicyDocumentProps
            {
                Statements = [new PolicyStatement(new PolicyStatementProps
                {
                    Sid = "Ghanavats.DotnetAws_ApiGateway_IAM_Permission_Policy",
                    Effect = Effect.ALLOW,
                    Actions = ["execute-api:Invoke"],
                    Resources = [$"arn:aws:execute-api:{props.Region}:{props.AccountId}:{props.ApiId}/{props.Stage}/HTTP_METHOD/PATH"]
                })]
            })
        });
    }
}
