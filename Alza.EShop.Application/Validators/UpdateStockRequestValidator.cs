using Alza.EShop.Application.DTOs.Requests;
using FluentValidation;

namespace Alza.EShop.Application.Validators;

/// <summary>
/// Validator for UpdateStockRequest.
/// </summary>
public class UpdateStockRequestValidator : AbstractValidator<UpdateStockRequest>
{
    public UpdateStockRequestValidator()
    {
        RuleFor(x => x.StockQuantity)
            .NotNull().WithMessage("StockQuantity is required")
            .GreaterThanOrEqualTo(0).WithMessage("StockQuantity must be greater than or equal to 0");
    }
}
