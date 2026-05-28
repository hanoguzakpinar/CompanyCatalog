using CompanyCatalog.Application.Abstractions.Persistence;
using CompanyCatalog.Application.Abstractions.Results;
using CompanyCatalog.Domain.Common;
using MediatR;

namespace CompanyCatalog.Application.Products.Commands.UpdateProductDetails;

internal sealed class UpdateProductDetailsHandler(
    IProductRepository productRepository,
    IUnitOfWork unitOfWork
) : IRequestHandler<UpdateProductDetailsCommand, Result>
{
    public async Task<Result> Handle(UpdateProductDetailsCommand request, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(request.Id, cancellationToken);
        if (product is null)
            return Result.Failure(Error.NotFound("Product.NotFound", $"{request.Id}'li ürün bulunamadı."));

        try
        {
            product.UpdateDetails(request.Name, request.Description, request.Price);
        }
        catch (DomainException ex)
        {
            return Result.Failure(Error.Validation("Product.Invalid", ex.Message));
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}