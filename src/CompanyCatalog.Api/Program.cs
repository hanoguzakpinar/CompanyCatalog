using CompanyCatalog.Application;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();

var app = builder.Build();

app.MapGet("/", () => "CompanyCatalog API");

app.Run();