namespace UserAPI.Core.Domain.Exceptions;

/// <summary>
/// Base exception for domain-specific errors
/// </summary>
public abstract class DomainException : Exception
{
    protected DomainException(string message) : base(message) { }
    protected DomainException(string message, Exception innerException) : base(message, innerException) { }
}

/// <summary>
/// Thrown when a requested entity is not found
/// </summary>
public class NotFoundException : DomainException
{
    public NotFoundException(string entityName, object key)
        : base($"{entityName} with identifier '{key}' was not found.") { }
}

/// <summary>
/// Thrown when a duplicate entity is detected
/// </summary>
public class DuplicateException : DomainException
{
    public DuplicateException(string entityName, string field, object value)
        : base($"{entityName} with {field} '{value}' already exists.") { }
}

/// <summary>
/// Thrown when authentication fails
/// </summary>
public class AuthenticationException : DomainException
{
    public AuthenticationException(string message = "Invalid email or password.")
        : base(message) { }
}
