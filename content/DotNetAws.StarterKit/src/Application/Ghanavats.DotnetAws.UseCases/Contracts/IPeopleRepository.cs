using Ghanavats.DotnetAws.Core.Entities;

namespace Ghanavats.DotnetAws.UseCases.Contracts;

public interface IPeopleRepository
{
    Task<Person> GetPersonById(Guid personId);
    Task CreatePerson(Person person);
}
