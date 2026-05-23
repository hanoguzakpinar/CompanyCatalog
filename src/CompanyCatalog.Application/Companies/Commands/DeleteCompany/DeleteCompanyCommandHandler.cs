using CompanyCatalog.Application.Abstractions.Persistence;
using CompanyCatalog.Application.Abstractions.Results;
using MediatR;

namespace CompanyCatalog.Application.Companies.Commands.DeleteCompany;

internal sealed class DeleteCompanyCommandHandler(ICompanyRepository companyRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteCompanyCommand, Result>
{
    public async Task<Result> Handle(DeleteCompanyCommand request, CancellationToken cancellationToken)
    {
        var company = await companyRepository.GetByIdAsync(request.Id, cancellationToken);
        if (company is null)
            return Result.Failure(Error.NotFound("Company.NotFound",
                $"{request.Id}li company sistemde kayıtlı değil."));

        if (company.IsActive)
            company.Deactivate();

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}