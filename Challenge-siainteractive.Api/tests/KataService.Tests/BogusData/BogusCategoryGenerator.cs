using Bogus;
using Challenge.Domain.Entities;

namespace Challenge.Tests.BogusData;

public static class BogusCategoryGenerator
{
    public static Category GetCategory()
    {
        var faker = new Faker();
        var name = faker.Name.FirstName();

        return Category.Create(name);
    }
}
