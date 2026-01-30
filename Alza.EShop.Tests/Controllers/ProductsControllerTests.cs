using Alza.EShop.API.Controllers.V1;
using Alza.EShop.Application.Common.Exceptions;
using Alza.EShop.Application.DTOs.Requests;
using Alza.EShop.Application.DTOs.Responses;
using Alza.EShop.Application.Services.Interfaces;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Alza.EShop.Tests.Controllers;

/// <summary>
/// Unit tests for ProductsController V1.
/// Tests HTTP responses and interactions with service layer.
/// </summary>
public class ProductsControllerTests
{
    private readonly Mock<IProductService> _serviceMock;
    private readonly Mock<IValidator<CreateProductRequest>> _createValidatorMock;
    private readonly Mock<IValidator<UpdateStockRequest>> _updateStockValidatorMock;
    private readonly ProductsController _controller;

    public ProductsControllerTests()
    {
        _serviceMock = new Mock<IProductService>();
        _createValidatorMock = new Mock<IValidator<CreateProductRequest>>();
        _updateStockValidatorMock = new Mock<IValidator<UpdateStockRequest>>();
        _controller = new ProductsController(
            _serviceMock.Object,
            _createValidatorMock.Object,
            _updateStockValidatorMock.Object);
    }

    #region GetAll Tests

    [Fact]
    public async Task GetAll_ReturnsOkWithProducts()
    {
        // Arrange
        var products = TestDataGenerator.GenerateProductResponses(5);
        _serviceMock.Setup(s => s.GetAllAsync())
            .ReturnsAsync(products);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedProducts = okResult.Value.Should().BeAssignableTo<IEnumerable<ProductResponse>>().Subject;
        returnedProducts.Should().HaveCount(5);
        _serviceMock.Verify(s => s.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAll_WhenNoProducts_ReturnsOkWithEmptyList()
    {
        // Arrange
        var emptyList = new List<ProductResponse>();
        _serviceMock.Setup(s => s.GetAllAsync())
            .ReturnsAsync(emptyList);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedProducts = okResult.Value.Should().BeAssignableTo<IEnumerable<ProductResponse>>().Subject;
        returnedProducts.Should().BeEmpty();
    }

    #endregion

    #region GetById Tests

    [Fact]
    public async Task GetById_WhenProductExists_ReturnsOkWithProduct()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var product = TestDataGenerator.GenerateProductResponse(productId);
        _serviceMock.Setup(s => s.GetByIdAsync(productId))
            .ReturnsAsync(product);

        // Act
        var result = await _controller.GetById(productId);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedProduct = okResult.Value.Should().BeOfType<ProductResponse>().Subject;
        returnedProduct.Id.Should().Be(productId);
        returnedProduct.Name.Should().Be(product.Name);
        _serviceMock.Verify(s => s.GetByIdAsync(productId), Times.Once);
    }

    [Fact]
    public async Task GetById_WhenProductDoesNotExist_ThrowsNotFoundException()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        _serviceMock.Setup(s => s.GetByIdAsync(nonExistentId))
            .ThrowsAsync(new NotFoundException("Product", nonExistentId));

        // Act
        Func<Task> act = async () => await _controller.GetById(nonExistentId);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
        _serviceMock.Verify(s => s.GetByIdAsync(nonExistentId), Times.Once);
    }

    #endregion

    #region Create Tests

    [Fact]
    public async Task Create_ValidRequest_ReturnsCreatedAtActionWithProduct()
    {
        // Arrange
        var request = TestDataGenerator.GenerateCreateProductRequest();
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

        _createValidatorMock.Setup(v => v.ValidateAsync(request, default))
            .ReturnsAsync(new ValidationResult());
        _serviceMock.Setup(s => s.CreateAsync(request))
            .ReturnsAsync(productResponse);

        // Act
        var result = await _controller.Create(request);

        // Assert
        var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdResult.ActionName.Should().Be(nameof(_controller.GetById));
        createdResult.RouteValues.Should().ContainKey("id");
        createdResult.RouteValues!["id"].Should().Be(productResponse.Id);
        
        var returnedProduct = createdResult.Value.Should().BeOfType<ProductResponse>().Subject;
        returnedProduct.Id.Should().Be(productResponse.Id);
        returnedProduct.Name.Should().Be(request.Name);
        
        _createValidatorMock.Verify(v => v.ValidateAsync(request, default), Times.Once);
        _serviceMock.Verify(s => s.CreateAsync(request), Times.Once);
    }

    [Fact]
    public async Task Create_InvalidRequest_ThrowsValidationException()
    {
        // Arrange
        var request = TestDataGenerator.GenerateInvalidCreateProductRequest();
        var validationFailures = new List<ValidationFailure>
        {
            new ValidationFailure("Name", "Name is required"),
            new ValidationFailure("ImageUrl", "ImageUrl is required")
        };

        _createValidatorMock.Setup(v => v.ValidateAsync(request, default))
            .ReturnsAsync(new ValidationResult(validationFailures));

        // Act
        Func<Task> act = async () => await _controller.Create(request);

        // Assert
        await act.Should().ThrowAsync<Application.Common.Exceptions.ValidationException>();
        _createValidatorMock.Verify(v => v.ValidateAsync(request, default), Times.Once);
        _serviceMock.Verify(s => s.CreateAsync(It.IsAny<CreateProductRequest>()), Times.Never);
    }

