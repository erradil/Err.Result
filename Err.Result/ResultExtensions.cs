namespace Err.Result;

/// <summary>
/// Railway-oriented extensions to compose <see cref="Result"/> and
/// <see cref="Result{TValue}"/> in a fluent, LINQ-free style.
/// </summary>
public static class ResultExtensions
{
    /// <summary>Wraps a value into a successful result.</summary>
    public static Result<TValue> ToResult<TValue>(this TValue value) => Result.Ok(value);

    /// <summary>Converts an error into a failed result.</summary>
    public static Result<TValue> ToResult<TValue>(this Error error) => Result.Fail<TValue>(error);

    /// <summary>Converts an error into a failed result.</summary>
    public static Result ToResult(this Error error) => Result.Fail(error);

    /// <summary>
    /// Transforms the value of a successful result; failures pass through unchanged.
    /// </summary>
    public static Result<TOut> Map<TIn, TOut>(this Result<TIn> result, Func<TIn, TOut> mapper) =>
        result.IsSuccess ? Result.Ok(mapper(result.Value)) : Result.Fail<TOut>(result.Error!);

    /// <summary>
    /// Chains an operation that itself returns a result; failures short-circuit.
    /// </summary>
    public static Result<TOut> Bind<TIn, TOut>(this Result<TIn> result, Func<TIn, Result<TOut>> binder) =>
        result.IsSuccess ? binder(result.Value) : Result.Fail<TOut>(result.Error!);

    /// <summary>
    /// Chains an operation that returns a value-less result; failures short-circuit.
    /// </summary>
    public static Result Bind<TIn>(this Result<TIn> result, Func<TIn, Result> binder) =>
        result.IsSuccess ? binder(result.Value) : Result.Fail(result.Error!);

    /// <summary>
    /// Ensures the value satisfies <paramref name="predicate"/>, otherwise fails with <paramref name="error"/>.
    /// </summary>
    public static Result<TValue> Ensure<TValue>(this Result<TValue> result, Func<TValue, bool> predicate, Error error) =>
        result.IsFailure
            ? result
            : predicate(result.Value) ? result : Result.Fail<TValue>(error);

    /// <summary>Runs a side effect on success and returns the result unchanged.</summary>
    public static Result<TValue> Tap<TValue>(this Result<TValue> result, Action<TValue> action)
    {
        if (result.IsSuccess) action(result.Value);
        return result;
    }

    /// <summary>Runs a side effect on failure and returns the result unchanged.</summary>
    public static Result<TValue> TapError<TValue>(this Result<TValue> result, Action<Error> action)
    {
        if (result.IsFailure) action(result.Error!);
        return result;
    }

    /// <summary>Runs a side effect on success and returns the result unchanged.</summary>
    public static Result Tap(this Result result, Action action)
    {
        if (result.IsSuccess) action();
        return result;
    }

    /// <summary>Runs a side effect on failure and returns the result unchanged.</summary>
    public static Result TapError(this Result result, Action<Error> action)
    {
        if (result.IsFailure) action(result.Error!);
        return result;
    }

    // ---- Async (Task) overloads ----

    /// <inheritdoc cref="Map{TIn,TOut}(Result{TIn},Func{TIn,TOut})"/>
    public static async Task<Result<TOut>> Map<TIn, TOut>(this Task<Result<TIn>> resultTask, Func<TIn, TOut> mapper) =>
        (await resultTask.ConfigureAwait(false)).Map(mapper);

    /// <summary>Async variant of <see cref="Map{TIn,TOut}(Result{TIn},Func{TIn,TOut})"/>.</summary>
    public static async Task<Result<TOut>> Map<TIn, TOut>(this Task<Result<TIn>> resultTask, Func<TIn, Task<TOut>> mapper)
    {
        var result = await resultTask.ConfigureAwait(false);
        return result.IsSuccess
            ? Result.Ok(await mapper(result.Value).ConfigureAwait(false))
            : Result.Fail<TOut>(result.Error!);
    }

    /// <inheritdoc cref="Bind{TIn,TOut}(Result{TIn},Func{TIn,Result{TOut}})"/>
    public static async Task<Result<TOut>> Bind<TIn, TOut>(this Task<Result<TIn>> resultTask, Func<TIn, Task<Result<TOut>>> binder)
    {
        var result = await resultTask.ConfigureAwait(false);
        return result.IsSuccess
            ? await binder(result.Value).ConfigureAwait(false)
            : Result.Fail<TOut>(result.Error!);
    }

    /// <inheritdoc cref="Ensure{TValue}(Result{TValue},Func{TValue,bool},Error)"/>
    public static async Task<Result<TValue>> Ensure<TValue>(this Task<Result<TValue>> resultTask, Func<TValue, bool> predicate, Error error) =>
        (await resultTask.ConfigureAwait(false)).Ensure(predicate, error);

    /// <inheritdoc cref="Tap{TValue}(Result{TValue},Action{TValue})"/>
    public static async Task<Result<TValue>> Tap<TValue>(this Task<Result<TValue>> resultTask, Action<TValue> action) =>
        (await resultTask.ConfigureAwait(false)).Tap(action);

    /// <inheritdoc cref="Result{TValue}.Match{TResult}(Func{TValue,TResult},Func{Error,TResult})"/>
    public static async Task<TResult> Match<TValue, TResult>(
        this Task<Result<TValue>> resultTask,
        Func<TValue, TResult> onSuccess,
        Func<Error, TResult> onFailure) =>
        (await resultTask.ConfigureAwait(false)).Match(onSuccess, onFailure);
}
