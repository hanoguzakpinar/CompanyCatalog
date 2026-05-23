using CompanyCatalog.Application.Abstractions.Results;
using MediatR;

namespace CompanyCatalog.Application.Companies.Commands.ToggleCompanyStatus;

public sealed record ToggleCompanyStatusCommand(Guid Id) : IRequest<Result>;