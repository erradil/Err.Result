namespace Err.Result;

/// <summary>
/// Represents the outcome of an operation that returns no value:
/// either success, or failure with an <see cref="Err.Result.Error"/>.
/// </summary>
public class Result
{
    /// <summary>Initializes a new result.</summary>
    protected Result(bool isSuccess, Error? error)
    {
        if (isSuccess && error is not null)
            throw new ArgumentException("A successful result cannot carry an error.", nameof(error));
        if (!isSuccess && error is null)
            throw new ArgumentNullException(nameof(error), "A failed result must carry an error.");

        IsSuccess = isSuccess;
        Error = error;
    }

    /// <summary>Whether the operation succeeded.</summary>
    public bool IsSuccess { get; }

    /// <summary>Whether the operation failed.</summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>The error when failed; <c>null</c> when successful.</summary>
    public Error? Error { get; }

    /// <summary>Creates a successful result.</summary>
    public static Result Ok() => new(true, null);

    /// <summary>Creates a failed result.</summary>
    public static Result Fail(Error error) => new(false, error);

    /// <summary>Creates a successful result carrying <paramref name="value"/>.</summary>
    public static Result<TValue> Ok<TValue>(TValue value) => new(value);

    /// <summary>Creates a failed result of <typeparamref name="TValue"/>.</summary>
    public static Result<TValue> Fail<TValue>(Error error) => new(error);

    /// <summary>
    /// Combines several results: success only if all succeed,
    /// otherwise the first failure.
    /// </summary>
    public static Result Combine(params Result[] results)
    {
        foreach (var result in results)
        {
            if (result.IsFailure)
                return Fail(result.Error!);
        }
        return Ok();
    }

    /// <summary>An error implicitly converts to a failed result.</summary>
    public static implicit operator Result(Error error) => Fail(error);

    /// <summary>Executes <paramref name="onSuccess"/> or <paramref name="onFailure"/> and returns its value.</summary>
    public TResult Match<TResult>(Func<TResult> onSuccess, Func<Error, TResult> onFailure) =>
        IsSuccess ? onSuccess() : onFailure(Error!);

    /// <summary>Executes <paramref name="onSuccess"/> or <paramref name="onFailure"/>.</summary>
    public void Switch(Action onSuccess, Action<Error> onFailure)
    {
        if (IsSuccess) onSuccess();
        else onFailure(Error!);
    }
}
