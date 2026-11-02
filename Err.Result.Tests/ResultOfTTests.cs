namespace Err.Result.Tests;

public class ResultOfTTests
{
    [Fact]
    public void Value_ImplicitlyConverts_ToSuccessfulResult()
    {
        Result<int> result = 42;

        Assert.True(result.IsSuccess);
        Assert.Equal(42, result.Value);
    }

    [Fact]
    public void Error_ImplicitlyConverts_ToFailedResult()
    {
        Result<int> result = Error.NotFound("N", "nope");

        Assert.True(result.IsFailure);
        Assert.Equal("N", result.Error!.Code);
    }

    [Fact]
    public void Value_OnFailure_Throws()
    {
        Result<int> result = Error.Failure("X", "boom");

        Assert.Throws<InvalidOperationException>(() => result.Value);
    }

    [Fact]
    public void ValueOr_ReturnsFallbackOnFailure()
    {
        Result<int> result = Error.Failure("X", "boom");

        Assert.Equal(-1, result.ValueOr(-1));
        Assert.Equal(0, result.ValueOr(_ => 0));
    }

    [Fact]
    public void Map_TransformsSuccess_PassesFailureThrough()
    {
        Result<int> ok = 2;
        Result<int> fail = Error.Failure("X", "boom");

        Assert.Equal(4, ok.Map(x => x * 2).Value);
        Assert.Equal("X", fail.Map(x => x * 2).Error!.Code);
    }

    [Fact]
    public void Bind_ChainsResults_AndShortCircuits()
    {
        static Result<int> Parse(string s) =>
            int.TryParse(s, out var n) ? n : Error.Validation("Parse", "not a number");

        Assert.Equal(6, Parse("3").Bind(x => Result.Ok(x * 2)).Value);
        Assert.Equal("Parse", Parse("abc").Bind(x => Result.Ok(x * 2)).Error!.Code);
    }

    [Fact]
    public void Ensure_FailsWhenPredicateIsFalse()
    {
        Result<int> result = 3;

        var ensured = result.Ensure(x => x > 5, Error.Validation("TooSmall", "must be > 5"));

        Assert.True(ensured.IsFailure);
        Assert.Equal("TooSmall", ensured.Error!.Code);
    }

    [Fact]
    public void Ensure_KeepsSuccessWhenPredicateIsTrue()
    {
        Result<int> result = 10;

        Assert.Equal(10, result.Ensure(x => x > 5, Error.Validation("TooSmall", "must be > 5")).Value);
    }

    [Fact]
    public void Tap_RunsSideEffectOnlyOnSuccess()
    {
        var seen = 0;
        Result<int> ok = 5;
        Result<int> fail = Error.Failure("X", "boom");

        ok.Tap(x => seen = x);
        Assert.Equal(5, seen);

        fail.Tap(x => seen = x);
        Assert.Equal(5, seen);
    }

    [Fact]
    public void TapError_RunsSideEffectOnlyOnFailure()
    {
        Error? seen = null;
        Result<int> fail = Error.Failure("X", "boom");

        fail.TapError(e => seen = e);

        Assert.Equal("X", seen!.Code);
    }

    [Fact]
    public async Task AsyncChaining_WorksAcrossTasks()
    {
        static async Task<Result<int>> LoadAsync(int id)
        {
            await Task.Delay(1);
            return id > 0 ? id : Error.NotFound("Missing", "bad id");
        }

        var value = await LoadAsync(2)
            .Map(x => x * 10)
            .Bind(x => LoadAsync(x + 1));

        Assert.Equal(21, value.Value);

        var failed = await LoadAsync(-1).Map(x => x * 10);
        Assert.Equal("Missing", failed.Error!.Code);
    }

    [Fact]
    public void Match_OnGenericResult_InvokesCorrectBranch()
    {
        Result<string> ok = "hello";
        Result<string> fail = Error.Failure("X", "boom");

        Assert.Equal("HELLO", ok.Match(s => s.ToUpperInvariant(), e => e.Code));
        Assert.Equal("X", fail.Match(s => s.ToUpperInvariant(), e => e.Code));
    }

    [Fact]
    public void ToResult_WrapsValuesAndErrors()
    {
        Assert.Equal(7, 7.ToResult().Value);
        Assert.True(Error.Failure("X", "m").ToResult<int>().IsFailure);
    }
}
