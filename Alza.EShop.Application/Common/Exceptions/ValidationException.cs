namespace Alza.EShop.Application.Common.Exceptions;

/// <summary>
/// Exception thrown when validation fails for a request.
/// </summary>
public class ValidationException : Exception
{
    /// <summary>
    /// Gets the validation errors.
    /// </summary>
    public IDictionary<string, string[]> Errors { get; }

    /// <summary>
    /// Initializes a new instance of the ValidationException class.
    /// </summary>
    /// <param name="errors">Dictionary of validation errors (field name to error messages).</param>
    public ValidationException(IDictionary<string, string[]> errors)
        : base("One or more validation errors occurred")
    {
        Errors = errors;
    }

    /// <summary>
    /// Initializes a new instance of the ValidationException class with FluentValidation errors.
    /// </summary>
    /// <param name="failures">FluentValidation failures.</param>
    public ValidationException(IEnumerable<FluentValidation.Results.ValidationFailure> failures)
        : base("One or more validation errors occurred")
    {
        Errors = failures
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.ErrorMessage).ToArray()
            );
    }
}
