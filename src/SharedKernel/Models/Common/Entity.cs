// <copyright file="Entity.cs" company="Markus - Iorio">
// Copyright (c) Markus - Iorio. All rights reserved.
// </copyright>

using System.Diagnostics;
using SharedKernel.Abstractions;

namespace SharedKernel.Models.Common;

/// <summary>
/// Base class for domain entities that provides identity and domain event support.
/// </summary>
internal abstract class Entity
{
    private readonly List<IDomainEvent> _domainEvents = [];

    /// <summary>
    /// Gets the unique identifier for this entity.
    /// </summary>
    public Guid Id { get; protected set; }

    /// <summary>
    /// Gets the collection of domain events raised by this entity.
    /// </summary>
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>
    /// Clears all domain events from this entity.
    /// </summary>
    public void ClearDomainEvents()
    {
        Debug.Assert(_domainEvents is not null, "Domain events list is null before clearing.");

        _domainEvents.Clear();

        Debug.Assert(_domainEvents.Count == 0, "Domain events were not cleared properly.");
    }

    /// <summary>
    /// Raises a domain event by adding it to the entity's event collection.
    /// </summary>
    /// <param name="domainEvent">The domain event to raise.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="domainEvent"/> is null.</exception>
    public void Raise(IDomainEvent domainEvent)
    {
        Debug.Assert(domainEvent is not null, "Domain event to raise must not be null.");

        _domainEvents.Add(domainEvent);

        Debug.Assert(
            _domainEvents.Contains(domainEvent),
            "Domain event was not added successfully."
        );
    }
}
