// <copyright file="TaskTitle.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Diagnostics;
using Monads.Results;
using SharedKernel.Models.Common;
using static Monads.Results.ResultFactory;

namespace Tasks.Domain.Models.Tasks.ValueObjects;

/// <summary>
/// Represents a task title with validation constraints.
/// </summary>
internal sealed record TaskTitle(string Value);

/// <summary>
/// Factory for creating <see cref="TaskTitle"/> instances.
/// </summary>
internal static class TaskTitleFactory
{
    /// <summary>
    /// Creates a new <see cref="TaskTitle"/> instance.
    /// </summary>
    /// <param name="value">The title value.</param>
    /// <returns>A result containing the task title or a domain error.</returns>
    public static Result<TaskTitle, DomainError> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            DomainError error = DomainErrorFactory.Generic(
                "Task title cannot be empty or whitespace."
            );
            return Failure<TaskTitle, DomainError>(error);
        }

        if (value.Length > 100)
        {
            DomainError error = DomainErrorFactory.Generic(
                "Task title cannot exceed 100 characters."
            );
            return Failure<TaskTitle, DomainError>(error);
        }

        string trimmedValue = value.Trim();
        Debug.Assert(!string.IsNullOrWhiteSpace(trimmedValue), "Trimmed title must not be empty.");

        TaskTitle title = new(trimmedValue);
        return Success<TaskTitle, DomainError>(title);
    }
}
