using FluentValidation;

namespace CompanyCatalog.Application.Companies.Commands.UpdateCompany;

internal sealed class UpdateCompanyCommandValidator : AbstractValidator<UpdateCompanyCommand>
{
    public UpdateCompanyCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
        
        RuleFor(x => x.Name)
            .NotEmpty()
            .MinimumLength(2)
            .MaximumLength(200);

        RuleFor(x => x.TaxNumber)
            .NotEmpty()
            .Length(10)
            .Matches("^[0-9]+$");
    }
}