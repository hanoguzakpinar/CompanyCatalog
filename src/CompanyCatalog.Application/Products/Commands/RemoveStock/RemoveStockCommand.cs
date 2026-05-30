using CompanyCatalog.Application.Abstractions.Results;
using MediatR;

namespace CompanyCatalog.Application.Products.Commands.RemoveStock;

public sealed record RemoveStockCommand(
    Guid ProductId,
    int Quantity) : IRequest<Result>;