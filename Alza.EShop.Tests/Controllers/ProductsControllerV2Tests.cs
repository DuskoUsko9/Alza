using Alza.EShop.Application.Common.Models;
using Alza.EShop.Application.DTOs.Responses;
using Alza.EShop.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Alza.EShop.Tests.Controllers;

/// <summary>
/// Unit tests for V2 ProductsController.
/// </summary>
public class ProductsControllerV2Tests
{
    private readonly Mock<IProductService> _mockProductService;
    private readonly API.Controllers.V2.ProductsController _controller;

    public ProductsControllerV2Tests()
    {
        _mockProductService = new Mock<IProductService>();

        _controller = new API.Controllers.V2.ProductsController(
            _mockProductService.Object);
    }

    [Fact]
    public async Task GetPaged_WithValidParameters_ReturnsOkWithPaginatedResult()
    {
        // Arrange
        var products = new List<ProductResponse>
        {
            new() { 
                Id = Guid.NewGuid(), 
                Name = "Product 1", 
                ImageUrl = "https://example.com/1.jpg",
                CreatedAt = DateTimeOffset.UtcNow 
            },
            new() { 
                Id = Guid.NewGuid(), 
                Name = "Product 2", 
                ImageUrl = "https://example.com/2.jpg",
                CreatedAt = DateTimeOffset.UtcNow 
            }
        };

        var paginatedResult = new PaginatedResult<ProductResponse>(products, 25, 1, 10);

        _mockProductService
            .Setup(s => s.GetPagedAsync(It.IsAny<PaginationParameters>()))
            .ReturnsAsync(paginatedResult);

        // Act
        var result = await _controller.GetPaged(1, 10);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedResult = Assert.IsType<PaginatedResult<ProductResponse>>(okResult.Value);
        Assert.Equal(2, returnedResult.Items.Count());
        Assert.Equal(25, returnedResult.TotalCount);
        Assert.Equal(3, returnedResult.TotalPages);
        Assert.True(returnedResult.HasNext);
        Assert.False(returnedResult.HasPrevious);
    }

    [Fact]
    public async Task GetPaged_WithDefaultParameters_UsesDefaults()
    {
        // Arrange
        var paginatedResult = new PaginatedResult<ProductResponse>(
            [], 0, 1, 10);

        _mockProductService
            .Setup(s => s.GetPagedAsync(It.Is<PaginationParameters>(p => 
                p.PageNumber == 1 && p.PageSize == 10)))
            .ReturnsAsync(paginatedResult);

        // Act
        var result = await _controller.GetPaged();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        _mockProductService.Verify(s => s.GetPagedAsync(
            It.Is<PaginationParameters>(p => p.PageNumber == 1 && p.PageSize == 10)), 
            Times.Once);
    }

    [Fact]
    public async Task GetPaged_SecondPage_HasCorrectPaginationMetadata()
    {
        // Arrange
        var products = new List<ProductResponse>
        {
            new() { 
                Id = Guid.NewGuid(), 
                Name = "Product 11", 
                ImageUrl = "https://example.com/11.jpg",
                CreatedAt = DateTimeOffset.UtcNow 
            }
        };

        var paginatedResult = new PaginatedResult<ProductResponse>(products, 25, 2, 10);

        _mockProductService
            .Setup(s => s.GetPagedAsync(It.IsAny<PaginationParameters>()))
            .ReturnsAsync(paginatedResult);

        // Act
        var result = await _controller.GetPaged(2, 10);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedResult = Assert.IsType<PaginatedResult<ProductResponse>>(okResult.Value);
        Assert.Equal(2, returnedResult.PageNumber);
        Assert.Equal(3, returnedResult.TotalPages);
        Assert.True(returnedResult.HasPrevious);
        Assert.True(returnedResult.HasNext);
    }
}
