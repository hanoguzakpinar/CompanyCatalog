using CompanyCatalog.Application.Abstractions.Persistence;
using CompanyCatalog.Application.Abstractions.Results;
using CompanyCatalog.Domain.Common;
using MediatR;

namespace CompanyCatalog.Application.Products.Commands.UpdateProductSku;

internal sealed class UpdateProductSkuHandler(
    IProductRepository productRepository,
    IUnitOfWork unitOfWork
) : IRequestHandler<UpdateProductSkuCommand, Result>
{
    public async Task<Result> Handle(UpdateProductSkuCommand request, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(request.Id, cancellationToken);
        if (product is null)
            return Result.Failure(Error.NotFound("Product.NotFound", $"{request.Id}'li ürün bulunamadı."));

        var skuTaken =
            await productRepository.ExistsBySkuExcludingAsync(request.Sku.ToUpperInvariant(), request.Id,
                cancellationToken);
        if (skuTaken)
            return Result.Failure(Error.Conflict(
                "Product.DuplicateSku", $"SKU {request.Sku} zaten mevcut."));

        try
        {
            product.UpdateSku(request.Sku);
        }
        catch (DomainException ex)
        {
            return Result.Failure(Error.Validation("Product.Invalid", ex.Message));
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}