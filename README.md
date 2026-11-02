# Err.Result

A lightweight, dependency-free **Result pattern** library for .NET.

Express success/failure without exceptions, compose operations fluently with
`Map` / `Bind` / `Ensure` / `Tap`, and match on outcomes. Async overloads included.

[![NuGet](https://img.shields.io/nuget/v/Err.Result.svg)](https://www.nuget.org/packages/Err.Result)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

## Installation

```bash
dotnet add package Err.Result
```

## Quick start

```csharp
using Err.Result;

// A method that returns Result<T> instead of throwing
Result<User> GetUser(int id)
{
    if (id <= 0)
        return Error.Validation("User.InvalidId", "Id must be positive.");

    var user = repository.Find(id);
    return user is null
        ? Error.NotFound("User.NotFound", $"No user with id {id}.")
        : user; // implicit conversion to Result<User>
}

// Compose operations — failures short-circuit automatically
Result<string> result = GetUser(42)
    .Ensure(u => u.IsActive, Error.Forbidden("User.Inactive", "User is deactivated."))
    .Map(u => u.Email)
    .Tap(email => logger.LogInformation("Found {Email}", email));

// Handle both branches explicitly
string message = result.Match(
    onSuccess: email => $"Welcome, {email}!",
    onFailure: error => $"Error {error.Code}: {error.Message}");
```

## Async pipelines

```csharp
var result = await GetUserAsync(42)
    .Ensure(u => u.IsActive, Error.Forbidden("User.Inactive", "User is deactivated."))
    .Bind(u => SendWelcomeEmailAsync(u.Email))
    .Tap(_ => metrics.Increment("welcome_emails"));
```

## API overview

| Member | Description |
|---|---|
| `Result.Ok()` / `Result.Ok(value)` | Create a success |
| `Result.Fail(error)` / `Result.Fail<T>(error)` | Create a failure |
| `Result.Combine(...)` | Success only if all succeed, else first failure |
| `Error.Failure / Validation / NotFound / Conflict / Unauthorized / Forbidden / Unexpected` | Typed error factories |
| `Map` | Transform the value on success |
| `Bind` | Chain operations returning `Result` |
| `Ensure` | Validate the value against a predicate |
| `Tap` / `TapError` | Side effects without breaking the chain |
| `Match` / `Switch` | Fold both branches into a value or an action |
| `Value` / `ValueOr` / `ValueOrDefault` | Extract the value (safely or not) |

Implicit conversions make returns natural: return a `TValue` for success or an
`Error` for failure — both convert automatically to `Result<TValue>`.

## Targets

- `netstandard2.0` — .NET Framework 4.7.2+, .NET Core 2.0+, and more
- `net8.0` — modern .NET

## License

[MIT](LICENSE)
