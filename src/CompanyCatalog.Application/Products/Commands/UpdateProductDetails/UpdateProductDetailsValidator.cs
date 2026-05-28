using FluentValidation;

namespace CompanyCatalog.Application.Products.Commands.UpdateProductDetails;

internal sealed class UpdateProductDetailsValidator : AbstractValidator<UpdateProductDetailsCommand>
{
    public UpdateProductDetailsValidator()
    {
        RuleFor(x => x.Id).NotEmpty();

        RuleFor(x => x.Name).NotEmpty().MinimumLength(2).MaximumLength(200);

        When(x => x.Description != null, () =>
            RuleFor(x => x.Description).MaximumLength(1000));

        RuleFor(x => x.Price).GreaterThanOrEqualTo(0);
    }
}