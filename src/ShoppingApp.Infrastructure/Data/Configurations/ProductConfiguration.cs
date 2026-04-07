using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShoppingApp.Domain.Entities;

namespace ShoppingApp.Infrastructure.Data.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Name).HasMaxLength(200).IsRequired();
        builder.Property(p => p.SKU).HasMaxLength(50).IsRequired();
        builder.HasIndex(p => p.SKU).IsUnique();
        builder.Property(p => p.Price).HasColumnType("decimal(18,2)");
        builder.Property(p => p.DiscountPrice).HasColumnType("decimal(18,2)");
        builder.HasOne(p => p.Category).WithMany(c => c.Products).HasForeignKey(p => p.CategoryId);
        builder.HasMany(p => p.Images).WithOne(i => i.Product).HasForeignKey(i => i.ProductId);
    }
}
