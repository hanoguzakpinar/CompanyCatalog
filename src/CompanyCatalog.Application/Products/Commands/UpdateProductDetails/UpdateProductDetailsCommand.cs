using CompanyCatalog.Application.Abstractions.Results;
using MediatR;

namespace CompanyCatalog.Application.Products.Commands.UpdateProductDetails;

public sealed record UpdateProductDetailsCommand(
    Guid Id,
    string Name,
    string? Description,
    decimal Price
) : IRequest<Result>;