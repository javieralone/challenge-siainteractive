using Challenge.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Challenge.Infrastructure.Data.EntityConfigurations;

public class ProductCategoryConfiguration : IEntityTypeConfiguration<ProductCategory>
{
    public void Configure(EntityTypeBuilder<ProductCategory> builder)
    {
        builder.ToTable("ProductCategories");

        builder.HasKey(pc => pc.Id);

        builder.Property(pc => pc.Id)
            .HasColumnName("Id")
            .ValueGeneratedOnAdd();

        builder.Property(pc => pc.ProductId)
            .HasColumnName("ProductId")
            .IsRequired();

        builder.Property(pc => pc.CategoryId)
            .HasColumnName("CategoryId")
            .IsRequired();

        builder.HasIndex(pc => new { pc.ProductId, pc.CategoryId })
            .IsUnique();

        builder.HasOne(pc => pc.Product)
            .WithMany(p => p.ProductCategories)
            .HasForeignKey(pc => pc.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(pc => pc.Category)
            .WithMany(c => c.ProductCategories)
            .HasForeignKey(pc => pc.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

