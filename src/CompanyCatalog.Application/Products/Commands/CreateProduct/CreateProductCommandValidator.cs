using FluentValidation;

namespace CompanyCatalog.Application.Products.Commands.CreateProduct;

internal sealed class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().MinimumLength(2).MaximumLength(200);

        When(x => x.Description != null, () =>
            RuleFor(x => x.Description).MaximumLength(1000));

        RuleFor(x => x.Sku)
            .NotEmpty().MinimumLength(3).MaximumLength(50);

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Fiyat, negatif olamaz.");

        RuleFor(x => x.StockQuantity)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Stok, negatif olamaz.");

        RuleFor(x => x.CompanyId).NotEmpty();
        RuleFor(x => x.CategoryId).NotEmpty();
    }
}