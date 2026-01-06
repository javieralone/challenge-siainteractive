using Challenge.Domain.Repositories;
using Challenge.Infrastructure.Data.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Challenge.Infrastructure.Data;

public static class ConfigureServices
{
    public static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
    }
}
