namespace Err.Result.Tests;

public class ResultTests
{
    [Fact]
    public void Ok_IsSuccess_AndNoError()
    {
        var result = Result.Ok();

        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.Null(result.Error);
    }

    [Fact]
    public void Fail_IsFailure_AndCarriesError()
    {
        var error = Error.NotFound("User.NotFound", "User 42 does not exist.");
        var result = Result.Fail(error);

        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Equal(error, result.Error);
    }

    [Fact]
    public void Error_ImplicitlyConverts_ToFailedResult()
    {
        Result result = Error.Validation("Email.Invalid", "Email format is invalid.");

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.Validation, result.Error!.Type);
    }

    [Fact]
    public void Combine_ReturnsFirstFailure()
    {
        var first = Error.Failure("E1", "first");
        var second = Error.Failure("E2", "second");

        var combined = Result.Combine(Result.Ok(), Result.Fail(first), Result.Fail(second));

        Assert.True(combined.IsFailure);
        Assert.Equal("E1", combined.Error!.Code);
    }

    [Fact]
    public void Combine_AllSuccess_IsSuccess()
    {
        var combined = Result.Combine(Result.Ok(), Result.Ok(), Result.Ok());

        Assert.True(combined.IsSuccess);
    }

    [Fact]
    public void Match_InvokesCorrectBranch()
    {
        var ok = Result.Ok();
        var fail = Result.Fail(Error.Failure("X", "boom"));

        Assert.Equal("ok", ok.Match(() => "ok", _ => "fail"));
        Assert.Equal("fail", fail.Match(() => "ok", _ => "fail"));
    }

    [Fact]
    public void Constructor_RejectsSuccessWithError()
    {
        Assert.Throws<ArgumentNullException>(() => Result.Fail(null!));
    }

    [Fact]
    public void ErrorFactories_SetExpectedTypes()
    {
        Assert.Equal(ErrorType.Failure, Error.Failure("c", "m").Type);
        Assert.Equal(ErrorType.Validation, Error.Validation("c", "m").Type);
        Assert.Equal(ErrorType.NotFound, Error.NotFound("c", "m").Type);
        Assert.Equal(ErrorType.Conflict, Error.Conflict("c", "m").Type);
        Assert.Equal(ErrorType.Unauthorized, Error.Unauthorized("c", "m").Type);
        Assert.Equal(ErrorType.Forbidden, Error.Forbidden("c", "m").Type);
        Assert.Equal(ErrorType.Unexpected, Error.Unexpected("c", "m").Type);
    }
}
