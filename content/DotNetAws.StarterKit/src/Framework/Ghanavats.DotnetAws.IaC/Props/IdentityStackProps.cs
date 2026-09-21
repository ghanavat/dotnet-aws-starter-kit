using Amazon.CDK;

namespace Ghanavats.DotnetAws.IaC.Props;

public class IdentityStackProps : IStackProps
{
    public string ApiId { get; init; } = string.Empty;
    public string Stage { get; init; } = string.Empty;
    public string? AccountId { get; init; } = string.Empty;
    public string? Region { get; init; } = string.Empty;
}
