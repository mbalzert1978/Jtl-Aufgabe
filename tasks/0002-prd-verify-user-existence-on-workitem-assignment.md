# PRD: Verify User Existence on WorkItem Assignment

## Introduction/Overview

When assigning a WorkItem to a user, the system must verify that the user exists before allowing the assignment. This prevents orphaned WorkItems being assigned to non-existent users and ensures data integrity across module boundaries in our modular monolith architecture.

Currently, the `AssignWorkItem` use case in the WorkItems module has no validation to check if the target user exists in the Users module. This PRD defines the implementation of cross-module communication using a clean public API pattern, following modular monolith best practices.

**Problem:** WorkItems can be assigned to users that don't exist in the system.

**Goal:** Implement a DDD-compliant user existence validation using Specification Pattern and Hexagonal Architecture (Port & Adapter), ensuring the domain layer controls the business rule while maintaining clear module boundaries.

## Goals

1. Implement `IUserExistenceChecker` interface in WorkItems.Domain as a domain service (Port)
2. Create `UserMustExistSpecification` in WorkItems.Domain to encapsulate the validation rule
3. Implement `UserExistenceAdapter` in WorkItems.Application that bridges to Users.Application public API (Adapter)
4. Integrate the specification into WorkItem domain model's assignment logic
5. Return a clear domain error (`UserNotFoundError`) when specification is not satisfied
6. Maintain module independence by controlling coupling through stable contracts
7. Enforce module boundaries with architecture tests to prevent direct access to internal types
8. Achieve 100% test coverage for the new validation logic in Tests.WorkItems

## User Stories

### Story 1: Developer - Domain-Driven Design Implementation

**As a** developer  
**I want** WorkItem assignment logic to validate user existence through a domain specification  
**So that** the business rule "user must exist" is enforced in the domain layer following DDD principles

**Acceptance Criteria:**

- `IUserExistenceChecker` interface is defined in WorkItems.Domain/Abstractions (Port)
- `UserMustExistSpecification` class exists in WorkItems.Domain/Specifications
- `UserExistenceAdapter` implements `IUserExistenceChecker` in WorkItems.Application (Adapter)
- Adapter calls `IUserService` from Users.Application (public API)
- `IUserService` is defined in Users.Application and marked as `public`
- WorkItem domain model uses the specification in its assignment logic
- Assignment fails with `UserNotFoundError` if specification is not satisfied

### Story 2: DevOps - Test Coverage

**As a** DevOps engineer  
**I want** comprehensive unit tests for the user existence verification feature  
**So that** I can ensure the feature works correctly and prevent regressions in CI/CD

**Acceptance Criteria:**

- Unit tests exist in `tests/Tests.WorkItems` for the `AssignWorkItem` use case
- Test covers happy path: user exists → assignment succeeds
- Test covers failure path: user doesn't exist → returns `UserNotFoundError`
- Architecture tests enforce that WorkItems module can only access public types from Users module
- All tests pass in CI/CD pipeline

## Functional Requirements

### 1. Domain Layer - Port Interface (WorkItems.Domain)

- **FR-1.1:** Create `IUserExistenceChecker` interface in WorkItems.Domain/Abstractions (Port in Hexagonal Architecture)
- **FR-1.2:** Define method signature: `Task<bool> ExistsAsync(UserId userId, CancellationToken cancellationToken = default)`
- **FR-1.3:** Interface represents the domain's need without depending on external implementations
- **FR-1.4:** Keep interface in domain layer to enforce Dependency Inversion Principle

### 2. Domain Layer - Specification Pattern (WorkItems.Domain)

- **FR-2.1:** Create `UserMustExistSpecification` class in WorkItems.Domain/Specifications
- **FR-2.2:** Specification encapsulates the business rule "assigned user must exist"
- **FR-2.3:** Specification depends on `IUserExistenceChecker` interface
- **FR-2.4:** Provide method `Task<bool> IsSatisfiedByAsync(UserId userId, CancellationToken cancellationToken)`
- **FR-2.5:** Make specification reusable across different use cases if needed

### 3. Domain Model Integration (WorkItems.Domain)

- **FR-3.1:** Add static factory method `AssignAsync` to WorkItem aggregate/entity
- **FR-3.2:** Method signature: `Task<Result<WorkItem>> AssignAsync(WorkItemId id, UserId assigneeId, UserMustExistSpecification spec, CancellationToken ct)`
- **FR-3.3:** Validate using specification before creating/updating WorkItem
- **FR-3.4:** Return `UserNotFoundError` if specification is not satisfied
- **FR-3.5:** Keep domain logic synchronous where possible, async only for external checks

