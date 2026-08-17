using CompanyCatalog.Application.Abstractions.Persistence;
using CompanyCatalog.Application.Abstractions.Results;
using CompanyCatalog.Domain.Common;
using MediatR;

namespace CompanyCatalog.Application.Products.Commands.RemoveStock;

internal sealed class RemoveStockCommandHandler(
    IUnitOfWork unitOfWork,
    IProductRepository productRepository
) : IRequestHandler<RemoveStockCommand, Result>
{
    public async Task<Result> Handle(RemoveStockCommand request, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(request.ProductId, cancellationToken);
        if (product is null)
            return Result.Failure(Error.NotFound("Product.NotFound", $"{request.ProductId}'li ürün bulunamadı."));

        try
        {
            product.RemoveStock(request.Quantity);
        }
        catch (DomainException ex)
        {
            return Result.Failure(Error.Validation("Product.InsufficientStock", ex.Message));
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}