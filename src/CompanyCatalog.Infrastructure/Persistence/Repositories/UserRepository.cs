using CompanyCatalog.Application.Abstractions.Persistence;
using CompanyCatalog.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace CompanyCatalog.Infrastructure.Persistence.Repositories;

internal sealed class UserRepository(ApplicationDbContext context) : IUserRepository
{
    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await context.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        => await context.Users.FirstOrDefaultAsync(u => u.Email == email.ToLowerInvariant(), cancellationToken);

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
        => await context.Users.AnyAsync(u => u.Email == email.ToLowerInvariant(), cancellationToken);

    public void Add(User user)
        => context.Users.Add(user);
}