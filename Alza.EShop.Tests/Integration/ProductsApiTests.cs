using Alza.EShop.Application.DTOs.Requests;
using Alza.EShop.Application.DTOs.Responses;
using Alza.EShop.Infrastructure.Data;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace Alza.EShop.Tests.Integration;

/// <summary>
/// Integration tests for Products API.
/// Tests full API flow end-to-end using WebApplicationFactory.
/// </summary>
public class ProductsApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _jsonOptions;

    public ProductsApiTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Remove the existing DbContext registration
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<EShopDbContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Generate a unique database name for this test class instance
                // This ensures all DbContext instances in this test share the same database
                var testDbName = $"TestDb_{Guid.NewGuid()}";
                
                // Add DbContext using in-memory database for testing
                services.AddDbContext<EShopDbContext>(options =>
                {
                    options.UseInMemoryDatabase(testDbName);
                });

                // Build service provider
                var sp = services.BuildServiceProvider();

                // Create a scope to obtain a reference to the database context
                using var scope = sp.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<EShopDbContext>();
                
                // Ensure the database is created
                context.Database.EnsureCreated();
                
                // Seed test data
                DbSeeder.SeedData(context);
            });
        });

        _client = _factory.CreateClient();
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    #region GET /api/v1/products Tests

    [Fact]
    public async Task GetAllProducts_ReturnsSuccessStatusCode()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/products");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetAllProducts_ReturnsListOfProducts()
    {
        // Arrange - Create test data using TestDataGenerator
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<EShopDbContext>();
            var testProducts = TestDataGenerator.GenerateProducts(3);
            context.Products.AddRange(testProducts);
            await context.SaveChangesAsync();
        }

        // Act
        var response = await _client.GetAsync("/api/v1/products");
        var content = await response.Content.ReadAsStringAsync();
        var products = JsonSerializer.Deserialize<List<ProductResponse>>(content, _jsonOptions);

        // Assert
        products.Should().NotBeNull();
        products.Should().NotBeEmpty();
        products.Should().OnlyContain(p => !string.IsNullOrEmpty(p.Name));
        products.Should().OnlyContain(p => !string.IsNullOrEmpty(p.ImageUrl));
    }

    #endregion

    #region GET /api/v1/products/{id} Tests

    [Fact]
    public async Task GetProductById_WhenProductExists_ReturnsProduct()
    {
        // Arrange - First get all products to get a valid ID
        var allProductsResponse = await _client.GetAsync("/api/v1/products");
        var allProducts = await allProductsResponse.Content.ReadFromJsonAsync<List<ProductResponse>>(_jsonOptions);
        var existingProductId = allProducts!.First().Id;

        // Act
        var response = await _client.GetAsync($"/api/v1/products/{existingProductId}");
        var product = await response.Content.ReadFromJsonAsync<ProductResponse>(_jsonOptions);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        product.Should().NotBeNull();
        product!.Id.Should().Be(existingProductId);
    }

    [Fact]
    public async Task GetProductById_WhenProductDoesNotExist_Returns404()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/v1/products/{nonExistentId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    #endregion

    #region POST /api/v1/products Tests

    [Fact]
    public async Task CreateProduct_ValidRequest_ReturnsCreatedStatusCode()
    {
        // Arrange
        var request = new CreateProductRequest
        {
            Name = "Integration Test Product",
            ImageUrl = "https://example.com/integration-test.jpg",
            Price = 149.99m,
            Description = "Product created via integration test",
            StockQuantity = 75
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/products", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateProduct_ValidRequest_ReturnsCreatedProduct()
    {
        // Arrange
        var request = new CreateProductRequest
        {
            Name = "New Integration Product",
            ImageUrl = "https://example.com/new-product.jpg",
            Price = 99.99m,
            Description = "New product for testing",
            StockQuantity = 50
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/products", request);
        var product = await response.Content.ReadFromJsonAsync<ProductResponse>(_jsonOptions);

        // Assert
        product.Should().NotBeNull();
        product!.Name.Should().Be(request.Name);
        product.ImageUrl.Should().Be(request.ImageUrl);
        product.Price.Should().Be(request.Price);
        product.Description.Should().Be(request.Description);
        product.StockQuantity.Should().Be(request.StockQuantity);
        product.Id.Should().NotBe(Guid.Empty);
        product.CreatedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromMinutes(1));
    }

    [Fact]
    public async Task CreateProduct_InvalidRequest_Returns400()
    {
        // Arrange
        var invalidRequest = new CreateProductRequest
        {
            Name = "", // Invalid: empty name
            ImageUrl = "not-a-url", // Invalid: not a valid URL
            Price = -10m // Invalid: negative price
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/products", invalidRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateProduct_CreatedProductCanBeRetrieved()
    {
        // Arrange
        var request = new CreateProductRequest
        {
            Name = "Retrievable Product",
            ImageUrl = "https://example.com/retrievable.jpg",
            Price = 199.99m,
            StockQuantity = 100
        };

        // Act - Create product
        var createResponse = await _client.PostAsJsonAsync("/api/v1/products", request);
        var createdProduct = await createResponse.Content.ReadFromJsonAsync<ProductResponse>(_jsonOptions);

        // Act - Retrieve the created product
        var getResponse = await _client.GetAsync($"/api/v1/products/{createdProduct!.Id}");
        var retrievedProduct = await getResponse.Content.ReadFromJsonAsync<ProductResponse>(_jsonOptions);

        // Assert
        retrievedProduct.Should().NotBeNull();
        retrievedProduct!.Id.Should().Be(createdProduct.Id);
        retrievedProduct.Name.Should().Be(request.Name);
        retrievedProduct.ImageUrl.Should().Be(request.ImageUrl);
    }

    #endregion

    #region PATCH /api/v1/products/{id}/stock Tests

    [Fact]
    public async Task UpdateStock_ValidRequest_ReturnsOkStatusCode()
    {
        // Arrange - Get an existing product
        var allProductsResponse = await _client.GetAsync("/api/v1/products");
        var allProducts = await allProductsResponse.Content.ReadFromJsonAsync<List<ProductResponse>>(_jsonOptions);
        var existingProductId = allProducts!.First().Id;

        var request = new UpdateStockRequest
        {
            StockQuantity = 200
        };

        // Act
        var response = await _client.PatchAsync(
            $"/api/v1/products/{existingProductId}/stock",
            JsonContent.Create(request));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateStock_ValidRequest_UpdatesStockQuantity()
    {
        // Arrange - Get an existing product
        var allProductsResponse = await _client.GetAsync("/api/v1/products");
        var allProducts = await allProductsResponse.Content.ReadFromJsonAsync<List<ProductResponse>>(_jsonOptions);
        var existingProductId = allProducts!.First().Id;

        var request = new UpdateStockRequest
        {
            StockQuantity = 150
        };

        // Act
        var response = await _client.PatchAsync(
            $"/api/v1/products/{existingProductId}/stock",
            JsonContent.Create(request));
        var updatedProduct = await response.Content.ReadFromJsonAsync<ProductResponse>(_jsonOptions);

        // Assert
        updatedProduct.Should().NotBeNull();
        updatedProduct!.StockQuantity.Should().Be(150);
        updatedProduct.UpdatedAt.Should().NotBeNull();
        updatedProduct.UpdatedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromMinutes(1));
    }

    [Fact]
    public async Task UpdateStock_WhenProductDoesNotExist_Returns404()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var request = new UpdateStockRequest
        {
            StockQuantity = 100
        };

        // Act
        var response = await _client.PatchAsync(
            $"/api/v1/products/{nonExistentId}/stock",
            JsonContent.Create(request));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateStock_InvalidRequest_Returns400()
    {
        // Arrange - Get an existing product
        var allProductsResponse = await _client.GetAsync("/api/v1/products");
        var allProducts = await allProductsResponse.Content.ReadFromJsonAsync<List<ProductResponse>>(_jsonOptions);
        var existingProductId = allProducts!.First().Id;

        var invalidRequest = new UpdateStockRequest
        {
            StockQuantity = -50 // Invalid: negative quantity
        };

        // Act
        var response = await _client.PatchAsync(
            $"/api/v1/products/{existingProductId}/stock",
            JsonContent.Create(invalidRequest));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateStock_WithZeroQuantity_UpdatesSuccessfully()
    {
        // Arrange - Get an existing product
        var allProductsResponse = await _client.GetAsync("/api/v1/products");
        var allProducts = await allProductsResponse.Content.ReadFromJsonAsync<List<ProductResponse>>(_jsonOptions);
        var existingProductId = allProducts!.First().Id;

        var request = new UpdateStockRequest
        {
            StockQuantity = 0
        };

        // Act
        var response = await _client.PatchAsync(
            $"/api/v1/products/{existingProductId}/stock",
            JsonContent.Create(request));
        var updatedProduct = await response.Content.ReadFromJsonAsync<ProductResponse>(_jsonOptions);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        updatedProduct!.StockQuantity.Should().Be(0);
    }

    #endregion

    #region End-to-End Workflow Tests

    [Fact]
    public async Task FullWorkflow_CreateRetrieveAndUpdateProduct()
    {
        // Step 1: Create a new product
        var createRequest = new CreateProductRequest
        {
            Name = "Workflow Test Product",
            ImageUrl = "https://example.com/workflow.jpg",
            Price = 299.99m,
            Description = "Product for workflow testing",
            StockQuantity = 100
        };

        var createResponse = await _client.PostAsJsonAsync("/api/v1/products", createRequest);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdProduct = await createResponse.Content.ReadFromJsonAsync<ProductResponse>(_jsonOptions);

        // Step 2: Retrieve the created product
        var getResponse = await _client.GetAsync($"/api/v1/products/{createdProduct!.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var retrievedProduct = await getResponse.Content.ReadFromJsonAsync<ProductResponse>(_jsonOptions);
        retrievedProduct!.Name.Should().Be(createRequest.Name);

        // Step 3: Update the product's stock
        var updateRequest = new UpdateStockRequest { StockQuantity = 250 };
        var updateResponse = await _client.PatchAsync(
            $"/api/v1/products/{createdProduct.Id}/stock",
            JsonContent.Create(updateRequest));
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var updatedProduct = await updateResponse.Content.ReadFromJsonAsync<ProductResponse>(_jsonOptions);
        updatedProduct!.StockQuantity.Should().Be(250);
    }

    #endregion
}
