using Alza.EShop.Application.DTOs.Requests;
using Alza.EShop.Application.DTOs.Responses;
using Alza.EShop.Application.Services.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Alza.EShop.API.Controllers.V1;

/// <summary>
/// Controller for product management operations (Version 1).
/// </summary>
/// <remarks>
/// Initializes a new instance of the ProductsController class.
/// </remarks>
/// <param name="productService">The product service.</param>
/// <param name="createValidator">The create product request validator.</param>
/// <param name="updateStockValidator">The update stock request validator.</param>
[ApiController]
[Route("api/v{version:apiVersion}/products")]
[ApiVersion("1.0")]
[Produces("application/json")]
[EnableRateLimiting("fixed")]
public class ProductsController(
    IProductService productService,
    IValidator<CreateProductRequest> createValidator,
    IValidator<UpdateStockRequest> updateStockValidator) : ControllerBase
{
    private readonly IProductService _productService = productService;
    private readonly IValidator<CreateProductRequest> _createValidator = createValidator;
    private readonly IValidator<UpdateStockRequest> _updateStockValidator = updateStockValidator;

    /// <summary>
    /// Gets list of all products.
    /// </summary>
    /// <returns>A list of all products.</returns>
    /// <response code="200">Returns the list of products.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProductResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ProductResponse>>> GetAll()
    {
        var products = await _productService.GetAllAsync();
        return Ok(products);
    }

    /// <summary>
    /// Gets a specific product by ID.
    /// </summary>
    /// <param name="id">The product identifier.</param>
    /// <returns>The product if found.</returns>
    /// <response code="200">Returns the requested product.</response>
    /// <response code="404">If the product is not found.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductResponse>> GetById(Guid id)
    {
        var product = await _productService.GetByIdAsync(id);
        return Ok(product);
    }

    /// <summary>
    /// Creates a new product.
    /// </summary>
    /// <param name="request">The product creation request.</param>
    /// <returns>The created product.</returns>
    /// <response code="201">Returns the newly created product.</response>
    /// <response code="400">If the request is invalid.</response>
    /// <remarks>
    /// Sample request:
    /// 
    ///     POST /api/v1/products
    ///     {
    ///         "name": "Wireless Mouse",
    ///         "imageUrl": "https://example.com/images/mouse.jpg",
    ///         "price": 29.99,
    ///         "description": "Ergonomic wireless mouse with precision tracking",
    ///         "stockQuantity": 100
    ///     }
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProductResponse>> Create([FromBody] CreateProductRequest request)
    {
        var validationResult = await _createValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            throw new Application.Common.Exceptions.ValidationException(validationResult.Errors);
        }

        var product = await _productService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    /// <summary>
    /// Updates the stock quantity of a product.
    /// </summary>
    /// <param name="id">The product identifier.</param>
    /// <param name="request">The stock update request.</param>
    /// <returns>The updated product.</returns>
    /// <response code="200">Returns the updated product.</response>
    /// <response code="400">If the request is invalid.</response>
    /// <response code="404">If the product is not found.</response>
    /// <remarks>
    /// Sample request:
    /// 
    ///     PATCH /api/v1/products/{id}/stock
    ///     {
    ///         "stockQuantity": 50
    ///     }
    /// </remarks>
    [HttpPatch("{id}/stock")]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductResponse>> UpdateStock(Guid id, [FromBody] UpdateStockRequest request)
    {
        var validationResult = await _updateStockValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            throw new Application.Common.Exceptions.ValidationException(validationResult.Errors);
        }

        var product = await _productService.UpdateStockAsync(id, request);
        return Ok(product);
    }

    /// <summary>
    /// Deletes a product by its identifier.
    /// </summary>
    /// <param name="id">The product identifier.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <response code="204">Indicates that the product was successfully deleted.</response>
    /// <response code="404">If the product is not found.</response>
    /// <remarks>
    /// Sample request:
    /// 
    ///     DELETE /api/v1/products/{id}
    /// </remarks>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _productService.DeleteAsync(id);
        return NoContent();
    }
}
