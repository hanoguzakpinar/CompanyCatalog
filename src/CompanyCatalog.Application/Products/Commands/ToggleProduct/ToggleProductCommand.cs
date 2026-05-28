using CompanyCatalog.Application.Abstractions.Results;
using MediatR;

namespace CompanyCatalog.Application.Products.Commands.ToggleProduct;

public sealed record ToggleProductCommand(Guid Id) : IRequest<Result>;