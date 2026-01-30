using Alza.EShop.Application.Common.Exceptions;
using Alza.EShop.Application.Common.Models;
using Alza.EShop.Application.DTOs.Requests;
using Alza.EShop.Application.DTOs.Responses;
using Alza.EShop.Application.Services.Interfaces;
using Alza.EShop.Domain.Entities;
using Alza.EShop.Domain.Interfaces;
using AutoMapper;

namespace Alza.EShop.Application.Services.Implementations;

/// <summary>
/// Service implementation for product operations.
/// </summary>
/// <remarks>
/// Initializes a new instance of the ProductService class.
/// </remarks>
/// <param name="repository">The product repository.</param>
/// <param name="mapper">The AutoMapper instance.</param>
public class ProductService(
    IProductRepository repository,
    IMapper mapper) : IProductService
{
    private readonly IProductRepository _repository = repository;
    private readonly IMapper _mapper = mapper;

    /// <inheritdoc/>
    public async Task<IEnumerable<ProductResponse>> GetAllAsync()
    {
        var products = await _repository.GetAllAsync();
        return _mapper.Map<IEnumerable<ProductResponse>>(products);
    }

    /// <inheritdoc/>
    public async Task<PaginatedResult<ProductResponse>> GetPagedAsync(PaginationParameters parameters)
    {
        // Validate pagination parameters
        if (!parameters.IsValid())
        {
            throw new ArgumentException("Invalid pagination parameters", nameof(parameters));
        }

        var (items, totalCount) = await _repository.GetPagedAsync(parameters.PageNumber, parameters.PageSize);
        
        var productResponses = _mapper.Map<IEnumerable<ProductResponse>>(items);

        return new PaginatedResult<ProductResponse>(
            productResponses,
            totalCount,
            parameters.PageNumber,
            parameters.PageSize);
    }

    /// <inheritdoc/>
    public async Task<ProductResponse> GetByIdAsync(Guid id)
    {
        var product = await _repository.GetByIdAsync(id) ?? throw new NotFoundException("Product", id);
        return _mapper.Map<ProductResponse>(product);
    }

    /// <inheritdoc/>
    public async Task<ProductResponse> CreateAsync(CreateProductRequest request)
    {
        var product = _mapper.Map<Product>(request);
        product.Id = Guid.NewGuid();
        product.CreatedAt = DateTimeOffset.UtcNow;

        var created = await _repository.CreateAsync(product);
        return _mapper.Map<ProductResponse>(created);
    }

    /// <inheritdoc/>
    public async Task<ProductResponse> UpdateStockAsync(Guid id, UpdateStockRequest request)
    {
        var product = await _repository.GetByIdAsync(id) ?? throw new NotFoundException("Product", id);
        product.StockQuantity = request.StockQuantity;
        product.UpdatedAt = DateTimeOffset.UtcNow;

        var updated = await _repository.UpdateAsync(product);
        return _mapper.Map<ProductResponse>(updated);
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(Guid id)
    {
        _ = await _repository.GetByIdAsync(id) ?? throw new NotFoundException("Product", id);
        await _repository.DeleteAsync(id);
    }
}
