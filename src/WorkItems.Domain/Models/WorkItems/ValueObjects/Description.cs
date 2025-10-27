// <copyright file="TaskDescription.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Diagnostics;
using Monads.Results;
using SharedKernel.Models.Common;
using static Monads.Results.ResultFactory;

namespace WorkItems.Domain.Models.WorkItems.ValueObjects;

/// <summary>
/// Represents a task description.
/// </summary>
internal sealed record Description(string Value);

/// <summary>
/// Factory for creating <see cref="Description"/> instances.
/// </summary>
internal static class DescriptionFactory
{
    /// <summary>
    /// Creates a new <see cref="Description"/> instance.
    /// </summary>
    /// <param name="value">The description value.</param>
    /// <returns>A result containing the task description or a domain error.</returns>
    public static Result<Description, DomainError> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            DomainError error = DomainErrorFactory.Validation(
                nameof(Description),
                "Description cannot be null, empty, or whitespace."
            );
            return Failure<Description, DomainError>(error);
        }

        if (value.Length > 4096)
        {
            DomainError error = DomainErrorFactory.Validation(
                nameof(Description),
                "Description cannot exceed 4096 characters."
            );
            return Failure<Description, DomainError>(error);
        }

        string trimmedValue = value.Trim();
        Debug.Assert(
            !string.IsNullOrWhiteSpace(trimmedValue),
            "Trimmed description must not be empty."
        );

        Description description = new(trimmedValue);
        return Success<Description, DomainError>(description);
    }

    /// <summary>
    /// Rehydrates a <see cref="Description"/> instance from a stored value.
    /// </summary>
    /// <param name="value">The stored description value.</param>
    /// <exception cref="ArgumentException">Thrown when the provided value is null or whitespace.</exception>
    /// <returns>The rehydrated <see cref="Description"/> instance.</returns>
    public static Description Rehydrate(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        return new(value);
    }
}
