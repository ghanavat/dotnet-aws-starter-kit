using ArchUnitNET.Domain;
using ArchUnitNET.xUnitV3;
using Microsoft.Extensions.DependencyInjection;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace Ghanavats.DotnetAws.ArchitectureTests;

public class ArchitectureTests : ArchitectureTestsBase
{
    private readonly IObjectProvider<IType> DomainLayer = Types()
        .That()
        .ResideInAssembly(DomainAssembly)
        .As("Domain Layer");

    private readonly IObjectProvider<IType> PresentationLayer = Types()
        .That()
        .ResideInAssembly(PresentationAssembly)
        .And().DoNotDependOnAny(typeof(IServiceCollection))
        .As("Presentation Layer");

    private readonly IObjectProvider<IType> InfrastructureLayer = Types()
        .That()
        .ResideInAssembly(InfrastructureAssembly)
        .As("Infrastructure Layer");

    private readonly IObjectProvider<IType> ApplicationLayer = Types()
        .That()
        .ResideInAssembly(ApplicationAssembly)
        .As("Application Layer");

    private readonly IObjectProvider<IType> SharedProject = Types()
        .That()
        .ResideInAssembly(SharedAssembly)
        .As("Shared Project");

    private readonly IObjectProvider<IType> ApiEndpoints = Classes()
        .That()
        .ResideInAssembly(PresentationAssembly)
        .And().ArePublic()
        .And().AreSealed()
        .And().AreAbstract()
        .And().HaveNameEndingWith("Endpoint")
        .As("API Endpoints");

    private readonly IObjectProvider<Interface> RepositoryInterfaces = Interfaces()
        .That().ResideInAssembly(ApplicationAssembly)
        .And().ArePublic()
        .And().HaveNameEndingWith("Repository")
        .As("Infrastructure Repositories");

    [Fact]
    public void Types_In_Presentation_ShouldNotDirectlyDependOnRepositories()
    {
        Types().That().Are(PresentationLayer)
            .Should().NotDependOnAny(InfrastructureLayer)
            .Because("Presentation layer should not depend on Infrastructure layer")
            .Check(Architecture);
    }

    [Fact]
    public void Endpoints_ShouldNotDependOnRepositories()
    {
        Classes().That().Are(ApiEndpoints)
            .Should().NotDependOnAny(RepositoryInterfaces)
            .Because("API endpoints should not directly depend on Repository interfaces")
            .Check(Architecture);
    }

    [Fact]
    public void RepositoryInterfaces_ShouldBeImplementedInInfrastructureLayer()
    {
        Classes().That().Are(InfrastructureLayer)
            .And().ResideInNamespace("Ghanavats.DotnetAws.__INFRA_TO_FEATURE_NAMESPACE__.Repositories")
            .Should().ImplementAnyInterfaces(RepositoryInterfaces)
            .Because("Repository interfaces should be implemented in the Infrastructure layer")
            .Check(Architecture);
    }

    [Fact]
    public void RepositoryInterfaces_ShouldNotBeImplementedInAnyLayerButInfrastructure()
    {
        Classes().That().AreNot(InfrastructureLayer)
            .And().DoNotResideInNamespace("Ghanavats.DotnetAws.__INFRA_TO_FEATURE_NAMESPACE__.Repositories")
            .Should().NotImplementAnyInterfaces(RepositoryInterfaces)
            .Because("Repository interfaces should only be implemented in the Infrastructure layer")
            .Check(Architecture);
    }
}
