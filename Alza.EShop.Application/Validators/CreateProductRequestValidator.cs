using Alza.EShop.Application.DTOs.Requests;
using FluentValidation;

namespace Alza.EShop.Application.Validators;

/// <summary>
/// Validator for CreateProductRequest.
/// </summary>
public class CreateProductRequestValidator : AbstractValidator<CreateProductRequest>
{
    public CreateProductRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(200).WithMessage("Name cannot exceed 200 characters");

        RuleFor(x => x.ImageUrl)
            .NotEmpty().WithMessage("ImageUrl is required")
            .MaximumLength(500).WithMessage("ImageUrl cannot exceed 500 characters")
            .Must(BeAValidUrl).WithMessage("ImageUrl must be a valid URL");

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0)
                .When(x => x.Price.HasValue)
                .WithMessage("Price must be greater than or equal to 0");

        RuleFor(x => x.Description)
            .MaximumLength(2000)
                .When(x => !string.IsNullOrEmpty(x.Description))
                .WithMessage("Description cannot exceed 2000 characters");

        RuleFor(x => x.StockQuantity)
            .GreaterThanOrEqualTo(0)
                .When(x => x.StockQuantity.HasValue)
                .WithMessage("StockQuantity must be greater than or equal to 0");
    }

    private bool BeAValidUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
            && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }
}
