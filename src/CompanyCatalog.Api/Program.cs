using CompanyCatalog.Application;
using CompanyCatalog.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.MapGet("/", () => "CompanyCatalog API");

app.Run();