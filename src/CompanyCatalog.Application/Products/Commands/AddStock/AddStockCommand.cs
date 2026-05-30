using CompanyCatalog.Application.Abstractions.Results;
using MediatR;

namespace CompanyCatalog.Application.Products.Commands.AddStock;

public sealed record AddStockCommand(
    Guid ProductId,
    int Quantity) : IRequest<Result>;