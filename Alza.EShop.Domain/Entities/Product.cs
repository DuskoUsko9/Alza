using Alza.EShop.Domain.Common;

namespace Alza.EShop.Domain.Entities;

/// <summary>
/// Basic product entity, inherits from BaseEntity
/// </summary>
public class Product : BaseEntity
{
    public string Name { get; set; } = string.Empty; // default to empty string to avoid nulls
    public string ImageUrl { get; set; } = string.Empty;
    public decimal? Price { get; set; }
    public string? Description { get; set; }
    public int? StockQuantity { get; set; }
}
