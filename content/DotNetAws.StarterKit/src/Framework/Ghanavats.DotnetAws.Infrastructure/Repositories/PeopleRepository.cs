using Amazon.DynamoDBv2.DataModel;
using Ghanavats.DotnetAws.__ENTITIES_NAMESPACE__;
using Ghanavats.DotnetAws.__FEATURE_NAMESPACE__.Contracts;
using Ghanavats.DotnetAws.__INFRA_DYNAMO_NAMESPACE__.DynamoDbModels;
using Microsoft.Extensions.Logging;

namespace Ghanavats.DotnetAws.__INFRA_TO_FEATURE_NAMESPACE__.Repositories;

public /*__REPOSITORY_ACCESS__*/ sealed class PeopleRepository : IPeopleRepository
{
    private readonly DynamoDBContext _dbContext;
    private readonly ILogger<PeopleRepository> _logger;

    public PeopleRepository(ILogger<PeopleRepository> logger,
        DynamoDBContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<Person> GetPersonById(Guid personId)
    {
        var result = await _dbContext.LoadAsync<DynamoDbPerson>(personId.ToString("D"));
        return result.ToDomain();
    }

    public async Task CreatePerson(Person person)
    {
        var newItem = Person.Create("Test1", "test1@domcin.com", "123456",
            new DateTime(1990, 01, 01).ToString("yyyy-MM-dd"));
        var dynamoDbPerson = new DynamoDbPerson
        {
            PersonId = newItem.Id.ToString("D"),
            Name = newItem.Name,
            Email = newItem.Email,
            Phone = newItem.Phone,
            DateOfBirth = newItem.DateOfBirth
        };

        await _dbContext.SaveAsync(dynamoDbPerson);
    }
}
