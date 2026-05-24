using CompanyCatalog.Application.Abstractions.Results;
using CompanyCatalog.Application.Categories.Shared;
using CompanyCatalog.Application.Companies.Shared;
using MediatR;

namespace CompanyCatalog.Application.Categories.Queries.GetCategories;

public sealed record GetCategoriesQuery(
    string? Search = null,
    bool? IsActive = null,
    int Page = 1,
    int PageSize = 20,
    string SortBy = "name",
    bool SortDescending = false)
    : IRequest<Result<PagedResult<CategoryResponse>>>;