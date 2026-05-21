using System.Security.Claims;
using CompanyCatalog.Api.Extensions;
using CompanyCatalog.Application.Auth.Commands.Login;
using CompanyCatalog.Application.Auth.Commands.Register;
using MediatR;

namespace CompanyCatalog.Api.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/auth").WithTags("Authentication");

        group.MapPost("/register", async (
                RegisterCommand command,
                ISender sender,
                CancellationToken ct
            ) =>
            {
                var result = await sender.Send(command, ct);
                return result.ToHttpResult(
                    onSuccess: id => Results.Created($"api/v1/users/{id}", new { id }));
            })
            .WithSummary("Yeni kullanıcı kaydı.")
            .Produces<object>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapPost("/login", async (
                LoginCommand command,
                ISender sender,
                CancellationToken ct) =>
            {
                var result = await sender.Send(command, ct);
                return result.ToHttpResult();
            })
            .WithSummary("Giriş yap ve Jwt token al.")
            .Produces<LoginResponse>(StatusCodes.Status200OK)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status401Unauthorized);

        group.MapGet("/me", (ClaimsPrincipal user) =>
            {
                var userId = user.FindFirstValue(ClaimTypes.NameIdentifier)
                             ?? user.FindFirstValue("sub");
                var email = user.FindFirstValue(ClaimTypes.Email)
                            ?? user.FindFirstValue("email");
                var role = user.FindFirstValue(ClaimTypes.Role);
                var fullName = user.FindFirstValue("fullName");

                return Results.Ok(new
                {
                    UserId = userId,
                    Email = email,
                    FullName = fullName,
                    Role = role
                });
            })
            .RequireAuthorization()
            .WithSummary("Token sahibi kullanıcının bilgileri")
            .Produces<object>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized);
    }
}