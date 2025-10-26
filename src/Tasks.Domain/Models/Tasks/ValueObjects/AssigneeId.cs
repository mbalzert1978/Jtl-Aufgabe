// <copyright file="AssigneeId.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Diagnostics;
using Monads.Results;
using SharedKernel.Models.Common;
using static Monads.Results.ResultFactory;

namespace Tasks.Domain.Models.Tasks.ValueObjects;

/// <summary>
/// Represents the unique identifier of a user assigned to a task.
/// </summary>
internal sealed record AssigneeId(Guid Value);

/// <summary>
/// Factory for creating <see cref="AssigneeId"/> instances.
/// </summary>
internal static class AssigneeIdFactory
{
    /// <summary>
    /// Creates a new <see cref="AssigneeId"/> instance.
    /// </summary>
    /// <param name="value">The assignee identifier value.</param>
    /// <returns>A result containing the assignee ID or a domain error.</returns>
    public static Result<AssigneeId, DomainError> Create(Guid value)
    {
        if (value == Guid.Empty)
        {
            DomainError error = DomainErrorFactory.Generic("Assignee ID cannot be empty.");
            return Failure<AssigneeId, DomainError>(error);
        }

        Debug.Assert(value != Guid.Empty, "Assignee ID must not be empty.");

        AssigneeId assigneeId = new(value);
        return Success<AssigneeId, DomainError>(assigneeId);
    }
}
