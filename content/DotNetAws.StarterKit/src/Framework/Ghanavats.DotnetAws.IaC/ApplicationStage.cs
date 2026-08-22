using Amazon.CDK;
using Constructs;

namespace Ghanavats.DotnetAws.IaC;

public class ApplicationStage : Stage
{
    public ApplicationStage(Construct scope, string id, IStageProps props) : base(scope, id, props)
    {
        //arn:aws:iam::aws:policy/AdministratorAccess
    }
}
