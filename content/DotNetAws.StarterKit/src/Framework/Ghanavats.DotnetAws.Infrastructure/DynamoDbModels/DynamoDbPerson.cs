using System.Runtime.CompilerServices;
using Amazon.DynamoDBv2.DataModel;
using Ghanavats.DotnetAws.__ENTITIES_NAMESPACE__;

[assembly: InternalsVisibleTo("Ghanavats.DotnetAws.Infrastructure.Tests")]

namespace Ghanavats.DotnetAws.__INFRA_TO_API_NAMESPACE__.DynamoDbModels;

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
                return null;
            }
            
            return Person.Rehydrate(Guid.Parse(source.PersonId), 
                source.Name, source.Email, 
                source.Phone,  source.DateOfBirth);
        }
    }
}
