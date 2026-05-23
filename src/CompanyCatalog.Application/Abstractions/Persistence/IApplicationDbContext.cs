using CompanyCatalog.Domain.Companies;
using CompanyCatalog.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace CompanyCatalog.Application.Abstractions.Persistence;

public interface IApplicationDbContext
{
    DbSet<Company> Companies { get; }
    DbSet<User> Users { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}