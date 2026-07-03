using System.Diagnostics;
using Amazon.DynamoDBv2.DataModel;
using Ghanavats.CleanArchitecture.Core.Entities;
using Ghanavats.CleanArchitecture.Infrastructure.DynamoDbModels;
using Ghanavats.CleanArchitecture.UseCases.Contracts;
using Microsoft.Extensions.Logging;

namespace Ghanavats.CleanArchitecture.Infrastructure.Repositories;

internal sealed class PeopleRepository : IPeopleRepository
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
        var stopwatch = Stopwatch.StartNew();
        var result = await _dbContext.LoadAsync<DynamoDbPerson>(personId.ToString("D"));
        stopwatch.Stop();

        if (_logger.IsEnabled(LogLevel.Information))
        {
            _logger.LogInformation("Successfully fetched Person Details and it took: {Time}",
                stopwatch.ElapsedMilliseconds);
        }

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
