# ADR 001: Query Layer Placement

## Status

Accepted

## Context

CQRS pattern requires different handling for Commands (write) and Queries (read).

## Decision

- **Commands**: Application Layer (business logic + validation)
- **Queries**: WebApi Layer with direct DbContext access

## Rationale

1. **Performance**: Avoid unnecessary abstraction layers for reads
2. **Simplicity**: Query logic is trivial (lookup by ID, filter by FK)
3. **Vertical Slices**: All query code co-located in feature folder

## Consequences

- ✅ Faster query execution
- ✅ Less code to maintain
- ⚠️ Future complex queries may need refactoring to Application layer

## Review Trigger

If queries require:

- Complex business logic
- Reusable filtering across endpoints
- Caching strategies
→ Consider moving to Application Layer
