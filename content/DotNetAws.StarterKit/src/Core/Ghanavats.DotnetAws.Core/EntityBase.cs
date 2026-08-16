namespace Ghanavats.DotnetAws.Core;

public abstract class EntityBase
{
    public virtual Guid Id { get; protected init; }

    protected EntityBase()
    {
    }

    protected EntityBase(Guid id)
    {
        Id = id;
    }
}
