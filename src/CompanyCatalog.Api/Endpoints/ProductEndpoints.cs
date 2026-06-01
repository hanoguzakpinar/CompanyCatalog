using CompanyCatalog.Api.Extensions;
using CompanyCatalog.Application.Products.Commands.AddStock;
using CompanyCatalog.Application.Products.Commands.CreateProduct;
using CompanyCatalog.Application.Products.Commands.DeleteProduct;
using CompanyCatalog.Application.Products.Commands.RemoveStock;
using CompanyCatalog.Application.Products.Commands.ToggleProduct;
using CompanyCatalog.Application.Products.Commands.UpdateProductDetails;
using CompanyCatalog.Application.Products.Commands.UpdateProductSku;
using CompanyCatalog.Application.Products.Queries.GetProducts;
using CompanyCatalog.Application.Products.Queries.GetProductsById;
using MediatR;

namespace CompanyCatalog.Api.Endpoints;

public static class ProductEndpoints
{
    public static void MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/products")
            .WithTags("Products")
            .RequireAuthorization();

        group.MapGet("/", async (
                ISender sender,
                CancellationToken ct,
                string? search = null,
                Guid? companyId = null,
                Guid? categoryId = null,
                decimal? minPrice = null,
                decimal? maxPrice = null,
                bool? isActive = null,
                bool? inStock = null,
                int page = 1,
                int pageSize = 20,
                string sortBy = "name",
                bool sortDescending = false
            ) =>
            {
                var query = new GetProductsQuery(search, companyId, categoryId, minPrice, maxPrice, isActive, inStock,
                    page,
                    pageSize, sortBy, sortDescending);
                var result = await sender.Send(query, ct);
                return result.ToHttpResult();
            })
            .WithSummary("Ürünler listeleme");

        group.MapGet("/{id:guid}", async (
                Guid id, ISender sender, CancellationToken ct
            ) =>
            {
                var result = await sender.Send(new GetProductByIdQuery(id), ct);
                return result.ToHttpResult();
            })
            .WithSummary("Id ile ürün sorgulama");

        group.MapPost("/", async (
                CreateProductCommand command,
                ISender sender,
                CancellationToken ct) =>
            {
                var result = await sender.Send(command, ct);
                return result.ToHttpResult(onSuccess: id => Results.Created(
                    $"/api/v1/products/{id}", new { id }));
            })
            .RequireAuthorization("AdminOnly")
            .WithSummary("Ürün kaydetme (Sadece Admin)");

        group.MapPut("/{id:guid}", async (
                Guid id,
                UpdateProductDetailsCommand command,
                ISender sender,
                CancellationToken ct) =>
            {
                if (id != command.Id)
                    return Results.BadRequest(
                        new { message = "Route id ile body id uyumsuz." });
                var result = await sender.Send(command, ct);
                return result.ToHttpResult();
            })
            .RequireAuthorization("AdminOnly")
            .WithSummary("Ürün güncelleme (Sadece Admin)");

        group.MapPatch("/{id:guid}/sku", async (
                Guid id,
                UpdateSkuRequest body,
                ISender sender,
                CancellationToken ct) =>
            {
                var command = new UpdateProductSkuCommand(id, body.Sku);
                var result = await sender.Send(command, ct);
                return result.ToHttpResult();
            })
            .RequireAuthorization("AdminOnly")
            .WithSummary("Ürün sku güncelleme (Sadece Admin)");

        group.MapDelete("/{id:guid}", async (
                ISender sender,
                CancellationToken ct,
                Guid id) =>
            {
                var result = await sender.Send(new DeleteProductCommand(id), ct);
                return result.ToHttpResult();
            })
            .RequireAuthorization("AdminOnly")
            .WithSummary("Ürün silme (Sadece Admin)");

        group.MapPatch("/{id:guid}/toggle-status", async (
                ISender sender,
                CancellationToken ct,
                Guid id) =>
            {
                var result = await sender.Send(new ToggleProductStatusCommand(id), ct);
                return result.ToHttpResult();
            })
            .RequireAuthorization("AdminOnly")
            .WithSummary("Ürün silme (Sadece Admin)");

        group.MapPatch("/{id:guid}/add-stock", async (
                Guid id,
                StockRequest body,
                ISender sender,
                CancellationToken ct) =>
            {
                var result = await sender.Send(
                    new AddStockCommand(id, body.Quantity), ct);
                return result.ToHttpResult();
            })
            .RequireAuthorization("AdminOnly")
            .WithSummary("Ürün stok arttırma (Sadece Admin)");

        group.MapPatch("/{id:guid}/remove-stock", async (
                Guid id,
                StockRequest body,
                ISender sender,
                CancellationToken ct) =>
            {
                var result = await sender.Send(
                    new RemoveStockCommand(id, body.Quantity), ct);
                return result.ToHttpResult();
            })
            .RequireAuthorization("AdminOnly")
            .WithSummary("Ürün stok azaltma (Sadece Admin)");
    }

    public sealed record UpdateSkuRequest(string Sku);

    public sealed record StockRequest(int Quantity);
}