using Alza.EShop.Application.Common.Exceptions;
using Alza.EShop.Application.DTOs.Requests;
using Alza.EShop.Application.DTOs.Responses;
using Alza.EShop.Application.Services.Implementations;
using Alza.EShop.Domain.Entities;
using Alza.EShop.Domain.Interfaces;
using AutoMapper;
using FluentAssertions;
using Moq;

namespace Alza.EShop.Tests.Services;

/// <summary>
/// Unit tests for ProductService.
/// Tests business logic using mocked dependencies.
/// </summary>
public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly ProductService _service;

    public ProductServiceTests()
    {
        _repositoryMock = new Mock<IProductRepository>();
        _mapperMock = new Mock<IMapper>();
        _service = new ProductService(
            _repositoryMock.Object,
            _mapperMock.Object);
    }

    #region GetAllAsync Tests

    [Fact]
    public async Task GetAllAsync_CallsRepositoryAndMapsCorrectly()
    {
        // Arrange
        var products = TestDataGenerator.GenerateProducts(3);
        var productResponses = TestDataGenerator.GenerateProductResponses(3);

        _repositoryMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(products);
        _mapperMock.Setup(m => m.Map<IEnumerable<ProductResponse>>(products))
            .Returns(productResponses);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        _repositoryMock.Verify(r => r.GetAllAsync(), Times.Once);
        _mapperMock.Verify(m => m.Map<IEnumerable<ProductResponse>>(products), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_WhenNoProducts_ReturnsEmptyList()
    {
        // Arrange
        var emptyList = new List<Product>();
        var emptyResponseList = new List<ProductResponse>();

        _repositoryMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(emptyList);
        _mapperMock.Setup(m => m.Map<IEnumerable<ProductResponse>>(emptyList))
            .Returns(emptyResponseList);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
        _repositoryMock.Verify(r => r.GetAllAsync(), Times.Once);
    }

    #endregion

    #region GetByIdAsync Tests

    [Fact]
    public async Task GetByIdAsync_WhenProductExists_ReturnsProductResponse()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var product = TestDataGenerator.GenerateProduct(productId);
        var productResponse = TestDataGenerator.GenerateProductResponse(productId);

        _repositoryMock.Setup(r => r.GetByIdAsync(productId))
            .ReturnsAsync(product);
        _mapperMock.Setup(m => m.Map<ProductResponse>(product))
            .Returns(productResponse);

        // Act
        var result = await _service.GetByIdAsync(productId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(productId);
        result.Name.Should().Be(productResponse.Name);
        _repositoryMock.Verify(r => r.GetByIdAsync(productId), Times.Once);
        _mapperMock.Verify(m => m.Map<ProductResponse>(product), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WhenProductDoesNotExist_ThrowsNotFoundException()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        _repositoryMock.Setup(r => r.GetByIdAsync(nonExistentId))
            .ReturnsAsync((Product?)null);

        // Act
        Func<Task> act = async () => await _service.GetByIdAsync(nonExistentId);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Product with ID {nonExistentId} was not found");
        _repositoryMock.Verify(r => r.GetByIdAsync(nonExistentId), Times.Once);
        _mapperMock.Verify(m => m.Map<ProductResponse>(It.IsAny<Product>()), Times.Never);
    }

    #endregion

    #region CreateAsync Tests

    [Fact]
    public async Task CreateAsync_ValidRequest_CreatesAndReturnsProductResponse()
    {
        // Arrange
        var request = TestDataGenerator.GenerateCreateProductRequest();
        var product = new Product
        {
            Name = request.Name,
            ImageUrl = request.ImageUrl,
            Price = request.Price,
            Description = request.Description,
            StockQuantity = request.StockQuantity
        };
        var productResponse = new ProductResponse
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            ImageUrl = request.ImageUrl,
            Price = request.Price,
            Description = request.Description,
            StockQuantity = request.StockQuantity,
            CreatedAt = DateTimeOffset.UtcNow
        };

        _mapperMock.Setup(m => m.Map<Product>(request))
            .Returns(product);
        _repositoryMock.Setup(r => r.CreateAsync(It.IsAny<Product>()))
            .ReturnsAsync((Product p) =>
            {
                p.CreatedAt = DateTimeOffset.UtcNow;
                return p;
            });
        _mapperMock.Setup(m => m.Map<ProductResponse>(It.IsAny<Product>()))
            .Returns(productResponse);

        // Act
        var result = await _service.CreateAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(request.Name);
        result.ImageUrl.Should().Be(request.ImageUrl);
        result.Price.Should().Be(request.Price);
        _repositoryMock.Verify(r => r.CreateAsync(It.Is<Product>(p =>
            p.Name == request.Name &&
            p.ImageUrl == request.ImageUrl &&
            p.Id != Guid.Empty &&
            p.CreatedAt != DateTimeOffset.MinValue
        )), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_SetsIdAndCreatedAt()
    {
        // Arrange
        var request = TestDataGenerator.GenerateCreateProductRequest();
        Product capturedProduct = null!;

        _mapperMock.Setup(m => m.Map<Product>(request))
            .Returns(new Product());
        _repositoryMock.Setup(r => r.CreateAsync(It.IsAny<Product>()))
            .Callback<Product>(p => capturedProduct = p)
            .ReturnsAsync((Product p) => p);
        _mapperMock.Setup(m => m.Map<ProductResponse>(It.IsAny<Product>()))
            .Returns(new ProductResponse());

        // Act
        await _service.CreateAsync(request);

        // Assert
        capturedProduct.Should().NotBeNull();
        capturedProduct.Id.Should().NotBe(Guid.Empty);
        capturedProduct.CreatedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1));
    }

    #endregion

    #region UpdateStockAsync Tests

    [Fact]
    public async Task UpdateStockAsync_WhenProductExists_UpdatesStockSuccessfully()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var product = TestDataGenerator.GenerateProduct(productId);
        var request = TestDataGenerator.GenerateUpdateStockRequest(150);
        var updatedProductResponse = TestDataGenerator.GenerateProductResponse(productId);
        updatedProductResponse.StockQuantity = 150;

        _repositoryMock.Setup(r => r.GetByIdAsync(productId))
            .ReturnsAsync(product);
        _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Product>()))
            .ReturnsAsync((Product p) => p);
        _mapperMock.Setup(m => m.Map<ProductResponse>(It.IsAny<Product>()))
            .Returns(updatedProductResponse);

        // Act
        var result = await _service.UpdateStockAsync(productId, request);

        // Assert
        result.Should().NotBeNull();
        result.StockQuantity.Should().Be(150);
        _repositoryMock.Verify(r => r.GetByIdAsync(productId), Times.Once);
        _repositoryMock.Verify(r => r.UpdateAsync(It.Is<Product>(p =>
            p.Id == productId &&
            p.StockQuantity == 150 &&
            p.UpdatedAt != DateTimeOffset.MinValue
        )), Times.Once);
    }

    [Fact]
    public async Task UpdateStockAsync_WhenProductDoesNotExist_ThrowsNotFoundException()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var request = TestDataGenerator.GenerateUpdateStockRequest(100);

        _repositoryMock.Setup(r => r.GetByIdAsync(nonExistentId))
            .ReturnsAsync((Product?)null);

        // Act
        Func<Task> act = async () => await _service.UpdateStockAsync(nonExistentId, request);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Product with ID {nonExistentId} was not found");
        _repositoryMock.Verify(r => r.GetByIdAsync(nonExistentId), Times.Once);
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Product>()), Times.Never);
    }

    [Fact]
    public async Task UpdateStockAsync_SetsUpdatedAt()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var product = TestDataGenerator.GenerateProduct(productId);
        var request = TestDataGenerator.GenerateUpdateStockRequest(200);
        Product capturedProduct = null!;

        _repositoryMock.Setup(r => r.GetByIdAsync(productId))
            .ReturnsAsync(product);
        _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Product>()))
            .Callback<Product>(p => capturedProduct = p)
            .ReturnsAsync((Product p) => p);
        _mapperMock.Setup(m => m.Map<ProductResponse>(It.IsAny<Product>()))
            .Returns(new ProductResponse());

        // Act
        await _service.UpdateStockAsync(productId, request);

        // Assert
        capturedProduct.Should().NotBeNull();
        capturedProduct.UpdatedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1));
        capturedProduct.StockQuantity.Should().Be(200);
    }

    [Fact]
    public async Task UpdateStockAsync_WithZeroQuantity_UpdatesSuccessfully()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var product = TestDataGenerator.GenerateProduct(productId);
        var request = TestDataGenerator.GenerateUpdateStockRequest(0);

        _repositoryMock.Setup(r => r.GetByIdAsync(productId))
            .ReturnsAsync(product);
        _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Product>()))
            .ReturnsAsync((Product p) => p);
        _mapperMock.Setup(m => m.Map<ProductResponse>(It.IsAny<Product>()))
            .Returns(new ProductResponse { StockQuantity = 0 });

        // Act
        var result = await _service.UpdateStockAsync(productId, request);

        // Assert
        result.Should().NotBeNull();
        result.StockQuantity.Should().Be(0);
        _repositoryMock.Verify(r => r.UpdateAsync(It.Is<Product>(p => p.StockQuantity == 0)), Times.Once);
    }

    #endregion
}
