using ArchUnitNET.Domain;
using ArchUnitNET.Loader;
using Ghanavats.DotnetAws.__ENTITIES_NAMESPACE__;
using Ghanavats.DotnetAws.Infrastructure.DependencyInjection;
using Ghanavats.DotnetAws.Shared;
using Ghanavats.DotnetAws.UseCases.DependencyInjection;
using Assembly = System.Reflection.Assembly;

namespace Ghanavats.DotnetAws.ArchitectureTests;

public class ArchitectureTestsBase
{
    private protected static readonly Assembly PresentationAssembly = typeof(Program).Assembly;

    private protected static readonly Assembly InfrastructureAssembly =
        typeof(InfrastructureExtension).Assembly;

    private protected static readonly Assembly DomainAssembly = typeof(Person).Assembly;

    private protected static readonly Assembly ApplicationAssembly =
        typeof(RegisterApplicationServices).Assembly;

    private protected static readonly Assembly SharedAssembly = typeof(AssemblyMarker).Assembly;

    private protected readonly Architecture Architecture = new ArchLoader()
        .LoadAssemblies(
            PresentationAssembly,
            InfrastructureAssembly,
            DomainAssembly,
            ApplicationAssembly,
            SharedAssembly
        )
        .Build();
}
