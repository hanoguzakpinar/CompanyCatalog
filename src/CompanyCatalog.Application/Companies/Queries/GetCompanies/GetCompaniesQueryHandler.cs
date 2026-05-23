using CompanyCatalog.Application.Abstractions.Persistence;
using CompanyCatalog.Application.Abstractions.Results;
using CompanyCatalog.Application.Companies.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CompanyCatalog.Application.Companies.Queries.GetCompanies;

internal sealed class GetCompaniesQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetCompaniesQuery, Result<PagedResult<CompanyResponse>>>
{
    public async Task<Result<PagedResult<CompanyResponse>>> Handle(GetCompaniesQuery request,
        CancellationToken cancellationToken)
    {
        var query = context.Companies.AsNoTracking();

        if (request.IsActive.HasValue)
            query = query.Where(c => c.IsActive == request.IsActive.Value);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim().ToLower();
            query = query.Where(c =>
                c.Name.ToLower().Contains(search) ||
                c.TaxNumber.Contains(search)
            );
        }

        query = (request.SortBy.ToLower(), request.SortDescending) switch
        {
            ("name", false) => query.OrderBy(c => c.Name),
            ("name", true) => query.OrderByDescending(c => c.Name),
            ("taxnumber", false) => query.OrderBy(c => c.TaxNumber),
            ("taxnumber", true) => query.OrderByDescending(c => c.TaxNumber),
            ("createdat", false) => query.OrderBy(c => c.CreatedAt),
            ("createdat", true) => query.OrderByDescending(c => c.CreatedAt),
            _ => query.OrderBy(c => c.Name)
        };

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip(request.Page - 1 * request.PageSize)
            .Take(request.PageSize)
            .Select(c => new CompanyResponse(
                c.Id,
                c.Name,
                c.TaxNumber,
                c.IsActive,
                c.CreatedAt,
                c.UpdatedAt
            ))
            .ToListAsync(cancellationToken);

        var result = new PagedResult<CompanyResponse>(
            items, totalCount, request.Page, request.PageSize
        );

        return Result.Success(result);
    }
}