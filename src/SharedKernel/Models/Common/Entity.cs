using System.Diagnostics;
using SharedKernel.Abstractions;

namespace SharedKernel.Models.Common;

public abstract class Entity
{
    public Guid Id { get; protected set; }
    private readonly List<IDomainEvent> _domainEvents = [];

    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void ClearDomainEvents()
    {
        Debug.Assert(_domainEvents is not null, "Domain events list is null before clearing.");

        _domainEvents.Clear();

        Debug.Assert(_domainEvents.Count == 0, "Domain events were not cleared properly.");
    }

    public void Raise(IDomainEvent domainEvent)
    {
        Debug.Assert(domainEvent is not null, "Attempted to raise a null domain event.");

        _domainEvents.Add(domainEvent);

        Debug.Assert(
            _domainEvents.Contains(domainEvent),
            "Domain event was not added successfully."
        );
    }
}
