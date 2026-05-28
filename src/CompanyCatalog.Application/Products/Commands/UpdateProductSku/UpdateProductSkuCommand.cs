using CompanyCatalog.Application.Abstractions.Results;
using MediatR;

namespace CompanyCatalog.Application.Products.Commands.UpdateProductSku;

public sealed record UpdateProductSkuCommand(
    Guid Id,
    string Sku
) : IRequest<Result>;