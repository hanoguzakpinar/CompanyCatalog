using CompanyCatalog.Application.Abstractions.Results;
using MediatR;

namespace CompanyCatalog.Application.Products.Commands.CreateProduct;

public sealed record CreateProductCommand(
    string Name,
    string? Description,
    string Sku,
    decimal Price,
    int StockQuantity,
    Guid CompanyId,
    Guid CategoryId) : IRequest<Result<Guid>>;