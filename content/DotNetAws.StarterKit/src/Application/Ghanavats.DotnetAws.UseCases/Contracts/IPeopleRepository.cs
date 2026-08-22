using Ghanavats.DotnetAws.__ENTITIES_NAMESPACE__;

namespace Ghanavats.DotnetAws.__FEATURE_NAMESPACE__.Contracts;

public interface IPeopleRepository
{
    Task<Person?> GetPersonById(Guid personId);
    Task CreatePerson(Person person);
}
