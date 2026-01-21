using Alza.EShop.Application.DTOs.Requests;
using Alza.EShop.Application.Validators;
using FluentAssertions;

namespace Alza.EShop.Tests.Validators;

/// <summary>
/// Unit tests for UpdateStockRequestValidator.
/// Tests validation rules for stock quantity updates.
/// </summary>
public class UpdateStockRequestValidatorTests
{
    private readonly UpdateStockRequestValidator _validator;

    public UpdateStockRequestValidatorTests()
    {
        _validator = new UpdateStockRequestValidator();
    }

    [Fact]
    public async Task Validate_ValidRequest_PassesValidation()
    {
        // Arrange
        var request = new UpdateStockRequest
        {
            StockQuantity = 100
        };

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task Validate_ZeroQuantity_PassesValidation()
    {
        // Arrange
        var request = new UpdateStockRequest
        {
            StockQuantity = 0
        };

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public async Task Validate_PositiveQuantity_PassesValidation()
    {
        // Arrange
        var request = new UpdateStockRequest
        {
            StockQuantity = 500
        };

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_NegativeQuantity_FailsValidation()
    {
        // Arrange
        var request = new UpdateStockRequest
        {
            StockQuantity = -10
        };

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => 
            e.PropertyName == "StockQuantity" && 
            e.ErrorMessage == "StockQuantity must be greater than or equal to 0");
    }

    [Fact]
    public async Task Validate_LargePositiveQuantity_PassesValidation()
    {
        // Arrange
        var request = new UpdateStockRequest
        {
            StockQuantity = 999999
        };

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_MinusOne_FailsValidation()
    {
        // Arrange
        var request = new UpdateStockRequest
        {
            StockQuantity = -1
        };

        // Act
        var result = await _validator.ValidateAsync(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].PropertyName.Should().Be("StockQuantity");
    }
}
