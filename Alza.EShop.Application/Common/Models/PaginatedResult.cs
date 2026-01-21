namespace Alza.EShop.Application.Common.Models;

/// <summary>
/// Represents a paginated result set containing items and pagination metadata.
/// </summary>
/// <typeparam name="T">The type of items in the result set.</typeparam>
public class PaginatedResult<T>
{
    /// <summary>
    /// Gets or sets the collection of items for the current page.
    /// </summary>
    public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();

    /// <summary>
    /// Gets or sets the current page number (1-based).
    /// </summary>
    public int PageNumber { get; set; }

    /// <summary>
    /// Gets or sets the number of items per page.
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Gets or sets the total number of items across all pages.
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Gets the total number of pages.
    /// </summary>
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

    /// <summary>
    /// Gets a value indicating whether there is a previous page.
    /// </summary>
    public bool HasPrevious => PageNumber > 1;

    /// <summary>
    /// Gets a value indicating whether there is a next page.
    /// </summary>
    public bool HasNext => PageNumber < TotalPages;

    /// <summary>
    /// Initializes a new instance of the <see cref="PaginatedResult{T}"/> class.
    /// </summary>
    public PaginatedResult()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PaginatedResult{T}"/> class with the specified parameters.
    /// </summary>
    /// <param name="items">The items for the current page.</param>
    /// <param name="totalCount">The total number of items across all pages.</param>
    /// <param name="pageNumber">The current page number.</param>
    /// <param name="pageSize">The number of items per page.</param>
    public PaginatedResult(IEnumerable<T> items, int totalCount, int pageNumber, int pageSize)
    {
        Items = items;
        TotalCount = totalCount;
        PageNumber = pageNumber;
        PageSize = pageSize;
    }
}
