// <copyright file="EventMapper.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Diagnostics;
using SharedKernel.Abstractions;

namespace Users.Application.Adapters;

/// <summary>
/// Provides extension methods for mapping domain events to application events.
/// </summary>
internal static class EventMapper
{
    /// <summary>
    /// Maps a domain event to its corresponding application event.
    /// </summary>
    /// <param name="domainEvent">The domain event to map.</param>
    /// <returns>The corresponding application event.</returns>
    /// <exception cref="UnreachableException">Thrown when no mapping is defined for the domain event type.</exception>
    public static IApplicationEvent MapToIEvent(this IDomainEvent domainEvent)
    {
        Debug.Assert(domainEvent is not null, "Domain event must not be null.");

        IApplicationEvent @event = domainEvent switch
        {
            Domain.Models.Users.UserCreatedEvent de => new UserCreatedEvent(
                de.UserId,
                de.EventId,
                de.EventType,
                de.OccurredAt
            ),
            _ => throw new UnreachableException(
                $"No mapping defined for domain event type {domainEvent.GetType().FullName}."
            ),
        };

        Debug.Assert(@event is not null, "Mapped application event must not be null.");
        return @event;
    }
}
