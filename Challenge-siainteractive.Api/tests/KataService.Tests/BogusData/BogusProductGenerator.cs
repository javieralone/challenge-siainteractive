using Bogus;
using Challenge.Domain.Entities;
using Challenge.Domain.ValueObjects;

namespace Challenge.Tests.BogusData;

public static class BogusProductGenerator
{
    public static Product GetProduct()
    {
        var faker = new Faker();
        var name = faker.Commerce.ProductName();
        var description = faker.Commerce.ProductDescription();

        return Product.Create(name, description);
    }

    public static Product GetProductWithImage()
    {
        var faker = new Faker();
        var name = faker.Commerce.ProductName();
        var description = faker.Commerce.ProductDescription();
        var imageUrl = $"https://picsum.photos/seed/{faker.Random.Number(1000, 9999)}/200/200";

        var product = Product.Create(name, description);
        product.AssignImage(new Image(imageUrl));

        return product;
    }
}

