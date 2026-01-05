using Challenge.Domain.Entities;
using Challenge.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KataService.Infrastructure.Data.EntityConfigurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasColumnName("Id")
            .ValueGeneratedOnAdd();

        builder.Property(p => p.Name)
            .HasColumnName("Name")
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Description)
            .HasColumnName("Description")
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(p => p.Image)
            .HasColumnName("Image")
            .HasConversion(
                v => v != null ? v.Value : null,
                v => v != null ? new Image(v) : null)
            .IsRequired(false)
            .HasMaxLength(500);

        builder.HasMany(p => p.ProductCategories)
            .WithOne(pc => pc.Product)
            .HasForeignKey(pc => pc.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

