using FluentValidation;

namespace CompanyCatalog.Application.Products.Commands.UpdateProductSku;

internal sealed class UpdateProductSkuValidator : AbstractValidator<UpdateProductSkuCommand>
{
    public UpdateProductSkuValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        
        RuleFor(x => x.Sku).NotEmpty().MinimumLength(3).MaximumLength(50);
    }
}