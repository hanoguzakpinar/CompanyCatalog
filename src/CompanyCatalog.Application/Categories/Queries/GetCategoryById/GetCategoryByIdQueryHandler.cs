using CompanyCatalog.Application.Abstractions.Persistence;
using CompanyCatalog.Application.Abstractions.Results;
using CompanyCatalog.Application.Categories.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CompanyCatalog.Application.Categories.Queries.GetCategoryById;

internal sealed class GetCategoryByIdQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetCategoryByIdQuery, Result<CategoryResponse>>
{
    public async Task<Result<CategoryResponse>> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var category = await context.Categories.AsNoTracking()
            .Where(c => c.Id == request.Id)
            .Select(c => new CategoryResponse(
                c.Id,
                c.Name,
                c.Description,
                c.IsActive,
                c.CreatedAt,
                c.UpdatedAt
            ))
            .FirstOrDefaultAsync(cancellationToken);

        if (category is null)
            return Result.Failure<CategoryResponse>(Error.NotFound("Category.NotFound",
                $"{request.Id} id'li category sistemde kayıtlı değil."));

        return Result.Success(category);
    }
}