namespace Alza.EShop.Application.DTOs.Responses;

/// <summary>
/// Response model containing product information.
/// </summary>
public class ProductResponse
{
    /// <summary>
    /// Unique product identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Product name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// URL to the product image.
    /// </summary>
    public string ImageUrl { get; set; } = string.Empty;

    /// <summary>
    /// Product price.
    /// </summary>
    public decimal? Price { get; set; }

    /// <summary>
    /// Product description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Available stock quantity.
    /// </summary>
    public int? StockQuantity { get; set; }

    /// <summary>
    /// Date and time when the product was created (UTC).
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// Date and time when the product was last updated (UTC).
    /// </summary>
    public DateTimeOffset? UpdatedAt { get; set; }
}
