# Result Monad - LLM Quick Reference

## Purpose

Railway-oriented programming pattern for explicit error handling without exceptions. Represents operations that can succeed (`Ok<T>`) or fail (`Err<E>`).

## Core Types

### Result<T, E>

Abstract base type representing either success or error state.

- **T**: Success value type (must be non-null)
- **E**: Error value type (must be non-null)

### Ok<T, E>

Success case containing value of type `T`.

### Err<T, E>

Error case containing error of type `E`.

### Unit

Represents "void" for operations with no meaningful return value. Singleton type `()`.

## Location

- **Core**: `src/SharedKernel/Models/Results/Result{T, E}.cs`
- **Unit Type**: `src/SharedKernel/Models/Unit.cs`
- **Extensions**: `src/SharedKernel/Extensions/Results/`
  - Sync: `Map`, `Bind`, `Match`, `MapErr`, `OrElse`, `Or`, `Flatten`
  - Async: Task and ValueTask variants of all operations

## Common Patterns

### Creating Results

```csharp
// Success - use factory method
Result<User, Error> success = ResultFactory.Success<User, Error>(user);

// Error - use factory method
Result<User, Error> failure = ResultFactory.Failure<User, Error>(error);
```

### Checking State

```csharp
bool isSuccess = result.IsOk;
bool isError = result.IsErrAnd(e => e.Code == 404);
```

### Pattern Matching

```csharp
// Match both cases
var output = result.Match(
    ok: user => $"User: {user.Name}",
    err: error => $"Error: {error.Message}"
);
```

### Transforming Success Values (Map)

```csharp
// Transform value if Ok, preserve error if Err
Result<string, Error> userName = userResult.Map(u => u.Name);
```

### Chaining Operations (Bind/FlatMap)

```csharp
// Chain operations that return Result
Result<Order, Error> result = userResult
    .Bind(user => GetOrder(user.Id))
    .Bind(order => ValidateOrder(order));
```

### Transforming Errors (MapErr)

```csharp
// Convert domain error to API error
Result<User, ApiError> apiResult = domainResult
    .MapErr(domainErr => new ApiError(domainErr));
```

### Providing Fallbacks (OrElse)

```csharp
// Try alternative if first fails
Result<User, Error> result = primaryLookup
    .OrElse(() => secondaryLookup);
```

### Unwrapping with Default (Or)

```csharp
// Get value or default
User user = userResult.Or(User.Anonymous);
```

## Usage Rules for This Project

### Domain Layer

- Return `Result<T, DomainError>` from all business logic operations
- Use factory methods (e.g., `User.Create()`) that return Results
- NO format validation - only business rules

### Application Layer  

- Handlers return `Result<TResponse, Error>`
- Chain domain operations using `Bind`
- Transform domain errors if needed using `MapErr`

### Infrastructure Layer

- Repositories can return `Result<T, Error>` for operations that may fail
- Wrap external failures in Result

### Presentation Layer (WebApi)

- Use `Match` to convert Results to HTTP responses
- Map to appropriate status codes: Ok→200/201, Err→400/404/etc
- Format validation happens HERE, not in domain

## Async Support

All operations have async variants:

- `Task<Result<T, E>>` → Use `MapTaskExtension`, `BindTaskExtension`, etc.
- `ValueTask<Result<T, E>>` → Use `MapValueTaskExtension`, `BindValueTaskExtension`, etc.

## Key Benefits

1. **Explicit errors**: No hidden exceptions
2. **Composable**: Chain operations with Bind
3. **Type-safe**: Compiler enforces error handling
4. **Railway-oriented**: Success path vs error path clearly separated
5. **Async-friendly**: First-class support for async operations

## Anti-Patterns to Avoid

- ❌ Don't throw exceptions from Result-returning methods
- ❌ Don't use Result for null - use nullable types
- ❌ Don't ignore error cases - always Match or handle
- ❌ Don't mix Result with try-catch in same code path
