using CompanyCatalog.Domain.Common;

namespace CompanyCatalog.Domain.Companies;

public sealed class Company : AggregateRoot<Guid>
{
    private Company()
    {
    }

    private Company(Guid id,
        string name,
        string taxNumber)
    {
        Id = id;
        Name = name;
        TaxNumber = taxNumber;
        IsActive = true;
    }

    public string Name { get; private set; } = string.Empty;
    public string TaxNumber { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }

    public static Company Create(
        string name,
        string taxNumber
    )
    {
        if (string.IsNullOrWhiteSpace(name) || name.Length < 2)
            throw new DomainException("Company name minimum 2 karakter olmalıdır.");

        if (name.Length > 200)
            throw new DomainException("Company name maksimum 200 karakter olabilir.");

        if (string.IsNullOrWhiteSpace(taxNumber))
            throw new DomainException("Tax number zorunludur.");

        if (taxNumber.Length != 10)
            throw new DomainException("Tax number 10 karakter uzunluğunda olmalıdır.");

        if (!taxNumber.All(char.IsDigit))
            throw new DomainException("Tax number sadece sayı içerebilir.");

        return new Company(Guid.CreateVersion7(), name.Trim(), taxNumber);
    }

    public void Rename(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName) || newName.Length < 2)
            throw new DomainException("Company name minimum 2 karakter olmalıdır.");

        if (newName.Length > 200)
            throw new DomainException("Company name maksimum 200 karakter olabilir.");

        Name = newName.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateTaxNumber(string newTaxNumber)
    {
        if (string.IsNullOrWhiteSpace(newTaxNumber)
            || newTaxNumber.Length != 10
            || !newTaxNumber.All(char.IsDigit))
            throw new DomainException("Tax number 10 karakter uzunluğunda olmalıdır.");

        TaxNumber = newTaxNumber;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        if (IsActive)
            throw new DomainException("Company zaten aktif durumdadır.");

        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        if (!IsActive)
            throw new DomainException("Company zaten inaktif durumdadır.");

        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }
}