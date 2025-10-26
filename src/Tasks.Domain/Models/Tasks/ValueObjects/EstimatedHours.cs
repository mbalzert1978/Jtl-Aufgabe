// <copyright file="EstimatedHours.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Diagnostics;
using Monads.Results;
using SharedKernel.Models.Common;
using static Monads.Results.ResultFactory;

namespace Tasks.Domain.Models.Tasks.ValueObjects;

/// <summary>
/// Represents the estimated hours for a task.
/// </summary>
internal sealed record EstimatedHours(int Value);

/// <summary>
/// Factory for creating <see cref="EstimatedHours"/> instances.
/// </summary>
internal static class EstimatedHoursFactory
{
    /// <summary>
    /// Creates a new <see cref="EstimatedHours"/> instance.
    /// </summary>
    /// <param name="value">The estimated hours value.</param>
    /// <returns>A result containing the estimated hours or a domain error.</returns>
    public static Result<EstimatedHours, DomainError> Create(int value)
    {
        if (value < 0)
        {
            DomainError error = DomainErrorFactory.Generic("Estimated hours cannot be negative.");
            return Failure<EstimatedHours, DomainError>(error);
        }

        Debug.Assert(value >= 0, "Estimated hours must be non-negative.");

        EstimatedHours estimatedHours = new(value);
        return Success<EstimatedHours, DomainError>(estimatedHours);
    }
}
