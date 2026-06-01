using CompanyCatalog.Application.Abstractions.Persistence;
using CompanyCatalog.Application.Abstractions.Results;
using CompanyCatalog.Application.Products.Commands.DeleteProduct;
using MediatR;

namespace CompanyCatalog.Application.Products.Commands.ToggleProduct;

internal sealed class ToggleProductHandler(
    IProductRepository productRepository,
    IUnitOfWork unitOfWork
) : IRequestHandler<ToggleProductStatusCommand, Result>
{
    public async Task<Result> Handle(ToggleProductStatusCommand request, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(request.Id, cancellationToken);
        if (product is null)
            return Result.Failure(Error.NotFound("Product.NotFound", $"{request.Id}'li ürün bulunamadı."));

        if (product.IsActive)
            product.Deactivate();
        else
            product.Activate();

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}