using CompanyCatalog.Domain.Categories;
using CompanyCatalog.Domain.Companies;
using CompanyCatalog.Domain.Products;
using CompanyCatalog.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace CompanyCatalog.Application.Abstractions.Persistence;

public interface IApplicationDbContext
{
    DbSet<Company> Companies { get; }
    DbSet<User> Users { get; }
    DbSet<Category> Categories { get; }
    DbSet<Product> Products { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}