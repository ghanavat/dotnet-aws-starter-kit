# Ghanavats.DotnetAws

A production-shaped .NET API on AWS Lambda, generated with Clean Architecture.

This solution was generated from the [Ghanavats .NET AWS Starter Kit](https://github.com/ghanavat/dotnet-aws-starter-kit). It gives you a working HTTP API running on Lambda behind API Gateway,
storing data in DynamoDB, with all infrastructure defined in C# using AWS CDK.

It is not a hello-world. The layering, the dependency rules, the tests, and the cold-start handling are the parts that usually take a week to get right on a new project. They're already done.

---

## What's in the box

- **Minimal API on AWS Lambda**, fronted by API Gateway
- **DynamoDB** for persistence, with the data access sitting behind an interface the domain owns
- **AWS CDK in C#** — your infrastructure is in the same language and the same solution as your application
<!--#if(architecture=='clean-arc')-->
- **Clean Architecture layers** with the dependency rule enforced by architecture tests, not by convention and hope
<!--#endif-->
<!--#if(architecture=='vertical-slice')-->
- **Vertical Slice** architecture for features, not horizontal layers of controllers, services, and repositories
<!--#endif-->
- **Cold-start handling** via Lambda SnapStart and a warmup path
- **A working sample feature** end to end, so the structure is demonstrated rather than described
- **Unit and architecture tests** wired up from the first commit

---

## Prerequisites

You need all of these before anything below will work. The CDK ones catch most people out.

### 1. .NET SDK

**.NET 10.0 SDK**

```bash
dotnet --version
```

If that fails, install from [here](https://dotnet.microsoft.com/en-us/download).

### 2. Node.js and the AWS CDK CLI

The CDK CLI is a Node package, even though your CDK code is C#. There is no way around this.

```bash
# Node.js 20 or later
node --version

# Install the CDK CLI globally
npm install -g aws-cdk

# Verify
cdk --version
```

If you don't have Node, install it from [here](https://nodejs.org/en/download).

### 3. AWS CLI, configured with credentials

```bash
aws --version
```

Install it from [here](https://docs.aws.amazon.com/cli/latest/userguide/getting-started-install.html) if missing, then configure credentials:

```bash
# Static credentials
aws configure

# Or IAM Identity Centre / SSO
aws configure sso
```

Confirm you're pointing at the account and region you expect:

```bash
aws sts get-caller-identity
```

Read that output before you deploy. Deploying into the wrong account is the most common and most annoying mistake here.

### 4. Amazon Lambda Tools

Used to build and package the Lambda deployment artifact.

```bash
dotnet tool install -g Amazon.Lambda.Tools

# If already installed
dotnet tool update -g Amazon.Lambda.Tools
```

### 5. Bootstrap your AWS account for CDK

**One time per account/region combination.** CDK cannot deploy without it. If you skip this, `cdk deploy` fails with an error about a missing staging bucket, and the message is not obvious.

```bash
cdk bootstrap aws://<YOUR_ACCOUNT_ID>/<YOUR_REGION>
```

For example:

```bash
cdk bootstrap aws://123456789012/eu-west-2
```

Bootstrapping creates a small CloudFormation stack (`CDKToolkit`) holding an S3 bucket and an ECR repository that CDK uses to stage deployment assets. It costs effectively nothing but it is not free,
and it stays in your account until you delete it.

### 6. IAM permissions

The identity you deploy with needs permission to create and manage: CloudFormation stacks, Lambda functions, API Gateway APIs, DynamoDB tables, IAM roles, and CloudWatch log groups.

An administrator identity works for getting started. **Do not use an administrator identity for a real deployment pipeline.** Scope it down before this goes anywhere near a shared account.

---

## Quick start

```bash
# 1. Restore dependencies
dotnet restore

# 2. Build
dotnet build

# 3. Run the tests
dotnet test
```

All three should pass on a freshly generated solution. If they don't, that's a bug in the template — please [open an issue](https://github.com/ghanavat/dotnet-aws-starter-kit/issues).

### Running the API locally

```bash
dotnet run --project src/Presentation/Ghanavats.DotnetAws.Api
```

The API starts on the URL printed in the console.

---

## Project structure

<!--#if(architecture=='clean-arc')-->
```
Ghanavats.DotnetAws/
├── src
│   ├── Presentation
│   │   └── Ghanavats.DotnetAws.Api/             # Minimal API endpoints, Lambda entry point, DI
│   ├── Core
│   │   └── Ghanavats.DotnetAws.Core/            # Entities, value objects, domain rules
│   ├── Application
│   │   └── Ghanavats.DotnetAws.UseCases/        # Use cases, abstractions, orchestration
│   ├── Framework
│   │   └── Ghanavats.DotnetAws.Infrastructure/  # DynamoDB, AWS SDK, external concerns
│   │   └── Ghanavats.DotnetAws.IaC/             # CDK stacks — the AWS infrastructure
│   ├── Shared
│       └── Ghanavats.DotnetAws.Shared/          # For all things needed across all layers and projects
├── tests
│   └── Ghanavats.DotnetAws.Api.Tests/
│   └── Ghanavats.DotnetAws.Core.Tests/
│   └── Ghanavats.DotnetAws.Infrastructure.Tests/
│   └── Ghanavats.DotnetAws.UseCases.Tests/
│   └── Ghanavats.DotnetAws.ArchitectureTests/
└── Ghanavats.DotnetAws.sln
```
<!--#endif-->

<!--#if(architecture=='vertical-slice')-->
```
Ghanavats.DotnetAws/
├── src/
│   ├── Presentation
│   │    └── Ghanavats.DotnetAws.Api/            # Minimal API endpoints, Lambda entry point, Features, Repositories, Domain Entities and DI
│   │        ├── Features
│   │        │  └── GetPersonDetails
│   │        │    └── Repositories
│   │        └── DomainEntities
│   ├── Framework
│       └── Ghanavats.DotnetAws.IaC/             # CDK stacks — the AWS infrastructure
├── tests/
│   └── Ghanavats.DotnetAws.Api.Tests/
└── Ghanavats.DotnetAws.sln
```
<!--#endif-->

<!--#if(architecture=='clean-arc')-->
### The dependency rule

Dependencies point inward. Nothing else is negotiable in this structure:

```
Api  ──▶  Application  ──▶  Domain
             ▲
Infrastructure ──┘
```

- **Core** references nothing. No AWS SDK, no Entity Framework, no ASP.NET. If you find yourself adding a package reference here, stop and reconsider.
- **Application - UseCases** references Core only. It defines the interfaces it needs — repositories, clocks, external services — and never knows who implements them.
- **Infrastructure** implements the interfaces Application declared. This is where the AWS SDK and DynamoDB live. Application does not reference Infrastructure.
- **Api** is the composition root. It wires the implementations to the interfaces at startup and exposes the HTTP surface.
- **IaC** is a standalone project that describes your AWS resources. It does not participate in the application dependency graph.

`DotNetAws.StarterKit.ArchitectureTests` asserts these rules. Break one and `dotnet test` fails, 
so the layering gets checked automatically instead of relying on code review to catch it. Run it in CI and make it a required check if you want it to actually block anything.
<!--#endif-->

---

## The sample feature

### **Get Person Details**

The solution ships with a working **GetPersonDetails** feature so you can see the structure carrying real weight instead of guessing from folder names.

It exercises the full path through the layers:
<!--#if(architecture=='clean-arc')-->
1. **Api** — `[GetPersonDetailsEndpoint].cs` defines the HTTP endpoint. It validates the request shape, calls into Application, and maps the result to a response. There's no business logic here.
2. **Application - UseCases** — `[GetPersonDetailsUseCase]` handles the use case. It orchestrates the work and depends only on the `[IPeopleRepository]` abstraction it defines itself.
3. **Core** — `[Person]` holds the state and the rules that must always hold true. Invariants are protected in the type, not checked in a service somewhere.
4. **Infrastructure** — `[PeopleRepository]` implements `[IPeopleRepository]` against DynamoDB. Swapping the storage engine means changing this class and nothing above it.
<!--#endif-->

<!--#if(architecture=='vertical-slice')-->
- **Api** — `[GetPersonDetailsEndpoint].cs` defines the HTTP endpoint. It validates the request shape, calls into Application, and maps the result to a response. There's no business logic here.
  - **Features** - `[GetPersonDetailsUseCase].cs` handles the use case. It orchestrates the work and depends only on the `[IPeopleRepository]` abstraction it defines itself.
  - **Features/GetPersonDetailsUseCase/Repositoies** - `[PeopleRepository].cs` implements `[IPeopleRepository]` against DynamoDB. Swapping the storage engine means changing this class and nothing above it.
  - **DomainEntities** - `[Person]` holds the state and the rules that must always hold true. Invariants are protected in the type, not checked in a service somewhere.
<!--#endif-->

Delete it once you understand the shape. It's a reference implementation, not a foundation you're expected to build on.

**Try it:**

```bash
curl -X POST <YOUR_API_URL>/[route] \
  -H "Content-Type: application/json" \
  -d '{ "example": "payload" }'
```

---

## Deploying to AWS

From the CDK project directory:

```bash
cd src/Framework/Ghanavats.DotnetAws.IaC
```

### See what will be created

```bash
cdk synth
```

This generates the CloudFormation template without deploying anything. Read it the first time. You're about to create resources in your own account and it's worth knowing what they are.

### Deploy

The solution is already configured to deploy into a `Dev` environment. 
You can change that in the CDK context if you want, but the default is fine for getting started.

This is done by using Stages in CDK. 
The `Dev` stage is defined in `src/Framework/Ghanavats.DotnetAws.IaC/Stages/ApplicationStage.cs` and called in `src/Framework/Ghanavats.DotnetAws.IaC/Program.cs`. 

The stacks are defined in `src/Framework/Ghanavats.DotnetAws.IaC/Stacks`.

Deploy all application stacks:

```bash
cdk deploy 'Dev/*' --parameters "<IdentityStackName>:TrustedClientPrincipalArn:<TrustedArn>"
```

Deploy specific stack by name. 
This is very useful when you have multiple stacks and want to deploy only one of them. 
For example, if you want to deploy only the API stack (Lambda as per this Starter Kit) and NOT the DynamoDB stack, run:

```bash
cdk deploy 'Dev/ApiServiceStack'
```

CDK shows you the IAM changes and asks for confirmation. Approve it, and the deployment takes a few minutes.

When it finishes, the API Gateway URL is printed as a stack output. Call it:

```bash
curl <YOUR_API_URL>/[route]
```

### What gets created

| Resource | Purpose |
|---|---|
| Lambda function | Runs the API |
| Lambda version + alias | Required for SnapStart — see below |
| API Gateway | HTTP front door |
| DynamoDB table | Persistence |
| IAM role | Lambda execution permissions |
| IAM role | Lambda execution permissions |
| CloudWatch log group | Function logs |

---

## Cold starts: SnapStart and warmup

.NET on Lambda has real initialisation work to do before it can answer the first request. This solution addresses that in two ways.

### SnapStart

<cite>Lambda initialises your function when you publish a function version, takes a snapshot of the memory and disk state of the initialised execution environment, encrypts it, and caches it.</cite>
Subsequent cold starts restore from the snapshot instead of re-running initialisation.

Things you need to know, because they constrain how you deploy:

- **SnapStart only works on published function versions and aliases that point to versions. It does not work on `$LATEST`.** The CDK stack publishes a version and creates an alias for this reason.
  If you invoke the unqualified function, you get no SnapStart benefit.
- **SnapStart requires .NET 8 or later on a Lambda managed runtime.** Container image deployments are not supported.
- **For .NET, x86_64 only.** ARM64 SnapStart support is Java-only. Don't switch this function to ARM.
- **SnapStart and provisioned concurrency are mutually exclusive.** Pick one.
- **For .NET, SnapStart has caching and restoration charges.** It is not free the way it is for Java. Check current Lambda pricing before assuming it's cheaper than the alternative for your traffic pattern.
- **If you use `Amazon.Lambda.Annotations`, you need version 1.6.0 or later** for SnapStart compatibility.

There's one correctness trap worth understanding before you add initialisation code. A single snapshot becomes the starting state for many execution environments —
so anything unique generated during initialisation gets duplicated across all of them. <cite>Unique IDs, secrets, and entropy used for pseudorandomness must be generated after initialisation, not during it.</cite>
The same applies to anything holding connection state or a time-sensitive credential.

### Warmup

SnapStart reduces the cost of a cold start. It doesn't remove it. The solution also includes a warmup path so the function can be kept ready.

> **[Describe your warmup extension here — what triggers it, what it touches, whether the schedule is configurable, and how to disable it.]**

---

## Configuration

> **[List your actual configuration surface here — environment variables, appsettings keys, CDK context parameters. Include the table name, region, and log level at minimum.]**

| Setting | Where | Default | Purpose |
|---|---|---|---|
| `[SETTING_NAME]` | `[appsettings.json / env var / CDK context]` | `[default]` | `[what it does]` |

---

## Testing

```bash
# Generate report for the code coverage output - install the following package globally
dotnet tool install -g dotnet-reportgenerator-globaltool

# Everything
dotnet test

# One project
dotnet test tests/Ghanavats.DotnetAws.Api.Tests

# With coverage across all projects
dotnet test --coverlet --coverlet-include "[Ghanavats.DotnetAws.*]*" --coverlet-exclude "*[*.Tests]"
```

Run the report:
```bash
reportgenerator -reports:"TestResults/*.xml" -targetdir:"coveragereport" -reporttypes:Html
```

**Unit tests** cover domain rules and application use cases. They have no AWS dependency and run in milliseconds.

<!--#if(architecture=='clean-arc')-->
**Architecture tests** assert the dependency rule described above. Add a forbidden reference and dotnet test fails. 
These run as tests, not as a build step — they catch nothing unless your pipeline runs `dotnet test` and blocks on failure. Make it a required status check on your default branch.
<!--#endif-->
---

## Calling the API with IAM authorization

The generated API uses two complementary access controls:

- **AWS IAM authorization** validates the caller’s AWS identity and checks whether it has permission to invoke the requested API Gateway route.
- **An API key** identifies the API client and applies the configured usage-plan throttling and quotas.

A successful request must be signed with AWS Signature Version 4 (SigV4) and include a valid `x-api-key` header.

### 1. Select the trusted AWS principal

Before deployment, identify the IAM user or role that will be permitted to assume the generated API invocation role:

```bash
aws sts get-caller-identity
```

An IAM user ARN can be used directly:

```text
arn:aws:iam::123456789012:user/username
```

If the command returns an STS session ARN:

```text
arn:aws:sts::123456789012:assumed-role/MyDeveloperRole/session-name
```

use the underlying IAM role ARN instead:

```text
arn:aws:iam::123456789012:role/MyDeveloperRole
```

Do not use an AWS account root ARN or the temporary `assumed-role` session ARN.

### 2. Deploy the application

The IAM stack requires the trusted principal ARN as a CloudFormation parameter.

First, list the CDK stacks:

```bash
cdk list
```

A development Stage might contain stacks similar to:

```text
Dev/ApiStack (Dev-ApiStack)
Dev/DatabaseStack (Dev-DatabaseStack)
Dev/IAMStack (Dev-IAMStack)
```

Deploy the Stage and pass the parameter specifically to the deployed IAM stack:

```bash
cdk deploy 'Dev/*' \
  --parameters 'Dev-IAMStack:TrustedClientPrincipalArn=arn:aws:iam::123456789012:role/MyDeveloperRole'
```

For an IAM user:

```bash
cdk deploy 'Dev/*' \
  --parameters 'Dev-IAMStack:TrustedClientPrincipalArn=arn:aws:iam::123456789012:user/username'
```

Replace `Dev-IAMStack` with the deployed stack name shown in parentheses by `cdk list`.

### 3. Assume the API invocation role

The IAM stack creates the following dedicated role:

```text
ApplicationIAMStack_IAM_Role
```

The trusted principal assumes this role before calling the API via AWS CLI:

```bash
aws sts assume-role \
  --role-arn arn:aws:iam::123456789012:role/ApplicationIAMStack_IAM_Role \
  --role-session-name dotnet-aws-api-client
```

AWS STS returns temporary credentials:

- `AccessKeyId`
- `SecretAccessKey`
- `SessionToken`
- `Expiration`

Do not store these credentials in source control. They expire and must be refreshed when necessary.

A named AWS CLI profile can manage role assumption and credential refreshing automatically. Add a profile to the AWS configuration file:

```ini
[profile dotnet-aws-api]
role_arn = arn:aws:iam::123456789012:role/ApplicationIAMStack_IAM_Role
source_profile = default
role_session_name = dotnet-aws-api-client
region = eu-west-1
```

Replace `default` with the profile representing the trusted IAM principal when necessary.

Verify the assumed identity:

```bash
aws sts get-caller-identity --profile dotnet-aws-api
```

The returned ARN should contain:

```text
assumed-role/ApplicationIAMStack_IAM_Role/
```

If you want to assume role in .NET, you can do something like this:

```csharp
var chain = new CredentialProfileStoreChain();

if (!chain.TryGetAWSCredentials( 
        "default",
        out var sourceCredentials))
{
    throw new InvalidOperationException(
        "The default AWS profile could not be loaded.");
}
        
using var stsClient = new AmazonSecurityTokenServiceClient(
    sourceCredentials,
    RegionEndpoint.EUWest1);

var response = await stsClient.AssumeRoleAsync(
    new AssumeRoleRequest
    {
        RoleArn =
            "arn:aws:iam::123456789012:role/ApplicationIAMStack_IAM_Role",

        RoleSessionName = "dotnet-aws-api-client"
    },
    CancellationToken.None);
```
**How the code snippet above works**
- First constructing a `CredentialProfileChain` object
- Then we try to obtain/populate the credentials (AWSCredentials) by calling `TryGetAWSCredentials`
- The AWSCredentials object is then used to create an STS Client - `AmazonSecurityTokenServiceClient`
- Finally a call to `AssumeRoleAsync` is made to create temporary credentials

### 4. Obtain the API endpoint and API key

Obtain the following values from the CDK deployment outputs or the AWS Console:

- The API Gateway base URL
- The AWS Region
- The API key
- A valid person ID

The API URL follows this format:

```text
https://<api-id>.execute-api.<region>.amazonaws.com/<api-stage>
```

Keep the API key out of source control, application logs, screenshots, and committed configuration files.

An API key is not an authentication credential. IAM and SigV4 provide authorization; the API key is used for client identification, throttling, and usage-plan management.

### 5. Send a signed request

Use an AWS SDK or another SigV4-capable HTTP client. Avoid constructing the `Authorization` header manually.

For example, `curl` versions with SigV4 support can call the API using the temporary credentials returned by STS:

```bash
curl \
  --aws-sigv4 "aws:amz:<aws-region>:execute-api" \
  --user "$AWS_ACCESS_KEY_ID:$AWS_SECRET_ACCESS_KEY" \
  --header "x-amz-security-token: $AWS_SESSION_TOKEN" \
  --header "x-api-key: $API_KEY" \
  "https://<api-id>.execute-api.<aws-region>.amazonaws.com/<api-stage>/api/<person-id>"
```

The signing service must be:

```text
execute-api
```

The signing Region must match the Region in which API Gateway was deployed.

Because the request uses temporary STS credentials, the `x-amz-security-token` header is required. SigV4-capable AWS SDK clients include this token automatically.

The generated IAM policy grants access only to the explicitly configured HTTP methods and resource paths. A correctly signed request will still be rejected if its method or path is outside the role’s `execute-api:Invoke` permission.

### Troubleshooting

| Result | Likely cause |
|---|---|
| `403 Forbidden` | Missing or invalid API key, insufficient IAM permission, or an invalid signature |
| `User is not authorized to perform execute-api:Invoke` | The wrong credentials were used, or the method/path is outside the generated IAM policy |
| Invalid security token | The STS session token is missing, invalid, or expired |
| Signature mismatch | The wrong Region or service name was used, headers changed after signing, or the machine clock is inaccurate |
| Missing authentication token | The API stage, HTTP method, or resource path may be incorrect |
| `AccessDenied` from `sts:AssumeRole` | The current principal does not match the trusted principal configured during deployment |

For additional information, see:

- [Control access to API Gateway using IAM](https://docs.aws.amazon.com/apigateway/latest/developerguide/permissions.html)
- [AWS Signature Version 4](https://docs.aws.amazon.com/IAM/latest/UserGuide/reference_sigv.html)
- [Assume an IAM role using the AWS CLI](https://docs.aws.amazon.com/IAM/latest/UserGuide/id_roles_use_switch-role-cli.html)
- [API Gateway usage plans and API keys](https://docs.aws.amazon.com/apigateway/latest/developerguide/api-gateway-api-usage-plans.html)

## Tearing it down

```bash
cd src/Framework/Ghanavats.DotnetAws.IaC
cdk destroy 'Dev/*'
```

**Do this when you're done evaluating.** The deployed stack is small and mostly falls inside the AWS Free Tier at zero traffic, but "mostly" is not "certainly",
and an idle DynamoDB table plus CloudWatch log retention will eventually appear on a bill.

Two things `cdk destroy` may leave behind depending on your retention settings:

- **The DynamoDB table**, if the removal policy retains it. Check the AWS console and delete it manually if so.
- **CloudWatch log groups.** These persist and accumulate. Delete them if you don't want them.

The `CDKToolkit` bootstrap stack also stays in your account. That's intentional — it's shared across all CDK projects in that account and region. Leave it unless you're removing CDK entirely.

---

## Troubleshooting

**`cdk deploy` fails mentioning a missing bucket or SSM parameter**
You haven't bootstrapped. Run `cdk bootstrap aws://<ACCOUNT>/<REGION>`.

**`cdk: command not found`**
The CDK CLI isn't installed or isn't on your PATH. `npm install -g aws-cdk`.

**`Unable to determine service/operation name to be authorized`, or credentials errors**
Your AWS credentials aren't configured or have expired. Run `aws sts get-caller-identity` to confirm.

**Deployed, but cold starts are unchanged**
You're almost certainly invoking `$LATEST` rather than the published version alias. SnapStart does nothing on `$LATEST`.

**`arm64 is not supported for SnapStart enabled functions`**
The function architecture has been changed to ARM64. SnapStart for .NET requires x86_64.

**Tests fail immediately after generating**
That's a template bug, not something you did. Please [open an issue](https://github.com/ghanavat/dotnet-aws-starter-kit/issues) with your .NET version and OS.

---

## Feedback

This starter kit is free and open source, and it improves when people tell me what's broken in it.

If the deployment path wasn't clear, if the structure doesn't match how you'd actually build this, or if something simply didn't work — [open an issue](https://github.com/ghanavat/dotnet-aws-starter-kit/issues). 
Critical feedback is more useful to me than a star.

Built by [Saeed Ghanavat](https://ghanavats.tech) · [Ghanavats Tech](https://ghanavats.tech)

---

## License

Apache-2.0. See [LICENSE](LICENSE).
