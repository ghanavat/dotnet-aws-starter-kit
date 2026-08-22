using Amazon.DynamoDBv2.DataModel;
using Ghanavats.DotnetAws.__INFRA_TO_API_NAMESPACE__.DynamoDbModels;
using Ghanavats.DotnetAws.__INFRA_TO_FEATURE_NAMESPACE__.Repositories;
using Moq;

namespace Ghanavats.DotnetAws.__INFRA_TO_API_NAMESPACE__.Tests.RepositoryTests;

public class PeopleRepositoryTests
{
    private readonly PeopleRepository _sut;
    private readonly Mock<IDynamoDBContext> _dynamoDbContextInterface;
    
    public PeopleRepositoryTests()
    {
        _dynamoDbContextInterface = new Mock<IDynamoDBContext>();
        _sut = new PeopleRepository(_dynamoDbContextInterface.Object);
    }

    [Fact]
    public async Task GetPersonById_ShouldReturnNull_WhenPersonDoesNotExist()
    {
        // Arrange
        var personId = Guid.NewGuid();
        _dynamoDbContextInterface.Setup(x => x.LoadAsync<DynamoDbPerson>(personId.ToString("D")))!
            .ReturnsAsync((DynamoDbPerson?)null);
        
        // Act
        var result = await _sut.GetPersonById(personId);
        
        // Assert
        Assert.Null(result);
    }
    
    [Fact]
    public async Task GetPersonById_ShouldReturnValidPerson_WhenPersonExists()
    {
        // Arrange
        var personId = Guid.NewGuid();
        var expectedPerson = new DynamoDbPerson { PersonId = personId.ToString() };
        
        _dynamoDbContextInterface.Setup(x => x.LoadAsync<DynamoDbPerson>(personId.ToString("D")))!
            .ReturnsAsync(expectedPerson);
        
        // Act
        var result = await _sut.GetPersonById(personId);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedPerson.PersonId, result?.Id.ToString());
    }
}
