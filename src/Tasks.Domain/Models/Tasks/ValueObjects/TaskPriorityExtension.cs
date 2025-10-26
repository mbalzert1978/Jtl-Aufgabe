// <copyright file="TaskPriorityExtension.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Diagnostics;
using Monads.Results;
using SharedKernel.Models.Common;
using static Monads.Results.ResultFactory;

namespace Tasks.Domain.Models.Tasks.ValueObjects;

/// <summary>
/// Provides extension methods for the <see cref="TaskPriority"/> enum.
/// </summary>
internal static class TaskPriorityExtension
{
    /// <summary>
    /// Validates whether the specified priority is a valid enum value.
    /// </summary>
    /// <param name="priority">The priority to validate.</param>
    /// <returns>A result containing the validated priority or a domain error.</returns>
    public static Result<TaskPriority, DomainError> Validate(this TaskPriority priority)
    {
        Debug.Assert(
            Enum.IsDefined(typeof(TaskPriority), priority),
            "Priority must be a valid enum value."
        );

        if (!Enum.IsDefined(typeof(TaskPriority), priority))
        {
            DomainError error = DomainErrorFactory.Generic($"Invalid task priority: {priority}.");
            return Failure<TaskPriority, DomainError>(error);
        }

        return Success<TaskPriority, DomainError>(priority);
    }
}
