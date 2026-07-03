using Amazon.DynamoDBv2.DataModel;
using Ghanavats.CleanArchitecture.Core.Entities;

namespace Ghanavats.CleanArchitecture.Infrastructure.DynamoDbModels;

[DynamoDBTable("People")]
internal sealed class DynamoDbPerson
{
    [DynamoDBHashKey]
    public string PersonId { get; init; } = string.Empty;
    [DynamoDBProperty]
    public string Name { get; init; } = string.Empty;
    [DynamoDBProperty]
    public string Email { get; init; } = string.Empty;
    [DynamoDBProperty]
    public string Phone { get; init; } = string.Empty;
    [DynamoDBProperty]
    public string DateOfBirth { get; init; } = string.Empty;
}

internal static class PersonMapper
{
    extension(DynamoDbPerson? source)
    {
        public Person? ToDomain()
        {
            if (source is null)
            {
                // Maybe I should throw an exception here?
                return null;
            }
            
            return Person.Rehydrate(Guid.Parse(source.PersonId), 
                source.Name, source.Email, 
                source.Phone,  source.DateOfBirth);
        }
    }
}
