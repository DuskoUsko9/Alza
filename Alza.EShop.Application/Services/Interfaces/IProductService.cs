using Alza.EShop.Application.Common.Models;
using Alza.EShop.Application.DTOs.Requests;
using Alza.EShop.Application.DTOs.Responses;

namespace Alza.EShop.Application.Services.Interfaces;

/// <summary>
/// Service interface for product operations.
/// </summary>
public interface IProductService
{
    /// <summary>
    /// Gets all products.
    /// </summary>
    /// <returns>A collection of all products.</returns>
    Task<IEnumerable<ProductResponse>> GetAllAsync();

    /// <summary>
    /// Gets a paginated list of products (V2 API).
    /// </summary>
    /// <param name="parameters">The pagination parameters.</param>
    /// <returns>A paginated result containing products and pagination metadata.</returns>
    Task<PaginatedResult<ProductResponse>> GetPagedAsync(PaginationParameters parameters);

    /// <summary>
    /// Gets a product by its identifier.
    /// </summary>
    /// <param name="id">The product identifier.</param>
    /// <returns>The product if found.</returns>
    /// <exception cref="Common.Exceptions.NotFoundException">Thrown when the product is not found.</exception>
    Task<ProductResponse> GetByIdAsync(Guid id);

    /// <summary>
    /// Creates a new product.
    /// </summary>
    /// <param name="request">The product creation request.</param>
    /// <returns>The created product.</returns>
    Task<ProductResponse> CreateAsync(CreateProductRequest request);

    /// <summary>
    /// Updates the stock quantity of a product synchronously (V1 API).
    /// </summary>
    /// <param name="id">The product identifier.</param>
    /// <param name="request">The stock update request.</param>
    /// <returns>The updated product.</returns>
    /// <exception cref="Common.Exceptions.NotFoundException">Thrown when the product is not found.</exception>
    Task<ProductResponse> UpdateStockAsync(Guid id, UpdateStockRequest request);

    /// <summary>
    /// Deletes a product by its identifier.
    /// </summary>
    /// <param name="id">The product identifier.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteAsync(Guid id);
}
