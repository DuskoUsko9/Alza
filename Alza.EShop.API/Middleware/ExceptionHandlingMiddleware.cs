using Alza.EShop.Application.Common.Exceptions;
using Alza.EShop.Application.DTOs.Responses;
using System.Net;
using System.Text.Json;

namespace Alza.EShop.API.Middleware;

/// <summary>
/// Middleware for global exception handling.
/// </summary>
/// <remarks>
/// Initializes a new instance of the ExceptionHandlingMiddleware class.
/// </remarks>
/// <param name="next">The next middleware in the pipeline.</param>
/// <param name="logger">The logger instance.</param>
public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger = logger;

    /// <summary>
    /// Invokes the middleware.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = exception switch
        {
            NotFoundException notFoundEx => new ApiErrorResponse
            {
                StatusCode = (int)HttpStatusCode.NotFound,
                Message = notFoundEx.Message,
                Errors = null
            },
            ValidationException validationEx => new ApiErrorResponse
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Message = "One or more validation errors occurred",
                Errors = validationEx.Errors
            },
            _ => new ApiErrorResponse
            {
                StatusCode = (int)HttpStatusCode.InternalServerError,
                Message = "An error occurred while processing your request",
                Errors = null
            }
        };

        context.Response.StatusCode = response.StatusCode;

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, jsonOptions));
    }
}
