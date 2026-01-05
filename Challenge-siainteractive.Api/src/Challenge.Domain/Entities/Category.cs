using Challenge.Domain.Abstractions;

namespace Challenge.Domain.Entities;

public class Category : EntityBase
{
    public string Name { get; internal set; }

    public ICollection<ProductCategory> ProductCategories { get; internal set; } = new List<ProductCategory>();

    private Category() { }

    public static Category Create(string name)
    {
        return new Category
        {
            Name = name
        };
    }

    public void Update(string name)
    {
        Name = name;
    }
}

