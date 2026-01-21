namespace Alza.EShop.Application.Common.Exceptions;

/// <summary>
/// Exception thrown when a requested resource is not found.
/// </summary>
public class NotFoundException : Exception
{
    /// <summary>
    /// Initializes a new instance of the NotFoundException class.
    /// </summary>
    /// <param name="entityName">The name of the entity that was not found.</param>
    /// <param name="id">The identifier of the entity that was not found.</param>
    public NotFoundException(string entityName, Guid id)
        : base($"{entityName} with ID {id} was not found")
    {
    }

    /// <summary>
    /// Initializes a new instance of the NotFoundException class with a custom message.
    /// </summary>
    /// <param name="message">The error message.</param>
    public NotFoundException(string message)
        : base(message)
    {
    }
}
