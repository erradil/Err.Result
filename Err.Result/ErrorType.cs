namespace Err.Result;

/// <summary>
/// Categorizes an <see cref="Error"/> so callers can react to error kinds
/// without parsing codes or messages.
/// </summary>
public enum ErrorType
{
    /// <summary>A generic, unclassified failure.</summary>
    Failure = 0,

    /// <summary>Input or state failed validation.</summary>
    Validation = 1,

    /// <summary>A requested resource was not found.</summary>
    NotFound = 2,

    /// <summary>The operation conflicts with the current state.</summary>
    Conflict = 3,

    /// <summary>The caller is not authenticated.</summary>
    Unauthorized = 4,

    /// <summary>The caller is authenticated but not allowed.</summary>
    Forbidden = 5,

    /// <summary>An unexpected, usually technical, error.</summary>
    Unexpected = 6
}
