using FluentValidation;

namespace CompanyCatalog.Application.Companies.Commands.CreateCompany;

internal sealed class CreateCompanyCommandValidator : AbstractValidator<CreateCompanyCommand>
{
    public CreateCompanyCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MinimumLength(2)
            .MaximumLength(200);

        RuleFor(x => x.TaxNumber)
            .NotEmpty()
            .Length(10)
            .Matches("^[0-9]+$")
            .WithMessage("Tax number sadece sayı içerebilir.");
    }
}