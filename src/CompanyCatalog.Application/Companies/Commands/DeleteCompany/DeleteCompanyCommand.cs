using CompanyCatalog.Application.Abstractions.Results;
using MediatR;

namespace CompanyCatalog.Application.Companies.Commands.DeleteCompany;

public sealed record DeleteCompanyCommand(Guid Id) : IRequest<Result>;