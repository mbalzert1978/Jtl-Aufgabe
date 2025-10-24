// <copyright file="DomainError.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Diagnostics;

namespace Users.Domain;

/// <summary>
/// Represents domain-specific errors in the Users bounded context.
/// </summary>
/// <remarks>
/// This is a discriminated union implemented as an abstract record.
/// Each variant represents a specific error condition in the domain.
/// Use the factory methods to create instances.
/// </remarks>
public abstract record DomainError;

/// <summary> For dev purposes only. Will be removed later. </summary>
/// <param name="Message">The error message describing the error condition.</param>
public sealed record Generic(string Message) : DomainError;

/// <summary>
/// Factory class for creating <see cref="DomainError"/> instances.
/// </summary>
public static class DomainErrorFactory
{
    /// <summary> For dev purposes only. Will be removed later. </summary>
    /// <param name="message">The error message describing the error condition.</param>
    public static DomainError Generic(string message)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(message);

        Generic err = new(message);
        Debug.Assert(err.Message == message, "Error message does not match");

        return err;
    }
}
