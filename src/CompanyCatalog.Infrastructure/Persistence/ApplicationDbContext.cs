using CompanyCatalog.Application.Abstractions.Persistence;
using CompanyCatalog.Domain.Companies;
using CompanyCatalog.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace CompanyCatalog.Infrastructure.Persistence;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options), IUnitOfWork, IApplicationDbContext
{
    public DbSet<User> Users => Set<User>();

    public DbSet<Company> Companies => Set<Company>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Tum IEntityTypeConfiguration'lari otomatik bul ve uygula
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}