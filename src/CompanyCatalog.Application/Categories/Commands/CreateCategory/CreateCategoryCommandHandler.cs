using CompanyCatalog.Application.Abstractions.Persistence;
using CompanyCatalog.Application.Abstractions.Results;
using CompanyCatalog.Domain.Categories;
using CompanyCatalog.Domain.Common;
using MediatR;

namespace CompanyCatalog.Application.Categories.Commands.CreateCategory;

internal sealed class CreateCategoryCommandHandler(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<CreateCategoryCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        Category category;
        try
        {
            category = Category.Create(request.Name, request.Description);
        }
        catch (DomainException ex)
        {
            return Result.Failure<Guid>(
                Error.Validation("Category.Invalid", ex.Message));
        }

        categoryRepository.Add(category);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(category.Id);
    }
}