### 4. Application Layer - Adapter (WorkItems.Application)

- **FR-4.1:** Create `UserExistenceAdapter` class in WorkItems.Application/Adapters (Adapter in Hexagonal Architecture)
- **FR-4.2:** Implement `IUserExistenceChecker` interface
- **FR-4.3:** Inject `Users.Application.IUserService` via constructor
- **FR-4.4:** Translate between WorkItems domain types (UserId) and Users module types (Guid)
- **FR-4.5:** Mark adapter as `internal sealed` class
- **FR-4.6:** Register `IUserExistenceChecker` → `UserExistenceAdapter` in WorkItems.Application DI

### 5. Domain Service (Users.Domain)

- **FR-5.1:** Create domain service interface `IUserExistenceService` in Users.Domain/Abstractions
- **FR-5.2:** Interface provides user existence checking capability within Users domain
- **FR-5.3:** Implementation in Users.Infrastructure queries the Users database
- **FR-5.4:** This is internal to the Users module

### 6. Public API (Users.Application)

- **FR-6.1:** Create public interface `IUserService` in Users.Application/Abstractions (Module's Public API)
- **FR-6.2:** Interface exposes `UserExistsAsync(Guid userId, CancellationToken cancellationToken)` method
- **FR-6.3:** Implementation delegates to Users.Domain service
- **FR-6.4:** This is the ONLY public contract exposed to other modules (including WorkItems)
- **FR-6.5:** Mark interface as `public`, implementation as `internal`

### 7. Use Case Orchestration (WorkItems.Application)

- **FR-7.1:** Update `AssignWorkItem` use case to inject `UserMustExistSpecification`
- **FR-7.2:** Call `WorkItem.AssignAsync()` domain method with the specification
- **FR-7.3:** Use case orchestrates infrastructure concerns (repository, transactions) but domain does validation
- **FR-7.4:** Return Result from domain without additional application-layer validation

### 8. Error Handling (WorkItems.Domain)

- **FR-8.1:** Create new error type `UserNotFoundError` implementing `IError` in WorkItems.Domain/Errors
- **FR-8.2:** Error should include the UserId that was not found
- **FR-8.3:** Error message should be clear: "User with ID '{userId}' does not exist."
- **FR-8.4:** Provide static factory method `Create(UserId userId)`

### 9. Architecture Boundary Enforcement (Tests.WorkItems)

- **FR-9.1:** Add architecture test in `Tests.WorkItems/Architecture/ModuleBoundaryTests.cs`
- **FR-9.2:** Test ensures WorkItems.Application can only access public types from Users.Application
- **FR-9.3:** Test ensures WorkItems.Domain has no dependencies on Users module at all
- **FR-9.4:** Test ensures WorkItems cannot directly access Users.Infrastructure or Users.Domain internals
- **FR-9.5:** Use NetArchTest.Rules (already in use) for implementation

### 10. Unit Test Coverage (Tests.WorkItems)

- **FR-10.1:** Domain Tests: Mock `IUserExistenceChecker` to test WorkItem.AssignAsync logic
- **FR-10.2:** Test: `WorkItem_AssignAsync_WhenUserExists_ShouldSucceed`
- **FR-10.3:** Test: `WorkItem_AssignAsync_WhenUserDoesNotExist_ShouldReturnUserNotFoundError`
- **FR-10.4:** Application Tests: Mock specification to test use case orchestration
- **FR-10.5:** Test: `AssignWorkItem_UseCase_ShouldCallDomainMethod`
- **FR-10.6:** Adapter Tests: Mock `IUserService` to test adapter translation logic
- **FR-10.7:** Specification Tests: Mock `IUserExistenceChecker` to test specification behavior

## Non-Goals (Out of Scope)

- **NG-1:** Asynchronous user validation via domain events (synchronous validation only)
- **NG-2:** Caching of user existence information in WorkItems module
- **NG-3:** Automatic user creation if user doesn't exist
- **NG-4:** Reassignment validation (only new assignments, as reassign feature doesn't exist yet)
- **NG-5:** Validating user existence during WorkItem updates (only during initial assignment)
- **NG-6:** Handling Users service unavailability with retry logic (modular monolith assumption: service is always available)
- **NG-7:** Batch validation of multiple users
- **NG-8:** Creating a separate Users.PublicApi assembly (keep interface in Users.Application as public)

## Design Considerations

### DDD & Hexagonal Architecture Pattern

This implementation follows **Domain-Driven Design** principles with **Hexagonal Architecture (Ports & Adapters)**:

1. **Domain Controls Business Rules:** The "user must exist" rule lives in WorkItems.Domain, not Application layer
2. **Specification Pattern:** Encapsulates the validation rule as a reusable, testable domain concept
3. **Port (Interface in Domain):** `IUserExistenceChecker` is the Port - domain defines what it needs
4. **Adapter (Implementation in Application):** `UserExistenceAdapter` is the Adapter - application provides implementation
5. **Dependency Inversion:** Domain doesn't depend on Application or Infrastructure - dependencies point inward
6. **Public API (Users Module):** `IUserService` is the stable contract exposed to other modules

### Module Communication Pattern

Follow the pattern demonstrated in the transcript combined with DDD:

1. **Internal Implementation:** Keep all implementation details (services, repositories, DB contexts) internal
2. **Public API Contract:** Expose only stable, minimal interfaces as public (`IUserService`)
3. **Dependency Direction:** WorkItems.Application → Users.Application (cross-module), WorkItems.Domain → no external dependencies
4. **Synchronous Communication:** Direct interface call through adapter (not events for precondition validation)
5. **Domain Purity:** Domain layer remains testable without external dependencies

### Architecture Layers

```bash
┌─────────────────────────────────────────────────────────────┐
│                    WorkItems.Application                    │
│  ┌───────────────-─┐              ┌─────────────────────┐   │
│  │  AssignWorkItem │              │ UserExistenceAdapter│   │
│  │   (Use Case)    │─────────────▶│    (Adapter)        │   │
│  └────────────────-┘              └─────────────────────┘   │
│         │                                   │               │
│         │ uses                              │ implements    │
│         ▼                                   ▼               │
│  ┌────────────────────────────┐   ┌──────────────────────┐  │
│  │UserMustExistSpecification  │   │ IUserExistenceChecker│  │
│  │                            │◀──│      (Port)          │  │
│  └────────────────────────────┘   └──────────────────────┘  │
│         │ from Domain                      │ from Domain    │
└─────────┼──────────────────────────────────┼────────────────┘
          │                                  │
          ▼                                  │
┌──────────────────────────────────────────┐ │
│        WorkItems.Domain                  │ │
│  ┌────────────────────────────────┐      │ │
│  │         WorkItem               │      │ │
│  │  AssignAsync(spec) ────────────┼─────-|-┘ 
│  └────────────────────────────────┘      │
└──────────────────────────────────────────┘

External Module Dependency (via Adapter):
┌──────────────────────────────────────────┐
│        Users.Application                 │
│  ┌────────────────────────────────┐      │
│  │      IUserService (Public)     │◀────-┼── Called by Adapter
│  └────────────────────────────────┘      │
│             │ delegates to               │
│             ▼                            │
│  ┌────────────────────────────────┐      │
│  │  Users.Domain Service          │      │
│  └────────────────────────────────┘      │
└──────────────────────────────────────────┘
```

### Component Overview

**WorkItems Module:**

- `WorkItems.Domain`: Contains Port interface, Specification, Domain Model, Errors (no external dependencies)
- `WorkItems.Application`: Contains Adapter, Use Cases, DI registration (depends on Users.Application public API)

**Users Module:**

- `Users.Domain`: Contains domain service for user existence checking (internal)
- `Users.Application`: Exposes public `IUserService` interface, delegates to domain service (Public API for other modules)

### Key Architecture Decisions

1. **Domain Service Location**: Users.Domain contains the actual user existence checking logic
2. **Public API Exposure**: Users.Application exposes `IUserService` as the public contract
3. **Adapter Pattern**: WorkItems.Application adapter translates between modules
4. **Specification Pattern**: Encapsulates business rule in WorkItems.Domain
5. **Dependency Direction**: WorkItems.Application → Users.Application (one-way, via public interface only)

## Technical Considerations

### Performance

- **Synchronous Call:** Since this is a modular monolith, the database call is local and fast
- **Database Query:** Simple existence check - highly optimized
- **No Caching Needed:** At this stage, direct DB queries are acceptable

### Dependency Registration

- All services registered as **Scoped** (standard for services that access DB)
- Registration happens in respective Application layer DependencyInjection classes
- WorkItems.Application: `IUserExistenceChecker` → `UserExistenceAdapter`
- WorkItems.Application: `UserMustExistSpecification`
- Users.Application: `IUserService` → implementation

### Testing Strategy

- **Domain Tests:** Mock `IUserExistenceChecker` to test WorkItem and Specification logic in isolation
- **Application Tests:** Mock specification to test use case orchestration
- **Adapter Tests:** Mock `IUserService` to test adapter translation
- **Architecture Tests:** Use NetArchTest.Rules to enforce module boundaries
- **Integration Tests:** Out of scope for this PRD (can be added later)

### Specification Pattern Rationale

**Why Specification Pattern?**

- Encapsulates business rule as first-class concept
- Reusable across multiple domain operations
- Easier to compose multiple specifications (AND, OR logic)
- Makes business rules explicit and discoverable
- Testable independently from domain model

### Synchronous vs Asynchronous Validation

#### Chosen Approach: Synchronous Validation**

Synchronous validation provides immediate feedback and simpler implementation without eventual consistency concerns. In a modular monolith, the database call is local and fast, making the slight latency increase negligible.

**Decision Rationale:** Assignment validation is a precondition (must be checked before assignment), not a side effect (something that happens after). Synchronous validation ensures the business rule is enforced atomically.

## Success Metrics

- **Code Quality:** All architecture tests pass - modules respect boundaries
- **Test Coverage:** 100% coverage for new validation logic (Domain, Application, Adapter layers)
- **Functionality:** Zero WorkItems assigned to non-existent users in production
- **Maintainability:** Changes to Users module internals don't break WorkItems (as long as public API contract is maintained)
- **DDD Compliance:** Domain layer remains pure and testable without external dependencies

## Open Questions

- ~~Should we validate user existence synchronously or asynchronously?~~ → **Resolved: Synchronous**
- ~~What should happen if user doesn't exist?~~ → **Resolved: Return UserNotFoundError**
- ~~Where should the validation logic live?~~ → **Resolved: Domain layer using Specification Pattern**
- ~~How do we avoid violating DDD principles?~~ → **Resolved: Port & Adapter (Hexagonal Architecture)**
- ~~How do we enforce module boundaries?~~ → **Resolved: NetArchTest architecture tests**

## Implementation Checklist

### Phase 1: Users Module - Domain Service (Users.Domain)

- [ ] Create `IUserExistenceService` interface in Users.Domain/Abstractions
- [ ] Implementation in Users.Infrastructure queries Users database
- [ ] Register service in Users.Infrastructure DependencyInjection

### Phase 2: Users Module - Public API (Users.Application)

- [ ] Create `IUserService` public interface in Users.Application/Abstractions
- [ ] Define `UserExistsAsync(Guid userId, CancellationToken)` method
- [ ] Mark interface as `public`
- [ ] Create internal implementation that delegates to Users.Domain service
- [ ] Register in Users.Application DependencyInjection

### Phase 3: WorkItems Module - Domain Layer (WorkItems.Domain)

- [ ] Create `IUserExistenceChecker` port interface in WorkItems.Domain/Abstractions
- [ ] Create `UserMustExistSpecification` class in WorkItems.Domain/Specifications
- [ ] Create `UserNotFoundError` in WorkItems.Domain/Errors
- [ ] Update `WorkItem` aggregate with `AssignAsync` method that uses specification
- [ ] Add XML documentation

### Phase 4: WorkItems Module - Adapter (WorkItems.Application)

- [ ] Create `UserExistenceAdapter` implementing `IUserExistenceChecker`
- [ ] Adapter injects `Users.Application.IUserService`
- [ ] Implement translation between domain types (UserId ↔ Guid)
- [ ] Mark adapter as `internal sealed`
- [ ] Register adapter and specification in DependencyInjection

### Phase 5: WorkItems Module - Use Case (WorkItems.Application)

- [ ] Update `AssignWorkItem` use case to inject `UserMustExistSpecification`
- [ ] Call `WorkItem.AssignAsync()` domain method
- [ ] Handle Result from domain
- [ ] Use case focuses on orchestration, not validation

### Phase 6: Testing - Domain (Tests.WorkItems)

- [ ] Test WorkItem.AssignAsync with mocked `IUserExistenceChecker`
- [ ] Test UserMustExistSpecification behavior
- [ ] Test UserNotFoundError creation

### Phase 7: Testing - Application (Tests.WorkItems)

- [ ] Test UserExistenceAdapter with mocked `IUserService`
- [ ] Test AssignWorkItem use case with mocked specification
- [ ] Verify correct orchestration and error handling

### Phase 8: Testing - Architecture (Tests.WorkItems)

- [ ] Create ModuleBoundaryTests.cs
- [ ] Test: WorkItems.Application can only access Users.Application public types
- [ ] Test: WorkItems.Domain has no dependencies on Users module
- [ ] Test: WorkItems cannot access Users.Infrastructure or Users.Domain
- [ ] Verify all architecture tests pass

---

**Created:** 28. Oktober 2025  
**Updated:** 28. Oktober 2025 (DDD & Hexagonal Architecture approach)  
**Status:** Ready for Implementation  
**Assignee:** TBD  
**Estimated Effort:** 3-4 hours (increased due to additional abstraction layers)
