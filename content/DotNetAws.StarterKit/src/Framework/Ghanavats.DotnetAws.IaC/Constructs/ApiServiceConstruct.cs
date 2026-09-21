using Amazon.CDK;
using Amazon.CDK.AWS.APIGateway;
using Amazon.CDK.AWS.CodeDeploy;
using Amazon.CDK.AWS.Lambda;
using Amazon.CDK.AWS.Logs;
using Constructs;
using Ghanavats.DotnetAws.IaC.Props;
using AssetOptions = Amazon.CDK.AWS.S3.Assets.AssetOptions;
using LogGroupProps = Amazon.CDK.AWS.Logs.LogGroupProps;
using Runtime = Amazon.CDK.AWS.Lambda.Runtime;

namespace Ghanavats.DotnetAws.IaC.Constructs;

public sealed class ApiServiceConstruct : Construct
{
    private Function LambdaFunction { get; }
    public string ApiId { get; private set; } = string.Empty;

    public ApiServiceConstruct(Construct scope, string id, ServiceStackProps props)
        : base(scope, id)
    {
        const string lambdaProjectPath = "src/Presentation/Ghanavats.DotnetAws.Api";

        LambdaFunction = new Function(this, id, new FunctionProps
        {
            Runtime = Runtime.DOTNET_10,
            MemorySize = props.Settings.LambdaMemorySize,
            Handler = "Ghanavats.DotnetAws.Api",
            SnapStart = SnapStartConf.ON_PUBLISHED_VERSIONS,
            Environment = new Dictionary<string, string>
            {
                { "PEOPLE_TABLE_NAME", props.Table.TableName },
                { "ASPNETCORE_ENVIRONMENT", props.Settings.Name }
            },
            LogGroup = new LogGroup(this, "Ghanavats.DotnetAws_LogGroup", new LogGroupProps
            {
                Retention = RetentionDays.ONE_WEEK,
                RemovalPolicy = RemovalPolicy.DESTROY
            }),
            Code = Code.FromAsset("../../../", new AssetOptions
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
                        $" && dotnet restore {lambdaProjectPath}/Ghanavats.DotnetAws.Api.csproj" +
                        $" && dotnet lambda package --project-location {lambdaProjectPath} --configuration Release --output-package /asset-output/ghanavats.dotnetaws_function.zip"
                    ]
                }
            })
        });

        props.Table.GrantReadWriteData(LambdaFunction);

        // used to make sure each CDK synthesis produces a different Version
        var alias = new Alias(this, "LambdaAlias", new AliasProps
        {
            AliasName = "Dev",
            Version = LambdaFunction.CurrentVersion,
            Description = "Development alias for Lambda function"
        });

        _ = new LambdaDeploymentGroup(this, "DeploymentGroup", new LambdaDeploymentGroupProps
        {
            Alias = alias,
            DeploymentConfig = LambdaDeploymentConfig.ALL_AT_ONCE
        });

        CreateRestApi(this, "ApiServiceStack", props.Settings.Name, LambdaFunction);
    }

    private void CreateRestApi(Construct scope, string id, string environmentName, IFunction lambdaFunction)
    {
        var restApiProps = new LambdaRestApiProps
        {
            Handler = lambdaFunction,
            Proxy = true,
            RestApiName = "Ghanavats.DotnetAws.Api",
            Description = "API Gateway to interact with Lambda Rest API",
            ApiKeySourceType = ApiKeySourceType.HEADER,
            EndpointTypes = [EndpointType.REGIONAL],
            Deploy = true,
            IntegrationOptions = new LambdaIntegrationOptions
            {
                AllowTestInvoke = false,
                PassthroughBehavior = PassthroughBehavior.WHEN_NO_TEMPLATES
            },
            DefaultMethodOptions = new MethodOptions
            {
                AuthorizationType = AuthorizationType.IAM
            },
            DeployOptions = new StageOptions
            {
                StageName = environmentName,
                Description = "Development stage",
                DataTraceEnabled = true,
                MetricsEnabled = true
            }
        };

        var api = new LambdaRestApi(scope, id, restApiProps);
        ApiId = api.RestApiId;
        
        var usagePlan = api.AddUsagePlan("usagePlan", new UsagePlanProps
        {
            Name = "Ghanavats.DotnetAws.Api_UsagePlan",
            Description = "Usage Plan for the API",
            Throttle = new ThrottleSettings
            {
                RateLimit = 10,
                BurstLimit = 2
            },
            ApiStages =
            [
                new UsagePlanPerApiStage
                {
                    Api = api,
                    Stage = api.DeploymentStage
                }
            ]
        });

        var apiKey = api.AddApiKey("ApiKey", new ApiKeyProps
        {
            ApiKeyName = "application_apikey"
        });

        usagePlan.AddApiKey(apiKey);
    }
}
