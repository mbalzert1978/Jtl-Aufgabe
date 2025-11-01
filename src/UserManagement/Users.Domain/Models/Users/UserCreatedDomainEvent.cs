// <copyright file="UserCreatedDomainEvent.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using SharedKernel.Abstractions;
using SharedKernel.Models.Common;

namespace Users.Domain.Models.Users;

/// <summary>
/// Domain event raised when a new user is created.
/// </summary>
/// <param name="UserId">The unique identifier of the created user.</param>
/// <param name="OccurredAt">The timestamp when the event occurred.</param>
internal sealed record UserCreatedEvent(Guid UserId, DateTimeOffset OccurredAt)
    : Event(Guid.NewGuid(), UserId, nameof(UserCreatedEvent), OccurredAt),
        IDomainEvent;
