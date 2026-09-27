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
        var trustedClientRoleArn = new CfnParameter(this, "TrustedClientPrincipalArn", new CfnParameterProps
        {
            Type = "String",
            Description =
                "ARN of the existing IAM user or role permitted to assume the API invocation role.",
            AllowedPattern =
                "^arn:(aws|aws-us-gov|aws-cn):iam::[0-9]{12}:(user|role)/.+$",
            ConstraintDescription = "The value must be a valid IAM user or role ARN."
        });
        
        _ = new IamPermissionPolicyConstruct(this, id, new IdentityStackProps
        {
              AccountId = props.AccountId,
              Region = props.Region,
              ApiId = props.ApiId,
              Stage = props.Stage,
              TrustedClientPrincipalArn = trustedClientRoleArn.ValueAsString
        });
    }
}
