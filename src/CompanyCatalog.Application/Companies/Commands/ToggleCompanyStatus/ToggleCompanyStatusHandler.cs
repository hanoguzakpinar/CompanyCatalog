using CompanyCatalog.Application.Abstractions.Persistence;
using CompanyCatalog.Application.Abstractions.Results;
using MediatR;

namespace CompanyCatalog.Application.Companies.Commands.ToggleCompanyStatus;

public class ToggleCompanyStatusHandler(ICompanyRepository companyRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<ToggleCompanyStatusCommand, Result>
{
    public async Task<Result> Handle(ToggleCompanyStatusCommand request, CancellationToken cancellationToken)
    {
        var company = await companyRepository.GetByIdAsync(request.Id, cancellationToken);
        if (company is null)
            return Result.Failure(Error.NotFound("Company.NotFound",
                $"{request.Id}li company sistemde kayıtlı değil."));

        if (company.IsActive)
            company.Deactivate();
        else
            company.Activate();

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}