    [Fact]
    public async Task Create_WhenValidationFails_DoesNotCallService()
    {
        // Arrange
        var request = TestDataGenerator.GenerateCreateProductRequestWithLongName();
        var validationFailures = new List<ValidationFailure>
        {
            new ValidationFailure("Name", "Name cannot exceed 200 characters")
        };

        _createValidatorMock.Setup(v => v.ValidateAsync(request, default))
            .ReturnsAsync(new ValidationResult(validationFailures));

        // Act
        Func<Task> act = async () => await _controller.Create(request);

        // Assert
        await act.Should().ThrowAsync<Application.Common.Exceptions.ValidationException>();
        _serviceMock.Verify(s => s.CreateAsync(It.IsAny<CreateProductRequest>()), Times.Never);
    }

    #endregion

    #region UpdateStock Tests

    [Fact]
    public async Task UpdateStock_ValidRequest_ReturnsOkWithUpdatedProduct()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var request = TestDataGenerator.GenerateUpdateStockRequest(100);
        var updatedProduct = TestDataGenerator.GenerateProductResponse(productId);
        updatedProduct.StockQuantity = 100;

        _updateStockValidatorMock.Setup(v => v.ValidateAsync(request, default))
            .ReturnsAsync(new ValidationResult());
        _serviceMock.Setup(s => s.UpdateStockAsync(productId, request))
            .ReturnsAsync(updatedProduct);

        // Act
        var result = await _controller.UpdateStock(productId, request);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedProduct = okResult.Value.Should().BeOfType<ProductResponse>().Subject;
        returnedProduct.Id.Should().Be(productId);
        returnedProduct.StockQuantity.Should().Be(100);
        
        _updateStockValidatorMock.Verify(v => v.ValidateAsync(request, default), Times.Once);
        _serviceMock.Verify(s => s.UpdateStockAsync(productId, request), Times.Once);
    }

    [Fact]
    public async Task UpdateStock_InvalidRequest_ThrowsValidationException()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var request = TestDataGenerator.GenerateInvalidUpdateStockRequest();
        var validationFailures = new List<ValidationFailure>
        {
            new ValidationFailure("StockQuantity", "Stock quantity must be >= 0")
        };

        _updateStockValidatorMock.Setup(v => v.ValidateAsync(request, default))
            .ReturnsAsync(new ValidationResult(validationFailures));

        // Act
        Func<Task> act = async () => await _controller.UpdateStock(productId, request);

        // Assert
        await act.Should().ThrowAsync<Application.Common.Exceptions.ValidationException>();
        _updateStockValidatorMock.Verify(v => v.ValidateAsync(request, default), Times.Once);
        _serviceMock.Verify(s => s.UpdateStockAsync(It.IsAny<Guid>(), It.IsAny<UpdateStockRequest>()), Times.Never);
    }

    [Fact]
    public async Task UpdateStock_WhenProductDoesNotExist_ThrowsNotFoundException()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var request = TestDataGenerator.GenerateUpdateStockRequest(50);

        _updateStockValidatorMock.Setup(v => v.ValidateAsync(request, default))
            .ReturnsAsync(new ValidationResult());
        _serviceMock.Setup(s => s.UpdateStockAsync(nonExistentId, request))
            .ThrowsAsync(new NotFoundException("Product", nonExistentId));

        // Act
        Func<Task> act = async () => await _controller.UpdateStock(nonExistentId, request);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
        _serviceMock.Verify(s => s.UpdateStockAsync(nonExistentId, request), Times.Once);
    }

    [Fact]
    public async Task UpdateStock_WithZeroQuantity_UpdatesSuccessfully()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var request = TestDataGenerator.GenerateUpdateStockRequest(0);
        var updatedProduct = TestDataGenerator.GenerateProductResponse(productId);
        updatedProduct.StockQuantity = 0;

        _updateStockValidatorMock.Setup(v => v.ValidateAsync(request, default))
            .ReturnsAsync(new ValidationResult());
        _serviceMock.Setup(s => s.UpdateStockAsync(productId, request))
            .ReturnsAsync(updatedProduct);

        // Act
        var result = await _controller.UpdateStock(productId, request);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedProduct = okResult.Value.Should().BeOfType<ProductResponse>().Subject;
        returnedProduct.StockQuantity.Should().Be(0);
        _serviceMock.Verify(s => s.UpdateStockAsync(productId, request), Times.Once);
    }

    #endregion

    #region Delete Tests

    [Fact]
    public async Task Delete_WhenProductExists_ReturnsNoContent()
    {
        // Arrange
        var productId = Guid.NewGuid();
        _serviceMock.Setup(s => s.DeleteAsync(productId))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Delete(productId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _serviceMock.Verify(s => s.DeleteAsync(productId), Times.Once);
    }

    [Fact]
    public async Task Delete_WhenProductDoesNotExist_ThrowsNotFoundException()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        _serviceMock.Setup(s => s.DeleteAsync(nonExistentId))
            .ThrowsAsync(new NotFoundException("Product", nonExistentId));

        // Act
        Func<Task> act = async () => await _controller.Delete(nonExistentId);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>();
        _serviceMock.Verify(s => s.DeleteAsync(nonExistentId), Times.Once);
    }

    #endregion
}
