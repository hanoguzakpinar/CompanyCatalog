using CompanyCatalog.Application.Abstractions.Persistence;
using CompanyCatalog.Application.Abstractions.Results;
using CompanyCatalog.Application.Categories.Shared;
using CompanyCatalog.Application.Companies.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CompanyCatalog.Application.Categories.Queries.GetCategories;

internal sealed class GetCategoriesQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetCategoriesQuery, Result<PagedResult<CategoryResponse>>>
{
    public async Task<Result<PagedResult<CategoryResponse>>> Handle(GetCategoriesQuery request,
        CancellationToken cancellationToken)
    {
        var query = context.Categories.AsNoTracking();

        if (request.IsActive.HasValue)
            query = query.Where(c => c.IsActive == request.IsActive.Value);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim().ToLower();
            query = query.Where(c =>
                c.Name.ToLower().Contains(search) ||
                (c.Description != null && c.Description.ToLower().Contains(search)));
        }

        query = (request.SortBy.ToLower(), request.SortDescending) switch
        {
            ("name", false) => query.OrderBy(c => c.Name),
            ("name", true) => query.OrderByDescending(c => c.Name),
            ("createdat", false) => query.OrderBy(c => c.CreatedAt),
            ("createdat", true) => query.OrderByDescending(c => c.CreatedAt),
            _ => query.OrderBy(c => c.Name)
        };

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(c => new CategoryResponse(
                c.Id,
                c.Name,
                c.Description,
                c.IsActive,
                c.CreatedAt,
                c.UpdatedAt
            ))
            .ToListAsync(cancellationToken);

        var result = new PagedResult<CategoryResponse>(items, totalCount, request.Page, request.PageSize);

        return Result.Success(result);
    }
}