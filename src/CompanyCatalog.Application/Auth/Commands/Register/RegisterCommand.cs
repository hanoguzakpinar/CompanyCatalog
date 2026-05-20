using CompanyCatalog.Application.Abstractions.Results;
using MediatR;

namespace CompanyCatalog.Application.Auth.Commands.Register;

public sealed record RegisterCommand(string Email, string Password, string FullName) : IRequest<Result<Guid>>;