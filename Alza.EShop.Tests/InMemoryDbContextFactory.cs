using Alza.EShop.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Alza.EShop.Tests;

/// <summary>
/// Factory class for creating InMemory DbContext instances for testing.
/// </summary>
public static class InMemoryDbContextFactory
{
    /// <summary>
    /// Creates a new InMemory EShopDbContext instance with a unique database name.
    /// Each call creates a fresh database instance for isolated testing.
    /// </summary>
    /// <param name="databaseName">Optional database name. If not provided, a unique GUID is used.</param>
    /// <returns>A configured EShopDbContext using InMemory database provider.</returns>
    public static EShopDbContext Create(string? databaseName = null)
    {
        var options = new DbContextOptionsBuilder<EShopDbContext>()
            .UseInMemoryDatabase(databaseName ?? Guid.NewGuid().ToString())
            .Options;

        var context = new EShopDbContext(options);
        
        // Ensure the database is created
        context.Database.EnsureCreated();
        
        return context;
    }

    /// <summary>
    /// Creates a new InMemory EShopDbContext instance and seeds it with test data.
    /// </summary>
    /// <param name="products">Products to seed into the database.</param>
    /// <param name="databaseName">Optional database name. If not provided, a unique GUID is used.</param>
    /// <returns>A configured EShopDbContext with seeded test data.</returns>
    public static EShopDbContext CreateWithData(IEnumerable<Domain.Entities.Product> products, string? databaseName = null)
    {
        var context = Create(databaseName);
        
        context.Products.AddRange(products);
        context.SaveChanges();
        
        return context;
    }
}
