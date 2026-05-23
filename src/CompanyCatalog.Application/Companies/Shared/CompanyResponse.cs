namespace CompanyCatalog.Application.Companies.Shared;

public sealed record CompanyResponse(
    Guid Id,
    string Name,
    string TaxNumber,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);