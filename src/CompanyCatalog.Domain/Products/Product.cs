using CompanyCatalog.Domain.Categories;
using CompanyCatalog.Domain.Common;
using CompanyCatalog.Domain.Companies;

namespace CompanyCatalog.Domain.Products;

public sealed class Product : AggregateRoot<Guid>
{
    private Product()
    {
    }

    private Product(
        Guid id, string name, string? description, string sku, decimal price, int stockQuantity,
        Guid companyId,
        Guid categoryId)
    {
        Id = id;
        Name = name;
        Description = description;
        Sku = sku;
        Price = price;
        StockQuantity = stockQuantity;
        CompanyId = companyId;
        CategoryId = categoryId;
        IsActive = true;
    }


    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string Sku { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public int StockQuantity { get; private set; }
    public bool IsActive { get; set; }

    //Foreign Keys
    public Guid CompanyId { get; private set; }
    public Guid CategoryId { get; private set; }

    //Navigation Props
    public Company? Company { get; set; }
    public Category? Category { get; set; }

    public static Product Create(string name, string? description, string sku, decimal price, int stockQuantity,
        Guid companyId, Guid categoryId)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Length < 2)
            throw new DomainException("Ürün ismi en az 2 karakter olmalıdır.");
        if (name.Length > 200)
            throw new DomainException("Ürün ismi maksimum 200 karakter olmalıdır.");
        if (description != null && description.Length > 1000)
            throw new DomainException("Açıklama maksimum 1000 karakter olmalıdır.");
        if (string.IsNullOrWhiteSpace(sku) || sku.Length < 3)
            throw new DomainException("SKU en az 3 karakter olmalıdır.");
        if (sku.Length > 50)
            throw new DomainException("SKU maksimum 50 karakter olmalıdır.");
        if (price < 0)
            throw new DomainException("Fiyat, negatif olamaz.");
        if (stockQuantity < 0)
            throw new DomainException("Stok adeti, negatif olamaz.");
        if (companyId == Guid.Empty)
            throw new DomainException("Company belirtilmelidir.");
        if (categoryId == Guid.Empty)
            throw new DomainException("Kategori belirtilmelidir.");

        return new Product(
            Guid.CreateVersion7(),
            name.Trim(),
            description?.Trim(),
            sku.Trim().ToUpperInvariant(),
            price,
            stockQuantity,
            companyId,
            categoryId
        );
    }

    public void UpdateDetails(string name, string? description, decimal price)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Length < 2)
            throw new DomainException("Ürün ismi en az 2 karakter olmalıdır.");
        if (name.Length > 200)
            throw new DomainException("Ürün ismi maksimum 200 karakter olmalıdır.");
        if (description != null && description.Length > 1000)
            throw new DomainException("Açıklama maksimum 1000 karakter olmalıdır.");
        if (price < 0)
            throw new DomainException("Fiyat, negatif olamaz.");

        Name = name.Trim();
        Description = description?.Trim();
        Price = price;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateSku(string newSku)
    {
        if (string.IsNullOrWhiteSpace(newSku) || newSku.Length < 3)
            throw new DomainException("SKU en az 3 karakter olmalıdır.");
        if (newSku.Length > 50)
            throw new DomainException("SKU maksimum 50 karakter olmalıdır.");

        Sku = newSku.Trim().ToUpperInvariant();
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddStock(int quantity)
    {
        if (quantity <= 0)
            throw new DomainException("Adet, sıfırdan büyük olmalıdır.");

        StockQuantity += quantity;
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveStock(int quantity)
    {
        if (quantity <= 0)
            throw new DomainException("Adet, sıfırdan büyük olmalıdır.");

        if (quantity > StockQuantity)
            throw new DomainException(
                $"Stok yetersiz. Mevcut Stok: {StockQuantity} , Düşülmek İstenen Stok: {quantity}");

        StockQuantity -= quantity;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        if (!IsActive)
            throw new DomainException("Ürün zaten inaktif durumdadır.");

        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        if (IsActive)
            throw new DomainException("Ürün zaten aktif durumdadır.");

        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }
}