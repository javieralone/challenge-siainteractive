namespace Challenge.Domain.Abstractions;

public interface IEntityBase
{
    long Id { get; internal set; }
}