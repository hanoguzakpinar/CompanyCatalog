using CompanyCatalog.Application.Abstractions.Persistence;
using CompanyCatalog.Application.Abstractions.Results;
using CompanyCatalog.Application.Companies.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CompanyCatalog.Application.Companies.Queries.GetCompanyById;

internal sealed class GetCompanyByIdQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetCompanyByIdQuery, Result<CompanyResponse>>
{
    public async Task<Result<CompanyResponse>> Handle(GetCompanyByIdQuery request, CancellationToken cancellationToken)
    {
        var company = await context.Companies.AsNoTracking()
            .Where(c => c.Id == request.Id)
            .Select(c => new CompanyResponse(
                c.Id,
                c.Name,
                c.TaxNumber,
                c.IsActive,
                c.CreatedAt,
                c.UpdatedAt
            ))
            .FirstOrDefaultAsync(cancellationToken);

        if (company is null)
            return Result.Failure<CompanyResponse>(Error.NotFound("Company.NotFound",
                $"{request.Id}li company sistemde kayıtlı değil."));

        return Result.Success(company);
    }
}