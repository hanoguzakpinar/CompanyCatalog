using CompanyCatalog.Domain.Common;

namespace CompanyCatalog.Domain.Categories;

public sealed class Category : AggregateRoot<Guid>
{
    private Category()
    {
    }

    private Category(Guid id, string name, string? description)
    {
        Id = id;
        Name = name;
        Description = description;
        IsActive = true;
    }

    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public bool IsActive { get; private set; }

    public Category Create(string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Length < 2)
            throw new DomainException("Category ismi 2 karakterden kısa olamaz.");
        if (name.Length > 100)
            throw new DomainException("Category ismi maksimum 100 karakter olabilir.");
        if (description != null && description.Length > 500)
            throw new DomainException("Category açıklaması maksimum 500 karakter olabilir.");

        return new Category(Guid.CreateVersion7(), name.Trim(), description?.Trim());
    }

    public void Rename(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName) || newName.Length < 2)
            throw new DomainException("Category ismi 2 karakterden kısa olamaz.");
        if (newName.Length > 100)
            throw new DomainException("Category ismi maksimum 100 karakter olabilir.");

        Name = newName.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateDescription(string? newDescription)
    {
        if (newDescription != null && newDescription.Length > 500)
            throw new DomainException("Category açıklaması maksimum 500 karakter olabilir.");

        Description = newDescription?.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        if (IsActive)
            throw new DomainException("Category zaten aktif durumdadır.");

        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        if (!IsActive)
            throw new DomainException("Category zaten inaktif durumdadır.");

        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }
}