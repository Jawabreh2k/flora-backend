namespace Flora.Cms.Application.Common.Exceptions;

/// <summary>
/// Thrown when input fails validation. Carries a property-keyed map of error
/// messages so the API can render an RFC-7807 validation problem (HTTP 400) that
/// matches the previous <c>ModelState</c> responses.
/// </summary>
public sealed class ValidationException : Exception
{
    public ValidationException(IReadOnlyDictionary<string, string[]> errors)
        : base("One or more validation errors occurred.")
        => Errors = errors;

    /// <summary>Single-message convenience for business-rule failures not tied to a field.</summary>
    public ValidationException(string message)
        : base(message)
        => Errors = new Dictionary<string, string[]>();

    public IReadOnlyDictionary<string, string[]> Errors { get; }
}

/// <summary>Thrown when a requested resource does not exist. Maps to HTTP 404.</summary>
public sealed class NotFoundException(string message) : Exception(message);

/// <summary>Thrown when a request conflicts with the current state. Maps to HTTP 409.</summary>
public sealed class ConflictException(string message) : Exception(message);
