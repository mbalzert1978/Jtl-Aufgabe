# ADR 004: Event Flow Direction

## Status

Accepted

## Context

Define which modules publish events to which other modules.

## Decision

**Unidirectional Event Flow**: Upstream → Downstream only

```bash
Users (Core Domain)
  ↓ publishes events
WorkItems (Supporting Domain)
  ↓ NO events back to Users
  ↓ publishes events for other supporting domains
Notifications (Supporting Domain)
```

## Rationale

1. **Separation of Concerns**: Core domain independent of supporting
2. **Loose Coupling**: WorkItems can be added/removed without Users changes
3. **Clear Dependencies**: Acyclic dependency graph

## Alternatives Considered

- Bidirectional events (rejected: circular dependency)
- WorkItems updates User aggregate (rejected: tight coupling)
- Read Model in Users (considered for future if needed)

## Consequences

- ✅ Clear module boundaries
- ✅ Users module remains stable
- ⚠️ Cross-module queries orchestrated in WebApi layer
- ⚠️ No real-time statistics in User aggregate
