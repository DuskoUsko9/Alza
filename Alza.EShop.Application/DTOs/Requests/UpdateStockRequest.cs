namespace Alza.EShop.Application.DTOs.Requests;

/// <summary>
/// Request model for updating product stock quantity.
/// </summary>
public class UpdateStockRequest
{
    /// <summary>
    /// New stock quantity. Required, must be greater than or equal to 0.
    /// </summary>
    public int StockQuantity { get; set; }
}
