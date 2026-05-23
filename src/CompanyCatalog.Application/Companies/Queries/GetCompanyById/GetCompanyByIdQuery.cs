using CompanyCatalog.Application.Abstractions.Results;
using CompanyCatalog.Application.Companies.Shared;
using MediatR;

namespace CompanyCatalog.Application.Companies.Queries.GetCompanyById;

public sealed record GetCompanyByIdQuery(Guid Id) : IRequest<Result<CompanyResponse>>;