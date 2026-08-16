using Amazon.CDK;
using Amazon.CDK.AWS.CodeDeploy;
using Amazon.CDK.AWS.Lambda;
using Amazon.CDK.AWS.Logs;
using Constructs;

namespace Ghanavats.DotnetAws.IaC.LambdaStack;

public class LambdaDeploymentStack : Stack
{
    internal static Function? CleanArchitectureLambda;

    internal LambdaDeploymentStack(Construct scope, string id, IStackProps? props = null) : base(scope, id, props)
    {
        const string apiProjectPath = "src/Presentation/Ghanavats.DotnetAws.Api";

        var lambdaFunction = new Function(this, "CleanArchitecture_Function", new FunctionProps
        {
            Runtime = Runtime.DOTNET_10,
            MemorySize = 2048,
            Handler = "Ghanavats.DotnetAws.Api",
            SnapStart = SnapStartConf.ON_PUBLISHED_VERSIONS,
            LogGroup = new LogGroup(this, "CleanArchitecture_LogGroup", new LogGroupProps
            {
                LogGroupName = "/aws/lambda/CleanArchitecture_Function",
                Retention = RetentionDays.ONE_WEEK,
                RemovalPolicy = RemovalPolicy.DESTROY
            }),
            Code = Code.FromAsset("../../../", new Amazon.CDK.AWS.S3.Assets.AssetOptions
            {
                Bundling = new BundlingOptions
                {
                    Image = Runtime.DOTNET_10.BundlingImage,
                    User = "root",
                    OutputType = BundlingOutput.ARCHIVED,
                    Command =
                    [
                        "/bin/sh",
                        "-c",
                        "mkdir -p /tmp/build" +
                        " && cp -R /asset-input/. /tmp/build" +
                        " && cd /tmp/build" +
                        " && dotnet tool install -g Amazon.Lambda.Tools" +
                        " && export PATH=\"$PATH:/root/.dotnet/tools\"" +
                        " && export DOTNET_CLI_HOME=/tmp" +
                        " && export NUGET_PACKAGES=/tmp/nuget" +
                        $" && dotnet restore {apiProjectPath}/Ghanavats.DotnetAws.Api.csproj" +
                        $" && dotnet lambda package --project-location {apiProjectPath} --configuration Release --output-package /asset-output/cleanarchitecture_function.zip"
                    ]
                }
            })
        });

        // used to make sure each CDK synthesis produces a different Version
        var version = lambdaFunction.CurrentVersion;
        var alias = new Alias(this, "LambdaAlias", new AliasProps
        {
            AliasName = "Dev",
            Version = version,
            Description = "Development alias for the CleanArchitecture Lambda function"
        });

        _ = new LambdaDeploymentGroup(this, "DeploymentGroup", new LambdaDeploymentGroupProps
        {
            Alias = alias,
            DeploymentConfig = LambdaDeploymentConfig.ALL_AT_ONCE
        });

        CleanArchitectureLambda = lambdaFunction;
    }
}
