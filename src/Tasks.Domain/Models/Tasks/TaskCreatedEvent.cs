// <copyright file="TaskCreatedEvent.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using SharedKernel.Models.Common;

namespace Tasks.Domain.Models.Tasks;

/// <summary>
/// Represents an event that is raised when a new task is created.
/// </summary>
/// <param name="TaskId">The identifier of the aggregate that produced this event.</param>
/// <param name="OccurredAt">The timestamp when the task was created.</param>
internal sealed record TaskCreatedEvent(Guid TaskId, DateTimeOffset OccurredAt)
    : Event(Guid.NewGuid(), TaskId, nameof(TaskCreatedEvent), OccurredAt);
