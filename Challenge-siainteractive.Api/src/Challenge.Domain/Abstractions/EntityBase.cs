namespace Challenge.Domain.Abstractions;

public abstract class EntityBase : IEntityBase
{
    public long Id { get; set; }
}