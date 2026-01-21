using Alza.EShop.Application.DTOs.Requests;
using Alza.EShop.Application.Validators;
using FluentAssertions;

namespace Alza.EShop.Tests.Validators;

/// <summary>
/// Unit tests for CreateProductRequestValidator.
/// Tests all validation rules for product creation.
/// </summary>
public class CreateProductRequestValidatorTests
{
    private readonly CreateProductRequestValidator _validator;

    public CreateProductRequestValidatorTests()
    {
        _validator = new CreateProductRequestValidator();
    }

    #region Name Validation Tests

    [Fact]
    public async Task Validate_ValidRequest_PassesValidation()
    {
        // Arrange
        var request = TestDataGenerator.GenerateCreateProductRequest();

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task Validate_EmptyName_FailsValidation()
    {
        // Arrange
        var request = new CreateProductRequest
        {
            Name = "",
            ImageUrl = "https://example.com/product.jpg",
            Price = 99.99m
        };

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name" && e.ErrorMessage == "Name is required");
    }

    [Fact]
    public async Task Validate_NullName_FailsValidation()
    {
        // Arrange
        var request = new CreateProductRequest
        {
            Name = null!,
            ImageUrl = "https://example.com/product.jpg",
            Price = 99.99m
        };

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Fact]
    public async Task Validate_NameExceeds200Characters_FailsValidation()
    {
        // Arrange
        var request = new CreateProductRequest
        {
            Name = new string('A', 201),
            ImageUrl = "https://example.com/product.jpg",
            Price = 99.99m
        };

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name" && e.ErrorMessage == "Name cannot exceed 200 characters");
    }

    [Fact]
    public async Task Validate_NameExactly200Characters_PassesValidation()
    {
        // Arrange
        var request = new CreateProductRequest
        {
            Name = new string('A', 200),
            ImageUrl = "https://example.com/product.jpg",
            Price = 99.99m
        };

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    #endregion

    #region ImageUrl Validation Tests

    [Fact]
    public async Task Validate_EmptyImageUrl_FailsValidation()
    {
        // Arrange
        var request = new CreateProductRequest
        {
            Name = "Test Product",
            ImageUrl = "",
            Price = 99.99m
        };

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "ImageUrl" && e.ErrorMessage == "ImageUrl is required");
    }

    [Fact]
    public async Task Validate_InvalidUrlFormat_FailsValidation()
    {
        // Arrange
        var request = new CreateProductRequest
        {
            Name = "Test Product",
            ImageUrl = "not-a-valid-url",
            Price = 99.99m
        };

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "ImageUrl" && e.ErrorMessage == "ImageUrl must be a valid URL");
    }

    [Fact]
    public async Task Validate_HttpUrl_PassesValidation()
    {
        // Arrange
        var request = new CreateProductRequest
        {
            Name = "Test Product",
            ImageUrl = "http://example.com/product.jpg",
            Price = 99.99m
        };

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_HttpsUrl_PassesValidation()
    {
        // Arrange
        var request = new CreateProductRequest
        {
            Name = "Test Product",
            ImageUrl = "https://example.com/product.jpg",
            Price = 99.99m
        };

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_ImageUrlExceeds500Characters_FailsValidation()
    {
        // Arrange
        var request = new CreateProductRequest
        {
            Name = "Test Product",
            ImageUrl = "https://example.com/" + new string('a', 500),
            Price = 99.99m
        };

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "ImageUrl" && e.ErrorMessage == "ImageUrl cannot exceed 500 characters");
    }

    #endregion

    #region Price Validation Tests

    [Fact]
    public async Task Validate_NullPrice_PassesValidation()
    {
        // Arrange
        var request = new CreateProductRequest
        {
            Name = "Test Product",
            ImageUrl = "https://example.com/product.jpg",
            Price = null
        };

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_ZeroPrice_PassesValidation()
    {
        // Arrange
        var request = new CreateProductRequest
        {
            Name = "Test Product",
            ImageUrl = "https://example.com/product.jpg",
            Price = 0m
        };

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_NegativePrice_FailsValidation()
    {
        // Arrange
        var request = new CreateProductRequest
        {
            Name = "Test Product",
            ImageUrl = "https://example.com/product.jpg",
            Price = -10m
        };

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Price" && e.ErrorMessage == "Price must be greater than or equal to 0");
    }

    [Fact]
    public async Task Validate_PositivePrice_PassesValidation()
    {
        // Arrange
        var request = new CreateProductRequest
        {
            Name = "Test Product",
            ImageUrl = "https://example.com/product.jpg",
            Price = 99.99m
        };

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    #endregion

    #region Description Validation Tests

    [Fact]
    public async Task Validate_NullDescription_PassesValidation()
    {
        // Arrange
        var request = new CreateProductRequest
        {
            Name = "Test Product",
            ImageUrl = "https://example.com/product.jpg",
            Description = null
        };

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_EmptyDescription_PassesValidation()
    {
        // Arrange
        var request = new CreateProductRequest
        {
            Name = "Test Product",
            ImageUrl = "https://example.com/product.jpg",
            Description = ""
        };

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_DescriptionExceeds2000Characters_FailsValidation()
    {
        // Arrange
        var request = new CreateProductRequest
        {
            Name = "Test Product",
            ImageUrl = "https://example.com/product.jpg",
            Description = new string('A', 2001)
        };

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Description" && e.ErrorMessage == "Description cannot exceed 2000 characters");
    }

    [Fact]
    public async Task Validate_DescriptionExactly2000Characters_PassesValidation()
    {
        // Arrange
        var request = new CreateProductRequest
        {
            Name = "Test Product",
            ImageUrl = "https://example.com/product.jpg",
            Description = new string('A', 2000)
        };

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    #endregion

    #region StockQuantity Validation Tests

    [Fact]
    public async Task Validate_NullStockQuantity_PassesValidation()
    {
        // Arrange
        var request = new CreateProductRequest
        {
            Name = "Test Product",
            ImageUrl = "https://example.com/product.jpg",
            StockQuantity = null
        };

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_ZeroStockQuantity_PassesValidation()
    {
        // Arrange
        var request = new CreateProductRequest
        {
            Name = "Test Product",
            ImageUrl = "https://example.com/product.jpg",
            StockQuantity = 0
        };

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_NegativeStockQuantity_FailsValidation()
    {
        // Arrange
        var request = new CreateProductRequest
        {
            Name = "Test Product",
            ImageUrl = "https://example.com/product.jpg",
            StockQuantity = -5
        };

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "StockQuantity" && e.ErrorMessage == "StockQuantity must be greater than or equal to 0");
    }

    [Fact]
    public async Task Validate_PositiveStockQuantity_PassesValidation()
    {
        // Arrange
        var request = new CreateProductRequest
        {
            Name = "Test Product",
            ImageUrl = "https://example.com/product.jpg",
            StockQuantity = 100
        };

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    #endregion

    #region Multiple Errors Tests

    [Fact]
    public async Task Validate_MultipleErrors_ReturnsAllErrors()
    {
        // Arrange
        var request = new CreateProductRequest
        {
            Name = "",
            ImageUrl = "invalid-url",
            Price = -10m,
            StockQuantity = -5
        };

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCountGreaterThanOrEqualTo(4);
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
        result.Errors.Should().Contain(e => e.PropertyName == "ImageUrl");
        result.Errors.Should().Contain(e => e.PropertyName == "Price");
        result.Errors.Should().Contain(e => e.PropertyName == "StockQuantity");
    }

    #endregion
}
