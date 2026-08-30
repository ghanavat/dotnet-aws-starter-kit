using Amazon.DynamoDBv2.DataModel;
using Ghanavats.DotnetAws.__ENTITIES_NAMESPACE__;
using Ghanavats.DotnetAws.__INFRA_TO_API_NAMESPACE__.DynamoDbModels;
using Ghanavats.DotnetAws.__USECASES_CONTRACT_TO_API_FEATURE_NAMESPACE__;

namespace Ghanavats.DotnetAws.__INFRA_TO_FEATURE_NAMESPACE__.Repositories;

public /*__PUBLIC_TO_INTERNAL_ACCESS__*/ sealed class PeopleRepository : IPeopleRepository
{
    private readonly IDynamoDBContext _dbContext;

    public PeopleRepository(IDynamoDBContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Person?> GetPersonById(Guid personId)
    {
        var result = await _dbContext.LoadAsync<DynamoDbPerson>(personId.ToString("D"));
        return result.ToDomain();
    }

    public async Task CreatePerson(Person person)
    {
        var dateOfBirth = DateTime.Parse(person.DateOfBirth).ToString("dd-MM-yyyy");
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
