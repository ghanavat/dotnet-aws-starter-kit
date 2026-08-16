using Ghanavats.DotnetAws.__ENTITIES_NAMESPACE__;

namespace Ghanavats.DotnetAws.__FEATURE_NAMESPACE__.GetPersonDetails.Responses;

public sealed class GetPersonByIdResponse
{
    public Guid PersonId { get; set; } = Guid.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string DateOfBirth { get; set; } = string.Empty;
}

public static class ResponseMapper
{
    extension(Person source)
    {
        public GetPersonByIdResponse ToResponse()
        {
            return new GetPersonByIdResponse
            {
                PersonId = source.Id,
                Name = source.Name,
                Email = source.Email,
                Phone = source.Phone,
                DateOfBirth = source.DateOfBirth
            };
        }
    }
}
