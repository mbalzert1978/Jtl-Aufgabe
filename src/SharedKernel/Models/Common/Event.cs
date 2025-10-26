// <copyright file="Event.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Diagnostics;
using SharedKernel.Abstractions;

namespace SharedKernel.Models.Common;

/// <summary>
/// Represents the base type for all domain events in the task management system.
/// </summary>
/// <param name="EventId">The unique identifier for this event instance.</param>
/// <param name="AggregateId">The identifier of the aggregate that produced this event.</param>
/// <param name="EventType">The type name of the event for serialization and routing.</param>
/// <param name="OccurredAt">The timestamp when the event occurred.</param>
internal abstract record Event(
    Guid EventId,
    Guid AggregateId,
    string EventType,
    DateTimeOffset OccurredAt
) : IDomainEvent;
