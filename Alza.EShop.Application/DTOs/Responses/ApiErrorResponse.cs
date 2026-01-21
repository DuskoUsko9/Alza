namespace Alza.EShop.Application.DTOs.Responses;

/// <summary>
/// Standard error response model for API errors.
/// </summary>
public class ApiErrorResponse
{
    /// <summary>
    /// HTTP status code.
    /// </summary>
    public int StatusCode { get; set; }

    /// <summary>
    /// Error message.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Detailed validation errors (field name to error messages).
    /// </summary>
    public IDictionary<string, string[]>? Errors { get; set; }
}
