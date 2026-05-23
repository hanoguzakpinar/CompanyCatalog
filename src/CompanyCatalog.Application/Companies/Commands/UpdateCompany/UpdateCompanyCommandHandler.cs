using CompanyCatalog.Application.Abstractions.Persistence;
using CompanyCatalog.Application.Abstractions.Results;
using CompanyCatalog.Domain.Common;
using MediatR;

namespace CompanyCatalog.Application.Companies.Commands.UpdateCompany;

internal sealed class UpdateCompanyCommandHandler(ICompanyRepository companyRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateCompanyCommand, Result>
{
    public async Task<Result> Handle(UpdateCompanyCommand request, CancellationToken cancellationToken)
    {
        var company = await companyRepository.GetByIdAsync(request.Id, cancellationToken);
        if (company is null)
            return Result.Failure(Error.NotFound("Company.NotFound",
                $"{request.Id}li company sistemde kayıtlı değil."));

        if (company.TaxNumber != request.TaxNumber)
        {
            var isExistTaxNumber = await companyRepository.ExistsByTaxNumberAsync(request.TaxNumber, cancellationToken);
            if (isExistTaxNumber)
                return Result.Failure(Error.Conflict("Company.DuplicateTaxNumber",
                    "Bu tax number sistemizde zaten kayıtlıdır."));
        }

        try
        {
            company.Rename(request.Name);
            company.UpdateTaxNumber(request.TaxNumber);
        }
        catch (DomainException e)
        {
            return Result.Failure(Error.Validation("Company.Invalid", e.Message));
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(company.Id);
    }
}