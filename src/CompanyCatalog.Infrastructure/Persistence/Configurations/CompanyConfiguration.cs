using CompanyCatalog.Domain.Companies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompanyCatalog.Infrastructure.Persistence.Configurations;

internal sealed class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.ToTable("companies");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(200)
            .HasColumnName("name");

        builder.Property(c => c.TaxNumber)
            .IsRequired()
            .HasMaxLength(10)
            .IsFixedLength()
            .HasColumnName("tax_number");

        builder.Property(c => c.IsActive)
            .HasColumnName("is_active");
        builder.Property(c => c.CreatedAt)
            .HasColumnName("created_at");
        builder.Property(c => c.UpdatedAt)
            .HasColumnName("updated_at");

        // TaxNumber unique constraint
        builder.HasIndex(c => c.TaxNumber).IsUnique();

        // Search ve Filtreleme performansi için indexler
        builder.HasIndex(c => c.Name);
        builder.HasIndex(c => c.IsActive);

        builder.Ignore(c => c.DomainEvents);
    }
}