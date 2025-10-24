// <copyright file="DomainError.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Diagnostics;
using SharedKernel.Abstractions;

namespace Users.Domain;

/// <summary>
/// Represents domain-specific errors in the Users bounded context.
/// </summary>
/// <remarks>
/// This is a discriminated union implemented as an abstract record.
/// Each variant represents a specific error condition in the domain.
/// Use the <see cref="DomainErrorFactory"/> to create instances.
/// </remarks>
internal abstract record DomainError;

/// <summary>
/// Represents an error when a user with the specified ID was not found.
/// </summary>
/// <param name="UserId">The ID of the user that was not found.</param>
internal sealed record UserNotFound(Guid UserId) : DomainError;

/// <summary>
/// Represents a validation error when a username is empty or whitespace.
/// </summary>
internal sealed record EmptyUserName : DomainError;

/// <summary>
/// Represents a database error that occurred during an operation.
/// </summary>
/// <typeparam name="TException">The type of exception that occurred.</typeparam>
/// <param name="Exception">The exception that was thrown.</param>
internal sealed record DatabaseError<TException>(TException Exception) : DomainError
    where TException : Exception;

/// <summary>
/// Factory for creating <see cref="DomainError"/> instances.
/// </summary>
internal static class DomainErrorFactory
{
    /// <summary>
    /// Creates a <see cref="DomainError"/> representing an empty username validation error.
    /// </summary>
    /// <returns>A <see cref="Domain.EmptyUserName"/> instance.</returns>
    public static DomainError EmptyUserName() => new EmptyUserName();

    /// <summary>
    /// Creates a <see cref="DomainError"/> representing a database error.
    /// </summary>
    /// <param name="exception">The exception that occurred during the database operation.</param>
    /// <returns>A <see cref="DatabaseError{TException}"/> instance.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="exception"/> is null.</exception>
    public static DomainError DatabaseError<TException>(TException exception)
        where TException : Exception
    {
        ArgumentNullException.ThrowIfNull(exception);

        return new DatabaseError<TException>(exception);
    }

    /// <summary>
    /// Creates a <see cref="DomainError"/> representing a user not found error.
    /// </summary>
    /// <param name="userId">The ID of the user that was not found.</param>
    /// <returns>A <see cref="Domain.UserNotFound"/> instance.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="userId"/> is empty.</exception>
    public static DomainError UserNotFound(Guid userId) => new UserNotFound(userId);
}
