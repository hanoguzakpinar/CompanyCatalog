using CompanyCatalog.Application.Abstractions.Persistence;
using CompanyCatalog.Application.Abstractions.Results;
using CompanyCatalog.Domain.Common;
using CompanyCatalog.Domain.Companies;
using MediatR;

namespace CompanyCatalog.Application.Companies.Commands.CreateCompany;

internal sealed class CreateCompanyCommandHandler(ICompanyRepository companyRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<CreateCompanyCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateCompanyCommand request, CancellationToken cancellationToken)
    {
        var taxNumberExists = await companyRepository.ExistsByTaxNumberAsync(request.TaxNumber, cancellationToken);
        if (taxNumberExists)
        {
            return Result.Failure<Guid>(Error.Conflict("Company.DuplicateTaxNumber",
                "Bu tax number sistemizde zaten kayıtlıdır."));
        }

        Company company;
        try
        {
            company = Company.Create(request.Name, request.TaxNumber);
        }
        catch (DomainException ex)
        {
            return Result.Failure<Guid>(Error.Validation("Company.Invalid", ex.Message));
        }

        companyRepository.Add(company);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(company.Id);
    }
}