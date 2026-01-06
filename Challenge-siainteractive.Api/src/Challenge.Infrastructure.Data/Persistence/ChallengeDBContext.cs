using Challenge.Domain.Entities;
using Challenge.Infrastructure.Data.EntityConfigurations;
using Microsoft.EntityFrameworkCore;

namespace Challenge.Infrastructure.Data.Persistence;

public class ChallengeDBContext : DbContext
{
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<ProductCategory> ProductCategories { get; set; }

    public ChallengeDBContext(DbContextOptions<ChallengeDBContext> contextOptions) : base(contextOptions)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new ProductConfiguration());
        modelBuilder.ApplyConfiguration(new CategoryConfiguration());
        modelBuilder.ApplyConfiguration(new ProductCategoryConfiguration());
    }
}
