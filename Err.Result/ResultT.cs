namespace Err.Result;

/// <summary>
/// Represents the outcome of an operation: either a <typeparamref name="TValue"/>
/// on success, or an <see cref="Err.Result.Error"/> on failure.
/// </summary>
/// <typeparam name="TValue">The type of the value carried on success.</typeparam>
public sealed class Result<TValue> : Result
{
    private readonly TValue? _value;

    internal Result(TValue value) : base(true, null) => _value = value;

    internal Result(Error error) : base(false, error) { }

    /// <summary>
    /// The value on success.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when accessed on a failed result.</exception>
    public TValue Value =>
        IsSuccess
            ? _value!
            : throw new InvalidOperationException(
                $"Cannot access the value of a failed result. Error: {Error!.Code} - {Error.Message}");

    /// <summary>The value on success, otherwise <c>default</c>.</summary>
    public TValue? ValueOrDefault => IsSuccess ? _value : default;

    /// <summary>The value on success, otherwise <paramref name="fallback"/>.</summary>
    public TValue ValueOr(TValue fallback) => IsSuccess ? _value! : fallback;

    /// <summary>The value on success, otherwise the result of <paramref name="fallback"/>.</summary>
    public TValue ValueOr(Func<Error, TValue> fallback) => IsSuccess ? _value! : fallback(Error!);

    /// <summary>A value implicitly converts to a successful result.</summary>
    public static implicit operator Result<TValue>(TValue value) => new(value);

    /// <summary>An error implicitly converts to a failed result.</summary>
    public static implicit operator Result<TValue>(Error error) => new(error);

    /// <summary>Projects the result into <typeparamref name="TResult"/>.</summary>
    public TResult Match<TResult>(Func<TValue, TResult> onSuccess, Func<Error, TResult> onFailure) =>
        IsSuccess ? onSuccess(_value!) : onFailure(Error!);

    /// <summary>Executes <paramref name="onSuccess"/> or <paramref name="onFailure"/>.</summary>
    public void Switch(Action<TValue> onSuccess, Action<Error> onFailure)
    {
        if (IsSuccess) onSuccess(_value!);
        else onFailure(Error!);
    }
}
