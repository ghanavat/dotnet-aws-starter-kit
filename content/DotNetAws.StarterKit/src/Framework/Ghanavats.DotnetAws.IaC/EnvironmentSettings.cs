namespace Ghanavats.DotnetAws.IaC;

public class EnvironmentSettings
{
    public required string Name { get; init; }
    public required bool IsProduction { get; init; }
    public bool DynamoDbDeletionProtection => IsProduction;
    public bool DynamoDbPointInTimeRecovery => IsProduction;
    public bool StackTerminationProtection => IsProduction;
    public int LambdaMemorySize { get; init; }
    public string LogLevel { get; init; } = "Information";
    public bool EnableDetailedMonitoring { get; init; }
    public bool EnableWaf { get; init; }
    public bool EnableXRay { get; init; }
}
