using CompanyCatalog.Application.Abstractions.Results;
using CompanyCatalog.Application.Companies.Shared;
using CompanyCatalog.Application.Products.Shared;
using MediatR;

namespace CompanyCatalog.Application.Products.Queries.GetProducts;

public sealed record GetProductsQuery(
    string? Search = null,
    Guid? CompanyId = null,
    Guid? CategoryId = null,
    decimal? MinPrice = null,
    decimal? MaxPrice = null,
    bool? IsActive = null,
    bool? InStock = null,
    int Page = 1,
    int PageSize = 20,
    string SortBy = "name",
    bool SortDescending = false
) : IRequest<Result<PagedResult<ProductResponse>>>;