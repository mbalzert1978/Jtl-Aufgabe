// <copyright file="TaskPriorityExtension.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Diagnostics;
using Monads.Results;
using SharedKernel.Models.Common;
using static Monads.Results.ResultFactory;

namespace WorkItems.Domain.Models.WorkItems.ValueObjects;

/// <summary>
/// Provides extension methods for the <see cref="Priority"/> enum.
/// </summary>
internal static class PriorityExtension
{
    /// <summary>
    /// Converts a string representation of priority to the corresponding <see cref="Priority"/> enum value.
    /// </summary>
    /// <param name="value">The string representation of the priority.</param>
    /// <returns>The corresponding <see cref="Priority"/> enum value.</returns>
    /// <exception cref="UnreachableException">Thrown when the provided string does not match any priority level.</exception>
    public static Priority IntoPriority(this string value) =>
        value.ToUpperInvariant() switch
        {
            "LOW" => Priority.Low,
            "NORMAL" => Priority.Normal,
            "HIGH" => Priority.High,
            "CRITICAL" => Priority.Critical,
            _ => throw new UnreachableException($"Exhausted priority: '{value}'."),
        };

    /// <summary>
    /// Converts a <see cref="Priority"/> enum value to its string representation.
    /// </summary>
    /// <param name="priority">The priority enum value.</param>
    /// <returns>The string representation of the priority.</returns>
    /// <exception cref="UnreachableException">Thrown when the provided priority is not defined.</exception>
    public static string ToString(this Priority priority) =>
        priority switch
        {
            Priority.Low => "LOW",
            Priority.Normal => "NORMAL",
            Priority.High => "HIGH",
            Priority.Critical => "CRITICAL",
            _ => throw new UnreachableException($"Exhausted priority: '{priority}'."),
        };

    /// <summary>
    /// Converts a string representation of priority to a <see cref="Priority"/> enum value wrapped in a result.
    /// </summary>
    /// <param name="priority">The string representation of the priority.</param>
    /// <returns>A result containing the corresponding <see cref="Priority"/> enum value or a
    public static Result<Priority, DomainError> TryIntoPriority(this string priority)
    {
        try
        {
            return Success<Priority, DomainError>(priority.IntoPriority());
        }
        catch (UnreachableException)
        {
            DomainError error = DomainErrorFactory.Generic("Invalid priority value.");
            return Failure<Priority, DomainError>(error);
        }
    }

    /// <summary>
    /// Validates whether the specified priority is a valid enum value.
    /// </summary>
    /// <param name="priority">The priority to validate.</param>
    /// <returns>A result containing the validated priority or a domain error.</returns>
    public static Result<Priority, DomainError> Validate(this Priority priority)
    {
        if (!Enum.IsDefined(typeof(Priority), priority))
        {
            DomainError error = DomainErrorFactory.Generic($"Invalid task priority: {priority}.");
            return Failure<Priority, DomainError>(error);
        }

        return Success<Priority, DomainError>(priority);
    }
}
