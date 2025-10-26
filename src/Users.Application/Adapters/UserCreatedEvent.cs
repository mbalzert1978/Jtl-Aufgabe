// <copyright file="UserCreatedEvent.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using SharedKernel.Abstractions;

namespace Users.Application.Adapters;

/// <summary>
/// Represents an event that is raised when a user is created.
/// </summary>
/// <param name="UserId">The unique identifier of the created user.</param>
/// <param name="EventId">The unique identifier of the event.</param>
/// <param name="EventType">The type of the event.</param>
/// <param name="OccurredAt">The date and time when the event occurred.</param>
/// <param name="LastErrorMessage">The last error message if the event publication failed.</param>
/// <param name="IsPublished">Indicates whether the event has been published.</param>
public sealed record UserCreatedEvent(
    Guid UserId,
    Guid EventId,
    string EventType,
    DateTimeOffset OccurredAt,
    string LastErrorMessage = "",
    bool IsPublished = false
) : IApplicationEvent;
