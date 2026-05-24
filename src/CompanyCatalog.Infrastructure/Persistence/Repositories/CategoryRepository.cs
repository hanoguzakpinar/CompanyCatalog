using CompanyCatalog.Application.Abstractions.Persistence;
using CompanyCatalog.Domain.Categories;
using Microsoft.EntityFrameworkCore;

namespace CompanyCatalog.Infrastructure.Persistence.Repositories;

internal sealed class CategoryRepository(
    ApplicationDbContext context) : ICategoryRepository
{
    public async Task<Category?> GetByIdAsync(
        Guid id, CancellationToken cancellationToken)
        => await context.Categories
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    public void Add(Category category)
        => context.Categories.Add(category);

    public void Update(Category category)
        => context.Categories.Update(category);

    public void Remove(Category category)
        => context.Categories.Remove(category);
}