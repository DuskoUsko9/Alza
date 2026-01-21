namespace Alza.EShop.Application.DTOs.Requests;

/// <summary>
/// Request model for creating a new product.
/// </summary>
public class CreateProductRequest
{
    /// <summary>
    /// Product name. Required, maximum 200 characters.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// URL to the product image. Required, maximum 500 characters.
    /// </summary>
    public string ImageUrl { get; set; } = string.Empty;

    /// <summary>
    /// Product price. Optional, must be greater than or equal to 0 if provided.
    /// </summary>
    public decimal? Price { get; set; }

    /// <summary>
    /// Product description. Optional, maximum 2000 characters.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Available stock quantity. Optional, must be greater than or equal to 0 if provided.
    /// </summary>
    public int? StockQuantity { get; set; }
}
