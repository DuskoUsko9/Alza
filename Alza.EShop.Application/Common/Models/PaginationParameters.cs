namespace Alza.EShop.Application.Common.Models;

/// <summary>
/// Represents pagination parameters for API requests.
/// </summary>
public class PaginationParameters
{
    private const int MaxPageSizeValue = 100;
    private int _pageSize = 10;

    /// <summary>
    /// Gets or sets the page number (1-based). Default is 1.
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Gets or sets the number of items per page. Default is 10, maximum is 100.
    /// </summary>
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > MaxPageSizeValue ? MaxPageSizeValue : value;
    }

    /// <summary>
    /// Gets the maximum allowed page size.
    /// </summary>
    public static int MaxPageSize => MaxPageSizeValue;

    /// <summary>
    /// Validates the pagination parameters.
    /// </summary>
    /// <returns>True if the parameters are valid; otherwise, false.</returns>
    public bool IsValid()
    {
        return PageNumber >= 1 && PageSize >= 1 && PageSize <= MaxPageSizeValue;
    }
}
