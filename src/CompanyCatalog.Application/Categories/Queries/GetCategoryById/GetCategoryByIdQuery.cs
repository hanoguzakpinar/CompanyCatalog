using CompanyCatalog.Application.Abstractions.Results;
using CompanyCatalog.Application.Categories.Shared;
using MediatR;

namespace CompanyCatalog.Application.Categories.Queries.GetCategoryById;

public sealed record GetCategoryByIdQuery(Guid Id)
    : IRequest<Result<CategoryResponse>>;