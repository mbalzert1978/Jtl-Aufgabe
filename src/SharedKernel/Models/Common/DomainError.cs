// <copyright file="DomainError.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

namespace SharedKernel.Models.Common;

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
/// <param name="Id">The ID of the user that was not found.</param>
/// <param name="Type">The entity that was not found.</param>
internal sealed record NotFound(Guid Id, Type Type) : DomainError;

/// <summary>
/// Represents a validation error.
/// </summary>
internal sealed record Validation(string Field, string Detail) : DomainError;

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
    /// Creates a <see cref="DomainError"/> representing a user not found error.
    /// </summary>
    /// <param name="id">The ID of the user that was not found.</param>
    /// <param name="type">The type of the entity that was not found.</param>
    public static DomainError NotFound(Guid id, Type type) => new NotFound(id, type);

    /// <summary>
    /// Creates a <see cref="DomainError"/> representing a validation error.
    /// </summary>
    /// <param name="Field">The name of the field that failed validation.</param>
    /// <param name="Detail">The detail of the validation error.</param>
    public static DomainError Validation(string Field, string Detail) =>
        new Validation(Field, Detail);

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
}
