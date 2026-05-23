using CompanyCatalog.Application.Abstractions.Persistence;
using CompanyCatalog.Domain.Companies;
using Microsoft.EntityFrameworkCore;

namespace CompanyCatalog.Infrastructure.Persistence.Repositories;

internal sealed class CompanyRepository(ApplicationDbContext context) : ICompanyRepository
{
    public async Task<Company?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await context.Companies.FirstOrDefaultAsync(c => c.Id == id, ct);
    }

    public async Task<Company?> GetByTaxNumberAsync(string taxNumber, CancellationToken ct = default)
    {
        return await context.Companies.FirstOrDefaultAsync(c => c.TaxNumber == taxNumber, ct);
    }

    public async Task<bool> ExistsByTaxNumberAsync(string taxNumber, CancellationToken ct = default)
    {
        return await context.Companies.AnyAsync(c => c.TaxNumber == taxNumber, ct);
    }

    public void Add(Company company)
    {
        context.Companies.Add(company);
    }

    public void Update(Company company)
    {
        context.Companies.Update(company);
    }

    public void Remove(Company company)
    {
        context.Companies.Remove(company);
    }
}