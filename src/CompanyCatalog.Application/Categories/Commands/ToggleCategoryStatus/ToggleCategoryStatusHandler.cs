using CompanyCatalog.Application.Abstractions.Persistence;
using CompanyCatalog.Application.Abstractions.Results;
using CompanyCatalog.Domain.Common;
using MediatR;

namespace CompanyCatalog.Application.Categories.Commands.ToggleCategoryStatus;

internal sealed class ToggleCategoryStatusHandler(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<ToggleCategoryStatusCommand, Result>
{
    public async Task<Result> Handle(ToggleCategoryStatusCommand request, CancellationToken cancellationToken)
    {
        var category = await categoryRepository.GetByIdAsync(request.Id, cancellationToken);
        if (category is null)
            return Result.Failure(Error.NotFound("Category.NotFound",
                $"{request.Id} id'li category sistemde kayıtlı değil."));

        if (category.IsActive)
            category.Deactivate();
        else
            category.Activate();

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}