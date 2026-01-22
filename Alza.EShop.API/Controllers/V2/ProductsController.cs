using Alza.EShop.Application.Common.Models;
using Alza.EShop.Application.DTOs.Requests;
using Alza.EShop.Application.DTOs.Responses;
using Alza.EShop.Application.Services.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Alza.EShop.API.Controllers.V2;

/// <summary>
/// Controller for product management operations (Version 2).
/// Adds pagination support for efficient product listing.
/// </summary>
/// <remarks>
/// Initializes a new instance of the ProductsController class.
/// </remarks>
/// <param name="productService">The product service.</param>
/// <param name="createValidator">The create product request validator.</param>
[ApiController]
[Route("api/v{version:apiVersion}/products")]
[ApiVersion("2.0")]
[Produces("application/json")]
[EnableRateLimiting("fixed")]
public class ProductsController(
    IProductService productService) : ControllerBase
{
    private readonly IProductService _productService = productService;

    /// <summary>
    /// Gets a paginated list of products.
    /// </summary>
    /// <param name="pageNumber">The page number (default: 1, minimum: 1).</param>
    /// <param name="pageSize">The number of items per page (default: 10, range: 1-100).</param>
    /// <returns>A paginated result containing products and pagination metadata.</returns>
    /// <response code="200">Returns the paginated list of products.</response>
    /// <response code="400">If pagination parameters are invalid.</response>
    /// <remarks>
    /// Sample request:
    /// 
    ///     GET /api/v2/products?pageNumber=1&amp;pageSize=10
    /// 
    /// </remarks>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResult<ProductResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PaginatedResult<ProductResponse>>> GetPaged(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var parameters = new PaginationParameters
        {
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await _productService.GetPagedAsync(parameters);
        return Ok(result);
    }
}
