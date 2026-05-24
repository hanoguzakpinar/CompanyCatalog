using CompanyCatalog.Application.Abstractions.Results;
using MediatR;

namespace CompanyCatalog.Application.Categories.Commands.ToggleCategoryStatus;

public sealed record ToggleCategoryStatusCommand(Guid Id) : IRequest<Result>;