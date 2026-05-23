using CompanyCatalog.Application.Abstractions.Results;
using CompanyCatalog.Application.Companies.Shared;
using MediatR;

namespace CompanyCatalog.Application.Companies.Queries.GetCompanies;

public sealed record GetCompaniesQuery(
    string? Search = null,
    bool? IsActive = null,
    int Page = 1,
    int PageSize = 20,
    string SortBy = "name",
    bool SortDescending = false
) : IRequest<Result<PagedResult<CompanyResponse>>>;