// <copyright file="TaskDueDate.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Diagnostics;
using Monads.Results;
using SharedKernel.Models.Common;
using static Monads.Results.ResultFactory;

namespace Tasks.Domain.Models.Tasks.ValueObjects;

/// <summary>
/// Represents a task due date that must be in the future.
/// </summary>
internal sealed record TaskDueDate(DateTimeOffset Value);

/// <summary>
/// Factory for creating <see cref="TaskDueDate"/> instances.
/// </summary>
internal static class TaskDueDateFactory
{
    /// <summary>
    /// Creates a new <see cref="TaskDueDate"/> instance.
    /// </summary>
    /// <param name="value">The due date value.</param>
    /// <param name="timeProvider">The time provider for getting the current time.</param>
    /// <returns>A result containing the task due date or a domain error.</returns>
    public static Result<TaskDueDate, DomainError> Create(
        DateTimeOffset value,
        TimeProvider timeProvider
    )
    {
        Debug.Assert(timeProvider is not null, "Time provider cannot be null.");

        if (value == default)
        {
            DomainError error = DomainErrorFactory.Generic("Due date cannot be the default value.");
            return Failure<TaskDueDate, DomainError>(error);
        }

        DateTimeOffset now = timeProvider.GetUtcNow();

        if (value <= now)
        {
            DomainError error = DomainErrorFactory.Generic("Due date must be in the future.");
            return Failure<TaskDueDate, DomainError>(error);
        }

        Debug.Assert(value > now, "Due date must be after current time.");

        TaskDueDate dueDate = new(value);
        return Success<TaskDueDate, DomainError>(dueDate);
    }
}
