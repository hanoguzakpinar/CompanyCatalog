using CompanyCatalog.Api.Extensions;
using CompanyCatalog.Application.Categories.Commands.CreateCategory;
using CompanyCatalog.Application.Categories.Commands.DeleteCategory;
using CompanyCatalog.Application.Categories.Commands.ToggleCategoryStatus;
using CompanyCatalog.Application.Categories.Commands.UpdateCategory;
using CompanyCatalog.Application.Categories.Queries.GetCategories;
using CompanyCatalog.Application.Categories.Queries.GetCategoryById;
using MediatR;

namespace CompanyCatalog.Api.Endpoints;

public static class CategoryEndpoints
{
    public static void MapCategoryEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/categories")
            .WithTags("Categories")
            .RequireAuthorization();

        group.MapGet("/", async (
                ISender sender,
                CancellationToken ct,
                string? search = null,
                bool? isActive = null,
                int page = 1,
                int pageSize = 20,
                string sortBy = "name",
                bool sortDescending = false
            ) =>
            {
                var query = new GetCategoriesQuery(search, isActive, page, pageSize, sortBy, sortDescending);
                var result = await sender.Send(query, ct);
                return result.ToHttpResult();
            })
            .WithSummary("Kategori listesi, filtreleme ve sayfalama ile birlikte.");

        group.MapGet("/{id:guid}", async (
                Guid id,
                ISender sender,
                CancellationToken ct
            ) =>
            {
                var query = new GetCategoryByIdQuery(id);
                var result = await sender.Send(query, ct);
                return result.ToHttpResult();
            })
            .WithSummary("Id ile kategori sorgulama");

        group.MapPost("/", async (
                ISender sender,
                CancellationToken ct,
                CreateCategoryCommand command
            ) =>
            {
                var result = await sender.Send(command, ct);
                return result.ToHttpResult(onSuccess: id => Results.Created($"api/v1/categories/{id}", new { id }));
            })
            .RequireAuthorization("AdminOnly")
            .WithSummary("Kategori oluşturma. (Sadece Admin)");

        group.MapPut("/{id:guid}", async (
                ISender sender,
                CancellationToken ct,
                UpdateCategoryCommand command,
                Guid id
            ) =>
            {
                if (id != command.Id)
                    return Results.BadRequest(
                        new { message = "Route id does not match body id" });

                var result = await sender.Send(command, ct);
                return result.ToHttpResult();
            })
            .RequireAuthorization("AdminOnly")
            .WithSummary("Kategori güncelleme. (Sadece Admin)");

        group.MapDelete("/{id:guid}", async (
                ISender sender,
                CancellationToken ct,
                Guid id
            ) =>
            {
                var result = await sender.Send(new DeleteCategoryCommand(id), ct);
                return result.ToHttpResult();
            })
            .RequireAuthorization("AdminOnly")
            .WithSummary("Kategori silme (Sadece Admin)");

        group.MapPatch("/{id:guid}/toggle-status", async (
                ISender sender,
                CancellationToken ct,
                Guid id) =>
            {
                var result = await sender.Send(new ToggleCategoryStatusCommand(id), ct);
                return result.ToHttpResult();
            })
            .RequireAuthorization("AdminOnly")
            .WithSummary("Kategori Aktiflik güncelleme (Sadece Admin)");
    }
}