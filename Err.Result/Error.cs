namespace Err.Result;

/// <summary>
/// Immutable description of a failure: a machine-readable <see cref="Code"/>,
/// a human-readable <see cref="Message"/> and an <see cref="ErrorType"/>.
/// </summary>
public sealed record Error(string Code, string Message, ErrorType Type = ErrorType.Failure)
{
    /// <summary>Creates a generic failure error.</summary>
    public static Error Failure(string code, string message) =>
        new(code, message, ErrorType.Failure);

    /// <summary>Creates a validation error.</summary>
    public static Error Validation(string code, string message) =>
        new(code, message, ErrorType.Validation);

    /// <summary>Creates a not-found error.</summary>
    public static Error NotFound(string code, string message) =>
        new(code, message, ErrorType.NotFound);

    /// <summary>Creates a conflict error.</summary>
    public static Error Conflict(string code, string message) =>
        new(code, message, ErrorType.Conflict);

    /// <summary>Creates an unauthorized error.</summary>
    public static Error Unauthorized(string code, string message) =>
        new(code, message, ErrorType.Unauthorized);

    /// <summary>Creates a forbidden error.</summary>
    public static Error Forbidden(string code, string message) =>
        new(code, message, ErrorType.Forbidden);

    /// <summary>Creates an unexpected error.</summary>
    public static Error Unexpected(string code, string message) =>
        new(code, message, ErrorType.Unexpected);
}
