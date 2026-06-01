using CompanyCatalog.Domain.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompanyCatalog.Infrastructure.Persistence.Configurations;

internal sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("products");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200)
            .HasColumnName("name");

        builder.Property(p => p.Description)
            .HasMaxLength(1000)
            .HasColumnName("description");

        builder.Property(p => p.Sku)
            .IsRequired()
            .HasMaxLength(50)
            .HasColumnName("sku");

        builder.Property(p => p.Price)
            .IsRequired()
            .HasPrecision(18, 2)
            .HasColumnName("price");

        builder.Property(p => p.StockQuantity)
            .IsRequired()
            .HasColumnName("stock_quantity");

        builder.Property(p => p.IsActive)
            .HasColumnName("is_active");

        builder.Property(p => p.CreatedAt)
            .HasColumnName("created_at");

        builder.Property(p => p.UpdatedAt)
            .HasColumnName("updated_at");

        builder.Property(p => p.CompanyId)
            .HasColumnName("company_id");

        builder.Property(p => p.CategoryId)
            .HasColumnName("category_id");

        builder.HasOne(p => p.Company)
            .WithMany()
            .HasForeignKey(f => f.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Category)
            .WithMany()
            .HasForeignKey(f => f.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(p => p.Sku).IsUnique();
        builder.HasIndex(p => p.CompanyId);
        builder.HasIndex(p => p.CategoryId);
        builder.HasIndex(p => p.IsActive);
        builder.HasIndex(p => p.Price);

        builder.Ignore(p => p.DomainEvents);
    }
}