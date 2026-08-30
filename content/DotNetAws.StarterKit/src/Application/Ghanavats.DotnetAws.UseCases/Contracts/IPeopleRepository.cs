using Ghanavats.DotnetAws.__ENTITIES_NAMESPACE__;

namespace Ghanavats.DotnetAws.__USECASES_CONTRACT_TO_API_FEATURE_NAMESPACE__;

public interface IPeopleRepository
{
    Task<Person?> GetPersonById(Guid personId);
    Task CreatePerson(Person person);
}
