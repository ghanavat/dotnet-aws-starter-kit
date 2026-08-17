# .NET AWS Starter Kit
A configurable .NET 10 solution template for building and deploying serverless APIs on AWS. It combines an ASP.NET Core Minimal API with AWS Lambda, API Gateway, DynamoDB, and infrastructure defined in C# using AWS CDK.

This repository is the template authoring and NuGet packaging project. The solution that users receive from dotnet new lives under content/DotNetAws.StarterKit.

This starter kit is actively developed. Clean Architecture is the default generation mode. The Vertical Slice mode should be treated as experimental until both generated outputs are covered by automated build tests.

# Repository structure
| Path                                                        | Purpose                                                                           |
|-------------------------------------------------------------|-----------------------------------------------------------------------------------|
| Ghanavats.DotNetAws.Templates.csproj                        | Builds the NuGet template package.                                                |
| content/DotNetAws.StarterKit/                               | The source content copied into generated solutions.                               |
| content/DotNetAws.StarterKit/.template.config/template.json | Defines the template identity, <br/>parameters, file selection, and post-actions. |

The root README describes the authoring repository and is also used as the NuGet package README. 
The README inside `content` belongs to the generated application, 
keeping package documentation separate from application and deployment guidance.

## What the generated starter kit includes
* ASP.NET Core Minimal API hosted on AWS Lambda
* Amazon API Gateway and DynamoDB
* AWS CDK infrastructure written in C#
* Clean Architecture and Vertical Slice generation options
* FluentValidation, exception handling, health checks, and OpenAPI
* Unit and architecture tests

See the generated-solution documentation for architecture details, prerequisites, 
and AWS deployment guidance.

# Use the template locally
Clone the repository and install the template from its authoring directory:

```bash
git clone https://github.com/ghanavat/dotnet-aws-starter-kit.git
dotnet new install ./
```

Generate the default Clean Architecture solution:

```bash
dotnet new ghanavats_dotnet_aws_starter \
--name MyCompany.MyProduct \
--architecture clean-arc
```

The manifest also exposes the Vertical Slice option:

```bash
dotnet new ghanavats_dotnet_aws_starter \
--name MyCompany.MyProduct \
--architecture vertical-slice
```

List the template or remove the local installation:

```bash
dotnet new list ghanavats_dotnet_aws_starter
dotnet new uninstall ./
```

Build the NuGet template package

```bash
dotnet pack
```

The package is written to `bin/Release/` Before publishing it, 
install the generated `.nupkg` in a clean template environment 
and verify that both architecture choices generate, restore, build, and test successfully.

# Contributing
When changing the template, update files under `content/DotNetAws.StarterKit/` and test 
the generated output rather than only the authoring project. Whole architecture-specific folders should be controlled by the source modifiers in `template.json`; shared project files can use Template Engine conditionals for smaller differences.

Feedback, issues, and pull requests are welcome.
