namespace Ghanavats.DotnetAws.Core.Entities;

/// <summary>
/// A greatly simplified sample Person entity.
/// Replace with your own or remove if not needed.
/// </summary>
public sealed class Person : EntityBase
{
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Phone { get; init; } = string.Empty;
    public string DateOfBirth { get; init; } = string.Empty;

    public Person()
    {
    }

    private Person(Guid id,
        string name,
        string email,
        string phone,
        string dateOfBirth)
    {
        Id = id;
        Name = name;
        Email = email;
        Phone = phone;
        DateOfBirth = dateOfBirth;
    }

    public static Person Create(string name, string email, string phone, string dateOfBirth)
    {
        return new Person(Guid.NewGuid(), name, email, phone, dateOfBirth);
    }

    public static Person Rehydrate(Guid id, string name, string email, string phone, string dateOfBirth)
    {
        return new Person(id, name, email, phone, dateOfBirth);
    }
}
