// <copyright file="Username.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Diagnostics;
using Monads.Results;
using Users.Domain;
using static Monads.Results.ResultFactory;

namespace Domain.Models.Users.ValueObjects;

/// <summary>
/// Represents a username value object in the domain model.
/// Encapsulates the username validation rules and ensures immutability.
/// </summary>
/// <param name="Value">The validated username string.</param>
/// <remarks>
/// This is a value object following Domain-Driven Design principles.
/// Business logic validation only - format validation is handled at the presentation layer.
/// Use the <see cref="UsernameFactory.Create"/> method to create instances.
/// </remarks>
public sealed record Username(string Value)
{
    /// <summary>
    /// Gets the username string value.
    /// </summary>
    /// <value>
    /// A non-null, non-empty username string.
    /// </value>
    public string Value { get; } = Value ?? throw new ArgumentNullException(nameof(Value));

    /// <inheritdoc/>
    public override string ToString() => Value;
}

/// <summary>
/// Factory class for creating <see cref="Username"/> instances with validation.
/// </summary>
public static class UsernameFactory
{
    private const string EmptyUsernameError = "Username cannot be empty or whitespace";

    /// <summary>
    /// Creates a new <see cref="Username"/> instance after validating the input.
    /// </summary>
    /// <param name="value">The username string to validate and encapsulate.</param>
    /// <returns>
    /// A <see cref="Result{T, E}"/> containing either a valid <see cref="Username"/> or an error message.
    /// </returns>
    /// <remarks>
    /// This method performs business logic validation only.
    /// Format validation (length, allowed characters) should be done at the presentation layer.
    /// </remarks>
    public static Result<Username, DomainError> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            Result<Username, DomainError> err = Failure<Username, DomainError>(
                DomainErrorFactory.Generic(EmptyUsernameError)
            );
            Debug.Assert(err.IsErr, "Expected error result");
            return err;
        }

        Result<Username, DomainError> ok = Success<Username, DomainError>(new(value));
        Debug.Assert(ok.IsOk, "Expected success result");
        return ok;
    }
}
