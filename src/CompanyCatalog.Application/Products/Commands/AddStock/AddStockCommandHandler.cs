using CompanyCatalog.Application.Abstractions.Persistence;
using CompanyCatalog.Application.Abstractions.Results;
using CompanyCatalog.Domain.Common;
using MediatR;

namespace CompanyCatalog.Application.Products.Commands.AddStock;

internal sealed class AddStockCommandHandler(
    IUnitOfWork unitOfWork,
    IProductRepository productRepository
) : IRequestHandler<AddStockCommand, Result>
{
    public async Task<Result> Handle(AddStockCommand request, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(request.ProductId, cancellationToken);
        if (product is null)
            return Result.Failure(Error.NotFound("Product.NotFound", $"{request.ProductId}'li ürün bulunamadı."));

        try
        {
            product.AddStock(request.Quantity);
        }
        catch (DomainException ex)
        {
            return Result.Failure(Error.Validation("Product.InvalidStock", ex.Message));
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}