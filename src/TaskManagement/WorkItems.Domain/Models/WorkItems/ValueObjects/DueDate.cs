// <copyright file="TaskDueDate.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Diagnostics;
using Monads.Results;
using SharedKernel.Models.Common;
using static Monads.Results.ResultFactory;

namespace WorkItems.Domain.Models.WorkItems.ValueObjects;

/// <summary>
/// Represents a task due date that must be in the future.
/// </summary>
internal sealed record DueDate(DateTimeOffset Value);

/// <summary>
/// Factory for creating <see cref="DueDate"/> instances.
/// </summary>
internal static class DueDateFactory
{
    /// <summary>
    /// Creates a new <see cref="DueDate"/> instance.
    /// </summary>
    /// <param name="value">The due date value.</param>
    /// <param name="timeProvider">The time provider for getting the current time.</param>
    /// <returns>A result containing the task due date or a domain error.</returns>
    public static Result<DueDate, DomainError> Create(
        DateTimeOffset? value,
        TimeProvider timeProvider
    )
    {
        Debug.Assert(timeProvider is not null, "Time provider cannot be null.");

        if (!value.HasValue)
            return Success<DueDate, DomainError>(new DueDate(DateTimeOffset.MaxValue));

        Debug.Assert(value.HasValue, "Due date value must have a value here.");

        DateTimeOffset now = timeProvider.GetUtcNow();

        if (value <= now)
        {
            DomainError error = DomainErrorFactory.Validation(
                nameof(DueDate),
                "Due date must be in the future."
            );
            return Failure<DueDate, DomainError>(error);
        }

        Debug.Assert(value > now, "Due date must be after current time.");

        DueDate dueDate = new(value.Value);
        return Success<DueDate, DomainError>(dueDate);
    }

    /// <summary>
    /// Rehydrates a <see cref="DueDate"/> instance from a stored value.
    /// </summary>
    /// <param name="value">The stored due date value.</param>
    /// <returns>The rehydrated <see cref="DueDate"/> instance.</returns>
    public static DueDate Rehydrate(DateTimeOffset? value)
    {
        if (!value.HasValue)
            return new(DateTimeOffset.MaxValue);
        return new(value.Value);
    }
}
