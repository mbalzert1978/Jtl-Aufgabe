// <copyright file="IApplicationEvent.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

namespace SharedKernel.Abstractions;

/// <summary>
/// Represents a domain event.
/// </summary>
public interface IApplicationEvent
{
    /// <summary>
    /// Gets the unique identifier of the event.
    /// </summary>
    Guid EventId { get; }

    /// <summary>
    /// Gets the type name of the event.
    /// </summary>
    string EventType { get; }

    /// <summary>
    /// Gets the timestamp when the event occurred.
    /// </summary>
    DateTimeOffset OccurredAt { get; }

    /// <summary>
    /// Gets or sets the last error message if the event publishing failed.
    /// </summary>
    string LastErrorMessage { get; }

    /// <summary>
    /// Gets or sets a value indicating whether the event has been published.
    /// </summary>
    bool IsPublished { get; }
}
