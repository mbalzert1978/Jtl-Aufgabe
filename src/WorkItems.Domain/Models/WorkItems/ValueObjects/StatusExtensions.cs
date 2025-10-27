// <copyright file="WorkItemStatusExtensions.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Diagnostics;
using Monads.Results;
using Monads.Results.Extensions.Sync;
using SharedKernel.Models.Common;
using WorkItems.Domain.Models.WorkItems.ValueObjects;
using static Monads.Results.ResultFactory;

namespace Tasks.Domain.Models.Tasks;

/// <summary>
/// Provides extension methods for the <see cref="Status"/> enum.
/// </summary>
internal static class StatusExtensions
{
    /// <summary>
    /// Converts a <see cref="Status"/> enum value to its string representation.
    /// </summary>
    /// <param name="status">The status enum value.</param>
    /// <returns>The string representation of the status.</returns>
    /// <exception cref="UnreachableException">Thrown when the provided status is not defined.</exception>
    public static string ToString(this Status status) =>
        status switch
        {
            Status.Todo => "TODO",
            Status.InProgress => "IN_PROGRESS",
            Status.Blocked => "BLOCKED",
            Status.InReview => "IN_REVIEW",
            Status.Completed => "COMPLETED",
            Status.Cancelled => "CANCELLED",
            _ => throw new UnreachableException($"Exhausted status: '{status}'."),
        };

    /// <summary>
    /// Converts a string representation of status to the corresponding <see cref="Status"/> enum value.
    /// </summary>
    /// <param name="value">The string representation of the status.</param>
    /// <returns>The corresponding <see cref="Status"/> enum value.</returns>
    /// <exception cref="UnreachableException">Thrown when the provided string does not match any status level.</exception>
    public static Status IntoStatus(this string value) =>
        value.ToUpperInvariant() switch
        {
            "TODO" => Status.Todo,
            "IN_PROGRESS" => Status.InProgress,
            "BLOCKED" => Status.Blocked,
            "IN_REVIEW" => Status.InReview,
            "COMPLETED" => Status.Completed,
            "CANCELLED" => Status.Cancelled,
            _ => throw new UnreachableException($"Exhausted status: '{value}'."),
        };

    /// <summary>
    /// Determines whether the task status represents a terminal state.
    /// </summary>
    /// <param name="status">The task status to check.</param>
    /// <returns><c>true</c> if the status is terminal; otherwise, <c>false</c>.</returns>
    public static bool IsTerminal(this Status status) =>
        status is Status.Completed or Status.Cancelled;

    /// <summary>
    /// Determines whether the task status allows transitions.
    /// </summary>
    /// <param name="current">The current task status.</param>
    /// <returns><c>true</c> if transitions are allowed; otherwise, <c>false</c>.</returns>
    public static bool AllowsTransition(this Status current) => !current.IsTerminal();

    /// <summary>
    /// Converts a string representation of status to a <see cref="Status"/> enum value wrapped in a result.
    /// </summary>
    /// <param name="status">The string representation of the status.</param>
    /// <returns>A result containing the corresponding <see cref="Status"/> enum value or a domain error.</returns>
    public static Result<Status, DomainError> TryIntoStatus(this string status)
    {
        try
        {
            return Success<Status, DomainError>(status.IntoStatus());
        }
        catch (UnreachableException)
        {
            DomainError error = DomainErrorFactory.Validation(
                nameof(Status),
                $"Invalid task status: '{status}'."
            );
            return Failure<Status, DomainError>(error);
        }
    }

    /// <summary>
    /// Validates whether a transition from the current status to a new status is allowed.
    /// </summary>
    /// <param name="current">The current task status.</param>
    /// <param name="targetStatus">The new status to transition to.</param>
    /// <returns>A result indicating success or a domain error.</returns>
    public static Result<Status, DomainError> ValidateTransition(
        this Status current,
        string targetStatus
    ) =>
        targetStatus
            .TryIntoStatus()
            .Bind(target =>
            {
                if (current.IsTerminal())
                {
                    DomainError error = DomainErrorFactory.Validation(
                        nameof(Status),
                        $"Cannot transition from terminal status '{current}'."
                    );
                    return Failure<Status, DomainError>(error);
                }

                Debug.Assert(
                    !current.IsTerminal(),
                    "Current status must not be terminal for a valid transition."
                );

                return Success<Status, DomainError>(target);
            });
}
