using CompanyCatalog.Domain.Products;

namespace CompanyCatalog.Application.Abstractions.Persistence;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<bool> ExistsBySkuAsync(string sku, CancellationToken ct = default);
    Task<bool> ExistsBySkuExcludingAsync(string sku, Guid excludeId, CancellationToken ct = default);

    void Add(Product product);
    void Update(Product product);
    void Remove(Product product);
}