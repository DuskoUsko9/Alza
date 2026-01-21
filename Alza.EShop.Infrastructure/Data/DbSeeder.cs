using Alza.EShop.Domain.Entities;

namespace Alza.EShop.Infrastructure.Data;

public static class DbSeeder
{
    private static readonly string[] ProductNames =
    [
        "Laptop", "Desktop PC", "Smartphone", "Tablet", "Smart Watch",
        "Wireless Headphones", "Gaming Mouse", "Mechanical Keyboard", "Monitor", "Webcam",
        "External SSD", "Power Bank", "USB Hub", "Docking Station", "Graphics Card",
        "Processor", "RAM Memory", "Motherboard", "Power Supply", "Case",
        "Cooling Fan", "Thermal Paste", "Cable", "Adapter", "Charger",
        "Mic Stand", "Ring Light", "Camera", "Lens", "Tripod"
    ];

    private static readonly string[] ProductAdjectives =
    [
        "Professional", "Gaming", "Portable", "Wireless", "USB-C",
        "High-Performance", "Compact", "Ultra-Fast", "Premium", "Budget",
        "Lightweight", "Durable", "Ergonomic", "Waterproof", "RGB",
        "4K", "8K", "5G", "AI-Powered", "Programmable"
    ];

    public static void SeedData(EShopDbContext context)
    {
        if (context.Products.Any())
            return;

        var products = new List<Product>();
        var random = new Random(42);
        var now = DateTimeOffset.UtcNow;

        for (int i = 0; i < 1000; i++)
        {
            var adjective = ProductAdjectives[random.Next(ProductAdjectives.Length)];
            var productName = ProductNames[random.Next(ProductNames.Length)];
            var name = $"{adjective} {productName} {i + 1}";
            
            var createdAtDays = random.Next(0, 365);
            var createdAt = now.AddDays(-createdAtDays).AddHours(random.Next(0, 24)).AddMinutes(random.Next(0, 60));
            var updatedAtDays = random.Next(0, createdAtDays + 1);
            var updatedAt = now.AddDays(-updatedAtDays).AddHours(random.Next(0, 24)).AddMinutes(random.Next(0, 60));

            products.Add(new Product
            {
                Id = Guid.NewGuid(),
                Name = name,
                ImageUrl = $"https://example.com/images/product-{i + 1}.jpg",
                Price = (decimal)(random.NextDouble() * 2000 + 10),
                Description = $"High-quality product: {name}. Features exceptional performance and reliability.",
                StockQuantity = random.Next(0, 500),
                CreatedAt = createdAt,
                UpdatedAt = updatedAt
            });

            if (products.Count >= 50)
            {
                context.Products.AddRange(products);
                context.SaveChanges();
                products.Clear();
            }
        }

        if (products.Count > 0)
        {
            context.Products.AddRange(products);
            context.SaveChanges();
        }
    }
}
