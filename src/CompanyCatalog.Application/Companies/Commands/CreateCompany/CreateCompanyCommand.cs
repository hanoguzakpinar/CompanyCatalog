using CompanyCatalog.Application.Abstractions.Results;
using MediatR;

namespace CompanyCatalog.Application.Companies.Commands.CreateCompany;

public sealed record CreateCompanyCommand(string Name, string TaxNumber) : IRequest<Result<Guid>>;