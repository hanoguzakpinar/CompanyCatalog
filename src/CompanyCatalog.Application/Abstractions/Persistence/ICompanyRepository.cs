using CompanyCatalog.Domain.Companies;

namespace CompanyCatalog.Application.Abstractions.Persistence;

public interface ICompanyRepository
{
    Task<Company?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Company?> GetByTaxNumberAsync(string taxNumber, CancellationToken ct = default);
    Task<bool> ExistsByTaxNumberAsync(string taxNumber, CancellationToken ct = default);

    void Add(Company company);
    void Update(Company company);
    void Remove(Company company);
}