using CompanyCatalog.Application.Abstractions.Results;
using MediatR;

namespace CompanyCatalog.Application.Companies.Commands.UpdateCompany;

public sealed record UpdateCompanyCommand(Guid Id, string Name, string TaxNumber) : IRequest<Result>;