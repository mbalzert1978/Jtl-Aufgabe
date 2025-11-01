// <copyright file="Title.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Diagnostics;
using Monads.Results;
using SharedKernel.Models.Common;
using static Monads.Results.ResultFactory;

namespace WorkItems.Domain.Models.WorkItems.ValueObjects;

/// <summary>
/// Represents a task title with validation constraints.
/// </summary>
internal sealed record Title(string Value);

/// <summary>
/// Factory for creating <see cref="Title"/> instances.
/// </summary>
internal static class TitleFactory
{
    /// <summary>
    /// Creates a new <see cref="Title"/> instance.
    /// </summary>
    /// <param name="value">The title value.</param>
    /// <returns>A result containing the task title or a domain error.</returns>
    public static Result<Title, DomainError> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            DomainError error = DomainErrorFactory.Validation(
                nameof(Title),
                "Title cannot be empty or whitespace."
            );
            return Failure<Title, DomainError>(error);
        }

        if (value.Length > 100)
        {
            DomainError error = DomainErrorFactory.Validation(
                nameof(Title),
                "Title cannot exceed 100 characters."
            );
            return Failure<Title, DomainError>(error);
        }

        string trimmedValue = value.Trim();
        Debug.Assert(!string.IsNullOrWhiteSpace(trimmedValue), "Trimmed title must not be empty.");

        Title title = new(trimmedValue);
        return Success<Title, DomainError>(title);
    }

    /// <summary>
    /// Rehydrates a <see cref="Title"/> instance from a stored value.
    /// </summary>
    /// <param name="value">The stored title value.</param>
    /// <returns>The rehydrated <see cref="Title"/> instance.</returns>
    /// <exception cref="ArgumentException">Thrown when the provided value is null or whitespace.</exception>
    public static Title Rehydrate(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        return new(value);
    }
}
