// <copyright file="TaskDescription.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Diagnostics;
using Monads.Results;
using SharedKernel.Models.Common;
using static Monads.Results.ResultFactory;

namespace Tasks.Domain.Models.Tasks.ValueObjects;

/// <summary>
/// Represents a task description.
/// </summary>
internal sealed record TaskDescription(string Value);

/// <summary>
/// Factory for creating <see cref="TaskDescription"/> instances.
/// </summary>
internal static class TaskDescriptionFactory
{
    /// <summary>
    /// Creates a new <see cref="TaskDescription"/> instance.
    /// </summary>
    /// <param name="value">The description value.</param>
    /// <returns>A result containing the task description or a domain error.</returns>
    public static Result<TaskDescription, DomainError> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            DomainError error = DomainErrorFactory.Generic(
                "Task description cannot be empty or whitespace."
            );
            return Failure<TaskDescription, DomainError>(error);
        }

        if (value.Length > 4096)
        {
            DomainError error = DomainErrorFactory.Generic(
                "Task description cannot exceed 4096 characters."
            );
            return Failure<TaskDescription, DomainError>(error);
        }

        string trimmedValue = value.Trim();
        Debug.Assert(
            !string.IsNullOrWhiteSpace(trimmedValue),
            "Trimmed description must not be empty."
        );

        TaskDescription description = new(trimmedValue);
        return Success<TaskDescription, DomainError>(description);
    }
}
