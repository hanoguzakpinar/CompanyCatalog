using CompanyCatalog.Api.Middleware;
using CompanyCatalog.Application;
using CompanyCatalog.Infrastructure;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, config) => config.ReadFrom.Configuration(context.Configuration));

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddOpenApi();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddHealthChecks()
    .AddNpgSql(
        builder.Configuration.GetConnectionString("Database")!,
        name: "postgres",
        tags: new[] { "db", "ready" });

var app = builder.Build();

app.UseExceptionHandler();

app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(opt =>
    {
        opt.WithTitle("CompanyCatalog API");
        opt.WithTheme(ScalarTheme.Purple);
    });
}

app.MapHealthChecks("/health");
app.MapHealthChecks("/health/ready", new()
{
    Predicate = check => check.Tags.Contains("ready")
});

app.MapGet("/", () => "Company Catalog API");

try
{
    Log.Information("API ayağa kaldırılıyor.");
    app.Run();
}
catch (Exception e)
{
    Log.Fatal(e, "Uygulama beklenmedik şekilde kapatıldı.");
}
finally
{
    Log.CloseAndFlush();
}