using Alza.EShop.Application.DTOs.Requests;
using Alza.EShop.Application.DTOs.Responses;
using Alza.EShop.Domain.Entities;

namespace Alza.EShop.Tests;

/// <summary>
/// Static helper class for generating test data for unit and integration tests.
/// </summary>
public static class TestDataGenerator
{
    /// <summary>
    /// Generates a list of Product entities for testing.
    /// </summary>
    /// <param name="count">Number of products to generate.</param>
    /// <returns>List of products with test data.</returns>
    public static List<Product> GenerateProducts(int count)
    {
        var products = new List<Product>();
        for (int i = 1; i <= count; i++)
        {
            products.Add(new Product
            {
                Id = Guid.NewGuid(),
                Name = $"Test Product {i}",
                ImageUrl = $"https://example.com/product-{i}.jpg",
                Price = 100m * i,
                Description = $"Description for test product {i}",
                StockQuantity = 10 * i,
                CreatedAt = DateTimeOffset.UtcNow.AddDays(-i),
                UpdatedAt = DateTimeOffset.UtcNow.AddDays(-i)
            });
        }
        return products;
    }

    /// <summary>
    /// Generates a single Product entity for testing.
    /// </summary>
    /// <param name="id">Optional product ID. If not provided, a new GUID is generated.</param>
    /// <returns>A product entity with test data.</returns>
    public static Product GenerateProduct(Guid? id = null)
    {
        return new Product
        {
            Id = id ?? Guid.NewGuid(),
            Name = "Test Product",
            ImageUrl = "https://example.com/product.jpg",
            Price = 99.99m,
            Description = "Test product description",
            StockQuantity = 50,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };
    }

    /// <summary>
    /// Generates a list of ProductResponse DTOs for testing.
    /// </summary>
    /// <param name="count">Number of product responses to generate.</param>
    /// <returns>List of product responses with test data.</returns>
    public static List<ProductResponse> GenerateProductResponses(int count)
    {
        return [.. GenerateProducts(count)
            .Select(p => new ProductResponse
            {
                Id = p.Id,
                Name = p.Name,
                ImageUrl = p.ImageUrl,
                Price = p.Price,
                Description = p.Description,
                StockQuantity = p.StockQuantity,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            })];
    }

    /// <summary>
    /// Generates a single ProductResponse DTO for testing.
    /// </summary>
    /// <param name="id">Optional product ID. If not provided, a new GUID is generated.</param>
    /// <returns>A product response with test data.</returns>
    public static ProductResponse GenerateProductResponse(Guid? id = null)
    {
        var product = GenerateProduct(id);
        return new ProductResponse
        {
            Id = product.Id,
            Name = product.Name,
            ImageUrl = product.ImageUrl,
            Price = product.Price,
            Description = product.Description,
            StockQuantity = product.StockQuantity,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        };
    }

    /// <summary>
    /// Generates a valid CreateProductRequest for testing.
    /// </summary>
    /// <returns>A valid create product request.</returns>
    public static CreateProductRequest GenerateCreateProductRequest()
    {
        return new CreateProductRequest
        {
            Name = "New Test Product",
            ImageUrl = "https://example.com/new-product.jpg",
            Price = 149.99m,
            Description = "New test product description",
            StockQuantity = 100
        };
    }

    /// <summary>
    /// Generates a valid UpdateStockRequest for testing.
    /// </summary>
    /// <param name="quantity">Stock quantity. Default is 75.</param>
    /// <returns>A valid update stock request.</returns>
    public static UpdateStockRequest GenerateUpdateStockRequest(int quantity = 75)
    {
        return new UpdateStockRequest
        {
            StockQuantity = quantity
        };
    }

    /// <summary>
    /// Generates an invalid CreateProductRequest with missing required fields.
    /// </summary>
    /// <returns>An invalid create product request.</returns>
    public static CreateProductRequest GenerateInvalidCreateProductRequest()
    {
        return new CreateProductRequest
        {
            Name = "", // Invalid: empty name
            ImageUrl = "", // Invalid: empty URL
            Price = -10m, // Invalid: negative price
            StockQuantity = -5 // Invalid: negative stock
        };
    }

    /// <summary>
    /// Generates an invalid UpdateStockRequest with negative quantity.
    /// </summary>
    /// <returns>An invalid update stock request.</returns>
    public static UpdateStockRequest GenerateInvalidUpdateStockRequest()
    {
        return new UpdateStockRequest
        {
            StockQuantity = -10 // Invalid: negative quantity
        };
    }

    /// <summary>
    /// Generates a CreateProductRequest with name exceeding max length.
    /// </summary>
    /// <returns>A create product request with invalid name length.</returns>
    public static CreateProductRequest GenerateCreateProductRequestWithLongName()
    {
        return new CreateProductRequest
        {
            Name = new string('A', 201), // Exceeds 200 character limit
            ImageUrl = "https://example.com/product.jpg",
            Price = 99.99m
        };
    }

    /// <summary>
    /// Generates a CreateProductRequest with invalid URL format.
    /// </summary>
    /// <returns>A create product request with invalid URL.</returns>
    public static CreateProductRequest GenerateCreateProductRequestWithInvalidUrl()
    {
        return new CreateProductRequest
        {
            Name = "Test Product",
            ImageUrl = "not-a-valid-url", // Invalid URL format
            Price = 99.99m
        };
    }
}
