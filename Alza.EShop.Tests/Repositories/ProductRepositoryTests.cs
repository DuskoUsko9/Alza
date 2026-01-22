using Alza.EShop.Domain.Entities;
using Alza.EShop.Infrastructure.Repositories;
using FluentAssertions;

namespace Alza.EShop.Tests.Repositories;

/// <summary>
/// Unit tests for ProductRepository.
/// Tests all CRUD operations and pagination using InMemory database.
/// </summary>
public class ProductRepositoryTests
{
    [Fact]
    public async Task GetAllAsync_ReturnsAllProducts()
    {
        // Arrange
        var products = TestDataGenerator.GenerateProducts(5);
        var context = InMemoryDbContextFactory.CreateWithData(products);
        var repository = new ProductRepository(context);

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(5);
        result.Should().BeInAscendingOrder(p => p.Name);
    }

    [Fact]
    public async Task GetAllAsync_WhenNoProducts_ReturnsEmptyList()
    {
        // Arrange
        var context = InMemoryDbContextFactory.Create();
        var repository = new ProductRepository(context);

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetByIdAsync_WhenProductExists_ReturnsProduct()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var product = TestDataGenerator.GenerateProduct(productId);
        var context = InMemoryDbContextFactory.CreateWithData(new[] { product });
        var repository = new ProductRepository(context);

        // Act
        var result = await repository.GetByIdAsync(productId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(productId);
        result.Name.Should().Be(product.Name);
        result.ImageUrl.Should().Be(product.ImageUrl);
    }

    [Fact]
    public async Task GetByIdAsync_WhenProductDoesNotExist_ReturnsNull()
    {
        // Arrange
        var context = InMemoryDbContextFactory.Create();
        var repository = new ProductRepository(context);
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await repository.GetByIdAsync(nonExistentId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateAsync_CreatesProductSuccessfully()
    {
        // Arrange
        var context = InMemoryDbContextFactory.Create();
        var repository = new ProductRepository(context);
        var newProduct = new Product
        {
            Id = Guid.NewGuid(),
            Name = "New Product",
            ImageUrl = "https://example.com/new-product.jpg",
            Price = 199.99m,
            Description = "New product description",
            StockQuantity = 50
        };

        // Act
        var result = await repository.CreateAsync(newProduct);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(newProduct.Id);
        result.Name.Should().Be(newProduct.Name);
        result.CreatedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1));
        result.UpdatedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1));

        // Verify it was saved to database
        var savedProduct = await repository.GetByIdAsync(newProduct.Id);
        savedProduct.Should().NotBeNull();
        savedProduct!.Name.Should().Be(newProduct.Name);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesProductSuccessfully()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var product = TestDataGenerator.GenerateProduct(productId);
        var context = InMemoryDbContextFactory.CreateWithData(new[] { product });
        var repository = new ProductRepository(context);

        // Modify the product
        product.Name = "Updated Product Name";
        product.Price = 299.99m;
        product.StockQuantity = 100;

        // Act
        var result = await repository.UpdateAsync(product);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Updated Product Name");
        result.Price.Should().Be(299.99m);
        result.StockQuantity.Should().Be(100);
        result.UpdatedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1));

        // Verify changes were saved
        var updatedProduct = await repository.GetByIdAsync(productId);
        updatedProduct!.Name.Should().Be("Updated Product Name");
    }

    [Fact]
    public async Task DeleteAsync_WhenProductExists_DeletesSuccessfully()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var product = TestDataGenerator.GenerateProduct(productId);
        var context = InMemoryDbContextFactory.CreateWithData(new[] { product });
        var repository = new ProductRepository(context);

        // Act
        var result = await repository.DeleteAsync(productId);

        // Assert
        result.Should().BeTrue();

        // Verify product was deleted
        var deletedProduct = await repository.GetByIdAsync(productId);
        deletedProduct.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_WhenProductDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var context = InMemoryDbContextFactory.Create();
        var repository = new ProductRepository(context);
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await repository.DeleteAsync(nonExistentId);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task ExistsAsync_WhenProductExists_ReturnsTrue()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var product = TestDataGenerator.GenerateProduct(productId);
        var context = InMemoryDbContextFactory.CreateWithData(new[] { product });
        var repository = new ProductRepository(context);

        // Act
        var result = await repository.ExistsAsync(productId);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsAsync_WhenProductDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var context = InMemoryDbContextFactory.Create();
        var repository = new ProductRepository(context);
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await repository.ExistsAsync(nonExistentId);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task GetPagedAsync_ReturnsCorrectPageOfProducts()
    {
        // Arrange
        var products = TestDataGenerator.GenerateProducts(25);
        var context = InMemoryDbContextFactory.CreateWithData(products);
        var repository = new ProductRepository(context);

        // Act
        var (items, totalCount) = await repository.GetPagedAsync(pageNumber: 2, pageSize: 10);

        // Assert
        items.Should().NotBeNull();
        items.Should().HaveCount(10);
        totalCount.Should().Be(25);
        items.Should().BeInAscendingOrder(p => p.Name);
    }

    [Fact]
    public async Task GetPagedAsync_FirstPage_ReturnsCorrectProducts()
    {
        // Arrange
        var products = TestDataGenerator.GenerateProducts(15);
        var context = InMemoryDbContextFactory.CreateWithData(products);
        var repository = new ProductRepository(context);

        // Act
        var (items, totalCount) = await repository.GetPagedAsync(pageNumber: 1, pageSize: 10);

        // Assert
        items.Should().HaveCount(10);
        totalCount.Should().Be(15);
    }

    [Fact]
    public async Task GetPagedAsync_LastPage_ReturnsRemainingProducts()
    {
        // Arrange
        var products = TestDataGenerator.GenerateProducts(25);
        var context = InMemoryDbContextFactory.CreateWithData(products);
        var repository = new ProductRepository(context);

        // Act
        var (items, totalCount) = await repository.GetPagedAsync(pageNumber: 3, pageSize: 10);

        // Assert
        items.Should().HaveCount(5); // Only 5 items on last page
        totalCount.Should().Be(25);
    }

    [Fact]
    public async Task GetPagedAsync_WhenNoProducts_ReturnsEmptyPage()
    {
        // Arrange
        var context = InMemoryDbContextFactory.Create();
        var repository = new ProductRepository(context);

        // Act
        var (items, totalCount) = await repository.GetPagedAsync(pageNumber: 1, pageSize: 10);

        // Assert
        items.Should().BeEmpty();
        totalCount.Should().Be(0);
    }

    [Fact]
    public async Task GetPagedAsync_WithPageBeyondTotalPages_ReturnsEmptyList()
    {
        // Arrange
        var products = TestDataGenerator.GenerateProducts(5);
        var context = InMemoryDbContextFactory.CreateWithData(products);
        var repository = new ProductRepository(context);

        // Act
        var (items, totalCount) = await repository.GetPagedAsync(pageNumber: 10, pageSize: 10);

        // Assert
        items.Should().BeEmpty();
        totalCount.Should().Be(5);
    }
}
