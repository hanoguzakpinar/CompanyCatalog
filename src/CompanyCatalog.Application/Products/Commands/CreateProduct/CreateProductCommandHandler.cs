using CompanyCatalog.Application.Abstractions.Persistence;
using CompanyCatalog.Application.Abstractions.Results;
using CompanyCatalog.Domain.Common;
using CompanyCatalog.Domain.Products;
using MediatR;

namespace CompanyCatalog.Application.Products.Commands.CreateProduct;

internal sealed class CreateProductCommandHandler(
    IUnitOfWork unitOfWork,
    IProductRepository productRepository,
    ICompanyRepository companyRepository,
    ICategoryRepository categoryRepository
) : IRequestHandler<CreateProductCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var company = await companyRepository.GetByIdAsync(request.CompanyId, cancellationToken);
        if (company is null || !company.IsActive)
            return Result.Failure<Guid>(Error.NotFound("Company.NotFound",
                $"{request.CompanyId}'li company bulunamadı."));

        var category = await categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);
        if (category is null || !category.IsActive)
            return Result.Failure<Guid>(Error.NotFound("Category.NotFound",
                $"{request.CategoryId}'li kategori bulunamadı."));

        var skuExists = await productRepository.ExistsBySkuAsync(request.Sku, cancellationToken);
        if (skuExists)
            return Result.Failure<Guid>(Error.Conflict("Product.DuplicateSku",
                $"{request.Sku}'li ürün bulunmaktadır."));

        Product product;
        try
        {
            product = Product.Create(
                request.Name,
                request.Description,
                request.Sku,
                request.Price,
                request.StockQuantity,
                request.CompanyId,
                request.CategoryId
            );
        }
        catch (DomainException ex)
        {
            return Result.Failure<Guid>(Error.Conflict("Product.Invalid", ex.Message));
        }

        productRepository.Add(product);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(product.Id);
    }
}