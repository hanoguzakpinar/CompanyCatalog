using CompanyCatalog.Application.Abstractions.Persistence;
using CompanyCatalog.Domain.Products;
using Microsoft.EntityFrameworkCore;

namespace CompanyCatalog.Infrastructure.Persistence.Repositories;

internal sealed class ProductRepository(ApplicationDbContext context) : IProductRepository
{
    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await context.Products.FirstOrDefaultAsync(p => p.Id == id, ct);
    }

    public async Task<bool> ExistsBySkuAsync(string sku, CancellationToken ct = default)
    {
        return await context.Products.AnyAsync(p => p.Sku == sku.ToUpperInvariant(), ct);
    }

    public async Task<bool> ExistsBySkuExcludingAsync(string sku, Guid excludeId, CancellationToken ct = default)
    {
        return await context.Products.AnyAsync(p => p.Sku == sku.ToUpperInvariant() && p.Id != excludeId, ct);
    }

    public void Add(Product product)
    {
        context.Products.Add(product);
    }

    public void Update(Product product)
    {
        context.Products.Update(product);
    }

    public void Remove(Product product)
    {
        context.Products.Remove(product);
    }
}