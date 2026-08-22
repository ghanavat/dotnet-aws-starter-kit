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
        var dateOfBirth = DateTime.Parse(person.DateOfBirth).ToString("yyyy-MM-dd");
        var newPerson = Person.Create(person.Name, person.Email, person.Phone, dateOfBirth);
        
        var dynamoDbPerson = new DynamoDbPerson
        {
            PersonId = newPerson.Id.ToString("D"),
            Name = newPerson.Name,
            Email = newPerson.Email,
            Phone = newPerson.Phone,
            DateOfBirth = newPerson.DateOfBirth
        };

        await _dbContext.SaveAsync(dynamoDbPerson);
    }
}
