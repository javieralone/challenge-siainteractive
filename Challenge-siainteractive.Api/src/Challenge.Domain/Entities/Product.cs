using Challenge.Domain.Abstractions;
using Challenge.Domain.ValueObjects;

namespace Challenge.Domain.Entities;

public class Product : EntityBase
{
    public string Name { get; internal set; }
    public string Description { get; internal set; }
    public Image? Image { get; internal set; }

    public ICollection<ProductCategory> ProductCategories { get; internal set; } = new List<ProductCategory>();

    private Product() { }

    public static Product Create(string name, string description)
    {
        return new Product
        {
            Name = name,
            Description = description
        };
    }

    public void AssignImage(Image image)
    {
        Image = image;
    }

    public void Update(string name, string description)
    {
        Name = name;
        Description = description;
    }
}