using CompanyCatalog.Api.Extensions;
using CompanyCatalog.Application.Companies.Commands.CreateCompany;
using CompanyCatalog.Application.Companies.Commands.DeleteCompany;
using CompanyCatalog.Application.Companies.Commands.ToggleCompanyStatus;
using CompanyCatalog.Application.Companies.Commands.UpdateCompany;
using CompanyCatalog.Application.Companies.Queries.GetCompanies;
using CompanyCatalog.Application.Companies.Queries.GetCompanyById;
using MediatR;

namespace CompanyCatalog.Api.Endpoints;

public static class CompanyEndpoints
{
    public static void MapCompanyEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/companies")
            .WithTags("Companies")
            .RequireAuthorization();

        //getCompanies
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
            var query = new GetCompaniesQuery(search, isActive, page, pageSize, sortBy, sortDescending);
            var result = await sender.Send(query, ct);
            return result.ToHttpResult();
        }).WithSummary("Company listesi, filtreleme ve sayfalama ile birlikte.");

        //getCompanyById
        group.MapGet("/{id:guid}", async (Guid id, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(new GetCompanyByIdQuery(id), ct);
                return result.ToHttpResult();
            })
            .WithSummary("Id ile company sorgulama");

        //createCompany
        group.MapPost("/", async (CreateCompanyCommand command, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(command, ct);
                return result.ToHttpResult(onSuccess: id => Results.Created($"/api/v1/companies/{id}", new { id }));
            })
            .RequireAuthorization("AdminOnly")
            .WithSummary("Company oluşturma. (Sadece Admin)");

        //updateCompany
        group.MapPut("/{id:guid}",
                async (Guid id, UpdateCompanyCommand command, ISender sender, CancellationToken ct) =>
                {
                    if (id != command.Id)
                        return Results.BadRequest(new { message = "Route id ile body id uyumsuz." });

                    var result = await sender.Send(command, ct);
                    return result.ToHttpResult();
                })
            .RequireAuthorization("AdminOnly")
            .WithSummary("Company güncelleme. (Sadece Admin)");

        //deleteCompany
        group.MapDelete("/{id:guid}", async (
                Guid id, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(
                    new DeleteCompanyCommand(id), ct);
                return result.ToHttpResult();
            })
            .RequireAuthorization("AdminOnly")
            .WithSummary("Company silme (Sadece Admin)");

        //toggleCompany
        group.MapPatch("/{id:guid}/toggle-status", async (
                Guid id, ISender sender, CancellationToken ct) =>
            {
                var result = await sender.Send(
                    new ToggleCompanyStatusCommand(id), ct);
                return result.ToHttpResult();
            })
            .RequireAuthorization("AdminOnly")
            .WithSummary("Company Aktiflik güncelleme (Sadece Admin)");
    }
}