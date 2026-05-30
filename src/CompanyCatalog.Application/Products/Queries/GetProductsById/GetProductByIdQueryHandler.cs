using CompanyCatalog.Application.Abstractions.Persistence;
using CompanyCatalog.Application.Abstractions.Results;
using CompanyCatalog.Application.Products.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CompanyCatalog.Application.Products.Queries.GetProductsById;

internal sealed class GetProductByIdQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetProductByIdQuery, Result<ProductResponse>>
{
    public async Task<Result<ProductResponse>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await context.Products.AsNoTracking()
            .Where(p => p.Id == request.Id)
            .Select(p => new ProductResponse(
                p.Id,
                p.Name,
                p.Description,
                p.Sku,
                p.Price,
                p.StockQuantity,
                p.IsActive,
                p.CompanyId,
                p.Company!.Name,
                p.CategoryId,
                p.Category!.Name,
                p.CreatedAt,
                p.UpdatedAt
            )).FirstOrDefaultAsync(cancellationToken);

        if (product is null)
            return Result.Failure<ProductResponse>(Error.NotFound("Product.NotFound",
                $"{request.Id}'li ürün bulunamadı."));

        return Result.Success(product);
    }
}