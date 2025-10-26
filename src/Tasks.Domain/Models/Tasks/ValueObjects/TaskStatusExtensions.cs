// <copyright file="TaskStatusExtensions.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Diagnostics;
using Monads.Results;
using SharedKernel.Models.Common;
using static Monads.Results.ResultFactory;

namespace Tasks.Domain.Models.Tasks;

/// <summary>
/// Provides extension methods for the <see cref="TaskStatus"/> enum.
/// </summary>
internal static class TaskStatusExtensions
{
    /// <summary>
    /// Determines whether the task status represents a terminal state.
    /// </summary>
    /// <param name="status">The task status to check.</param>
    /// <returns><c>true</c> if the status is terminal; otherwise, <c>false</c>.</returns>
    public static bool IsTerminal(this TaskStatus status) =>
        status is TaskStatus.Completed or TaskStatus.Cancelled;

    /// <summary>
    /// Determines whether the task status allows transitions.
    /// </summary>
    /// <param name="current">The current task status.</param>
    /// <returns><c>true</c> if transitions are allowed; otherwise, <c>false</c>.</returns>
    public static bool AllowsTransition(this TaskStatus current) => !current.IsTerminal();

    /// <summary>
    /// Validates whether a transition from the current status to a new status is allowed.
    /// </summary>
    /// <param name="current">The current task status.</param>
    /// <param name="targetStatus">The new status to transition to.</param>
    /// <returns>A result indicating success or a domain error.</returns>
    public static Result<TaskStatus, DomainError> ValidateTransition(
        this TaskStatus current,
        TaskStatus targetStatus
    )
    {
        Debug.Assert(
            Enum.IsDefined(typeof(TaskStatus), current),
            "Current status must be a valid TaskStatus."
        );

        if (!current.AllowsTransition())
        {
            DomainError error = DomainErrorFactory.Generic(
                "Cannot change status of a completed or cancelled task."
            );
            return Failure<TaskStatus, DomainError>(error);
        }

        if (!Enum.IsDefined(typeof(TaskStatus), targetStatus))
        {
            DomainError error = DomainErrorFactory.Generic("Invalid task status.");
            return Failure<TaskStatus, DomainError>(error);
        }

        return Success<TaskStatus, DomainError>(targetStatus);
    }
}
