namespace Challenge.Domain.ValueObjects;

public class Image : ValueObject
{
    public string Value { get; }

    public Image(string value)
    {
        Value = value;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public static implicit operator string(Image value)
    {
        return value.Value;
    }
}
