// <copyright file="Errors.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using SharedKernel.Abstractions;
using SharedKernel.Models.Common;

namespace Users.Application.Adapters;

/// <summary>
/// Represents an error when a resource was not found.
/// </summary>
/// <param name="Id">The identifier of the not found resource.</param>
public sealed record NotFoundError(Guid Id) : IError
{
    /// <inheritdoc/>
    public string Message => $"Resource with ID '{Id}' was not found.";

    /// <inheritdoc/>
    public ErrorType Type => ErrorType.NotFound;
}

/// <summary>
/// Represents a validation error.
/// </summary>
public sealed record EmptyUserName : IError
{
    /// <inheritdoc/>
    public string Message => "Username cannot be empty or whitespace.";

    /// <inheritdoc/>
    public ErrorType Type => ErrorType.Validation;
}

/// <summary>
/// Represents an internal error.
/// </summary>
/// <param name="Message">The error message.</param>
public sealed record InternalError(string Message) : IError
{
    /// <inheritdoc/>
    public ErrorType Type => ErrorType.Internal;
}

/// <summary>
/// Represents a database error.
/// </summary>
/// <param name="Exception">The underlying exception.</param>
public sealed record DatabaseError(Exception Exception) : IError
{
    /// <inheritdoc/>
    public string Message => $"A database error occurred: {Exception.Message}";

    /// <inheritdoc/>
    public ErrorType Type => ErrorType.Internal;
}

/// <summary>
/// Factory for creating application-level errors from domain errors.
/// </summary>
internal static class ApplicationErrorFactory
{
    private const string UnknownErrorMessage = "An unknown error occurred.";

    /// <summary>
    /// Creates an application error from a domain error.
    /// </summary>
    /// <param name="error">The domain error to convert.</param>
    /// <returns>An <see cref="IError"/> representing the application-level error.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="error"/> is null.</exception>
    public static IError FromDomainError(DomainError error)
    {
        ArgumentNullException.ThrowIfNull(error);

        return error switch
        {
            UserNotFound err => new NotFoundError(err.UserId),
            SharedKernel.Models.Common.EmptyUserName => new EmptyUserName(),
            DatabaseError<Exception> err => new DatabaseError(err.Exception),
            _ => new InternalError(UnknownErrorMessage),
        };
    }

    /// <summary>
    /// Creates an unexpected internal error.
    /// </summary>
    /// <returns>An <see cref="IError"/> representing an unexpected internal error.</returns>
    public static IError UnexpectedError() => new InternalError(UnknownErrorMessage);
}
