using CompanyCatalog.Application.Abstractions.Results;
using MediatR;

namespace CompanyCatalog.Application.Auth.Commands.Login;

public sealed record LoginCommand(string Email, string Password) : IRequest<Result<LoginResponse>>;