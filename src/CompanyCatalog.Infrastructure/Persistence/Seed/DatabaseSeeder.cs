using CompanyCatalog.Application.Abstractions.Authentication;
using CompanyCatalog.Domain.Categories;
using CompanyCatalog.Domain.Companies;
using CompanyCatalog.Domain.Products;
using CompanyCatalog.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CompanyCatalog.Infrastructure.Persistence.Seed;

public sealed class DatabaseSeeder(
    ApplicationDbContext context,
    IPasswordHasher passwordHasher,
    ILogger<DatabaseSeeder> logger)
{
    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        // Migration'lari otomatik uygula (production-friendly)
        await context.Database.MigrateAsync(cancellationToken);

        await SeedAdminUserAsync(cancellationToken);
        await SeedSampleDataAsync(cancellationToken);
    }

    private async Task SeedAdminUserAsync(CancellationToken ct)
    {
        const string adminEmail = "oguz@hanakpinar.com";

        var exists = await context.Users
            .AnyAsync(u => u.Email == adminEmail, ct);
        if (exists)
        {
            logger.LogInformation("Admin user already exists.");
            return;
        }

        var passwordHash = passwordHasher.Hash("Admin123!");
        var admin = User.Create(
            adminEmail,
            passwordHash,
            "System Admin",
            UserRole.Admin);
        context.Users.Add(admin);
        await context.SaveChangesAsync(ct);

        logger.LogInformation("Admin user seeded: {Email}", adminEmail);
    }

    private async Task SeedSampleDataAsync(CancellationToken ct)
    {
        // Zaten veri varsa atla
        if (await context.Companies.AnyAsync(ct))
        {
            logger.LogInformation("Sample data exists, skipping");
            return;
        }

        // Company'ler
        var techCompany = Company.Create("Tekno A.S.", "1234567890");
        var fashionCompany = Company.Create("Moda Ltd.", "9876543210");
        context.Companies.AddRange(techCompany, fashionCompany);

        // Category'ler
        var electronics = Category.Create("Elektronik", "Elektronik urunler");
        var clothing = Category.Create("Giyim", "Giyim ve aksesuar");
        context.Categories.AddRange(electronics, clothing);

        // Once kaydet ki Id'ler olussun
        await context.SaveChangesAsync(ct);

        // Product'lar (Company ve Category Id'lerini kullan)
        var products = new[]
        {
            Product.Create(
                "Kablosuz Kulaklik", "Bluetooth 5.0",
                "ELKT-001", 1499.99m, 50,
                techCompany.Id, electronics.Id),
            Product.Create(
                "Mekanik Klavye", "RGB aydinlatmali",
                "ELKT-002", 899.50m, 30,
                techCompany.Id, electronics.Id),
            Product.Create(
                "Pamuklu T-Shirt", "100% pamuk",
                "GIYIM-001", 149.90m, 100,
                fashionCompany.Id, clothing.Id)
        };
        context.Products.AddRange(products);
        await context.SaveChangesAsync(ct);

        logger.LogInformation("Sample data seeded successfully");
    }
}