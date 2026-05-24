using CompanyCatalog.Domain.Categories;

namespace CompanyCatalog.Application.Abstractions.Persistence;

public interface ICategoryRepository
{
    Task<Category?> GetByIdAsync(
        Guid id, CancellationToken cancellationToken);

    void Add(Category category);
    void Update(Category category);
    void Remove(Category category);
}