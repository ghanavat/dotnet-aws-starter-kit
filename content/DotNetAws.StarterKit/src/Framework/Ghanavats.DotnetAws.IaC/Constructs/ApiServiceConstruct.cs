using Amazon.CDK;
using Amazon.CDK.AWS.APIGateway;
using Amazon.CDK.AWS.CodeDeploy;
using Amazon.CDK.AWS.Lambda;
using Amazon.CDK.AWS.Logs;
using Constructs;
using Ghanavats.DotnetAws.IaC.Props;

namespace Ghanavats.DotnetAws.IaC.Constructs;

public sealed class ApiServiceConstruct : Construct
{
    private Function LambdaFunction { get; }

    public ApiServiceConstruct(Construct scope, string id, ServiceStackProps props)
        : base(scope, id)
    {
        const string apiProjectPath = "src/Presentation/Ghanavats.DotnetAws.Api";

        LambdaFunction = new Function(this, "Ghanavats.DotnetAws_Function", new FunctionProps
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
                        $" && dotnet lambda package --project-location {apiProjectPath} --configuration Release --output-package /asset-output/ghanavats.dotnetaws_function.zip"
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

        CreateRestApi(this, "ApiServiceStack", LambdaFunction);
    }

    private static void CreateRestApi(Construct scope, string id, IFunction lambdaFunction)
    {
        var api = new LambdaRestApi(scope, id, new LambdaRestApiProps
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
                ApiKeyRequired = true,
                // AuthorizationType = AuthorizationType.COGNITO, // COMING SOON
            },
            DeployOptions = new StageOptions
            {
                StageName = "dev",
                Description = "Development stage",
                DataTraceEnabled = true,
                MetricsEnabled = true
            }
        });

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
            /*Value: Not setting a value for the Value property will create the API Key with auto generated value. Ideal.*/
        });

        usagePlan.AddApiKey(apiKey);
    }
}
