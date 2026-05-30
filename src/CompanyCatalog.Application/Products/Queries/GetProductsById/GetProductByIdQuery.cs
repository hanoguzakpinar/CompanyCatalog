using CompanyCatalog.Application.Abstractions.Results;
using CompanyCatalog.Application.Products.Shared;
using MediatR;

namespace CompanyCatalog.Application.Products.Queries.GetProductsById;

public sealed record GetProductByIdQuery(
    Guid Id) : IRequest<Result<ProductResponse>>;