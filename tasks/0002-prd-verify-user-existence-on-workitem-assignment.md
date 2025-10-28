# PRD: Verify User Existence on WorkItem Assignment

## Introduction/Overview

When assigning a WorkItem to a user, the system must verify that the user exists before allowing the assignment. This prevents orphaned WorkItems being assigned to non-existent users and ensures data integrity across module boundaries in our modular monolith architecture.

Currently, the `AssignWorkItem` use case in the WorkItems module has no validation to check if the target user exists in the Users module. This PRD defines the implementation of cross-module communication using a clean public API pattern, following modular monolith best practices.

**Problem:** WorkItems can be assigned to users that don't exist in the system.

**Goal:** Implement a public API in Users.Application that allows WorkItems.Application to verify user existence synchronously before assignment, while maintaining clear module boundaries.

## Goals

1. Implement a public `IUserService` interface in Users.Application that exposes user existence checking
2. Integrate the user existence check into the `AssignWorkItem` use case
3. Return a clear domain error (`UserNotFoundError`) when assignment fails due to non-existent user
4. Maintain module independence by controlling coupling through a stable public API contract
5. Enforce module boundaries with architecture tests to prevent direct access to internal types
6. Achieve 100% test coverage for the new validation logic in Tests.WorkItems

## User Stories

### Story 1: Developer - Public API Implementation

**As a** developer  
**I want** the `AssignWorkItem` use case to check user existence via a public API from Users.Application  
**So that** WorkItems can only be assigned to valid users and modules remain loosely coupled

**Acceptance Criteria:**

- `IUserService` interface is defined in Users.Application and marked as `public`
- Interface contains `Task<bool> UserExistsAsync(Guid userId, CancellationToken cancellationToken)` method
- Implementation class is `internal` to Users.Application
- Service is registered in Users.Application DI container
- `AssignWorkItem` use case injects `IUserService` and calls it before assignment
- Assignment fails with `UserNotFoundError` if user doesn't exist

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

### 1. Public API Interface (Users.Application)

- **FR-1.1:** Create a public interface `IUserService` in Users.Application
- **FR-1.2:** Define method signature: `Task<bool> UserExistsAsync(Guid userId, CancellationToken cancellationToken)`
- **FR-1.3:** Implement `UserService` class (internal) that queries the Users database to check existence
- **FR-1.4:** Register `IUserService` → `UserService` in Users.Application's `DependencyInjection.cs`

### 2. User Existence Validation (WorkItems.Application)

- **FR-2.1:** Inject `IUserService` into `AssignWorkItem` use case via constructor
- **FR-2.2:** Call `UserExistsAsync` with the target user ID before creating/updating the WorkItem
- **FR-2.3:** If user doesn't exist, return `Result<WorkItem>.Failure(UserNotFoundError.Create(userId))`
- **FR-2.4:** If user exists, proceed with WorkItem assignment as normal

### 3. Error Handling

- **FR-3.1:** Create new error type `UserNotFoundError` implementing `IError` in WorkItems.Domain
- **FR-3.2:** Error should include the userId that was not found
- **FR-3.3:** Error message should be clear: "User with ID '{userId}' does not exist."
- **FR-3.4:** Since this is a modular monolith, Users service unavailability should not occur (fail fast is acceptable)

### 4. Architecture Boundary Enforcement

- **FR-4.1:** Add architecture test in `Tests.WorkItems/Architecture/LayerTests.cs` (or new `ModuleBoundaryTests.cs`)
- **FR-4.2:** Test ensures WorkItems.Application can only access public types from Users.Application
- **FR-4.3:** Test ensures WorkItems cannot directly access Users.Infrastructure or Users.Domain internals
- **FR-4.4:** Use NetArchTest.Rules (already in use) for implementation

### 5. Unit Test Coverage (Tests.WorkItems)

- **FR-5.1:** Test: `AssignWorkItem_WhenUserExists_ShouldSucceed`
- **FR-5.2:** Test: `AssignWorkItem_WhenUserDoesNotExist_ShouldReturnUserNotFoundError`
- **FR-5.3:** Mock `IUserService` in tests to control behavior
- **FR-5.4:** Verify correct error is returned with proper userId

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

### Module Communication Pattern

Follow the pattern demonstrated in the transcript:

1. **Internal Implementation:** Keep all implementation details (services, repositories, DB contexts) internal
2. **Public API Contract:** Expose only stable, minimal interfaces as public
3. **Dependency Direction:** WorkItems → Users (one-way dependency)
4. **Synchronous Communication:** Direct interface call (not events for this validation)

### Project Structure

```
src/Users.Application/
  ├── Abstractions/
  │   └── IUserService.cs        // PUBLIC interface
  ├── Services/
  │   └── UserService.cs         // INTERNAL implementation
  └── DependencyInjection.cs     // Register IUserService

src/WorkItems.Application/
  └── UseCases/
      └── AssignWorkItem.cs      // Injects IUserService

src/WorkItems.Domain/
  └── Errors/
      └── UserNotFoundError.cs   // New error type

tests/Tests.WorkItems/
  ├── Architecture/
  │   └── ModuleBoundaryTests.cs // New boundary tests
  └── UseCases/
      └── AssignWorkItemTests.cs // Unit tests
```

### Interface Design

```csharp
// Users.Application/Abstractions/IUserService.cs
public interface IUserService
{
    Task<bool> UserExistsAsync(Guid userId, CancellationToken cancellationToken = default);
}
```

### Error Design

```csharp
// WorkItems.Domain/Errors/UserNotFoundError.cs
public sealed record UserNotFoundError : IError
{
    public string Message => $"User with ID '{UserId}' does not exist.";
    public Guid UserId { get; init; }
    
    public static UserNotFoundError Create(Guid userId) => new() { UserId = userId };
}
```

## Technical Considerations

### Performance

- **Synchronous Call:** Since this is a modular monolith, the database call is local and fast
- **Database Query:** Simple existence check via `Users.Any(u => u.Id == userId)` - highly optimized
- **No Caching Needed:** At this stage, direct DB queries are acceptable

### Dependency Registration

- Register `IUserService` as **Scoped** (standard for services that access DB)
- Registration happens in `Users.Application.DependencyInjection`
- WorkItems.Application gets the service via DI automatically

### Testing Strategy

- **Unit Tests:** Mock `IUserService` in WorkItems tests
- **Architecture Tests:** Use NetArchTest.Rules to enforce boundaries
- **Integration Tests:** Out of scope for this PRD (can be added later)

### Pro/Contra: Synchronous vs Asynchronous Validation

**Synchronous (Chosen Approach):**

- ✅ **Pro:** Immediate feedback - assignment fails instantly if user doesn't exist
- ✅ **Pro:** Simpler implementation - no eventual consistency concerns
- ✅ **Pro:** Better UX - user knows immediately if assignment failed
- ✅ **Pro:** Easier to reason about - no race conditions
- ⚠️ **Contra:** Tight temporal coupling - WorkItems operation depends on Users query
- ⚠️ **Contra:** Slightly increased latency (but negligible in modular monolith)

**Asynchronous (Alternative - Not Chosen):**

- ✅ **Pro:** Loose temporal coupling - operations can proceed independently
- ❌ **Contra:** Complex error handling - how to notify about failed assignments?
- ❌ **Contra:** Eventual consistency - WorkItem created first, validated later
- ❌ **Contra:** Poor UX - user doesn't know if assignment will succeed
- ❌ **Contra:** Orphaned data risk - WorkItems might remain in invalid state

**Decision:** Synchronous validation is more appropriate for this use case because assignment validation is a precondition, not a side effect.

## Success Metrics

- **Code Quality:** All architecture tests pass - modules respect boundaries
- **Test Coverage:** 100% coverage for new validation logic in AssignWorkItem
- **Functionality:** Zero WorkItems assigned to non-existent users in production
- **Maintainability:** Changes to Users module internals don't break WorkItems (as long as public API contract is maintained)

## Open Questions

- ~~Should we validate user existence synchronously or asynchronously?~~ → **Resolved: Synchronous**
- ~~What should happen if user doesn't exist?~~ → **Resolved: Return UserNotFoundError**
- ~~Where should the public API interface be defined?~~ → **Resolved: Users.Application, marked public**
- ~~How do we enforce module boundaries?~~ → **Resolved: NetArchTest architecture tests**

## Implementation Checklist

### Phase 1: Public API (Users.Application)

- [ ] Create `IUserService` interface in Users.Application/Abstractions
- [ ] Mark interface as `public`
- [ ] Create `UserService` implementation (internal)
- [ ] Implement `UserExistsAsync` method with database query
- [ ] Register service in DependencyInjection.cs

### Phase 2: Error Type (WorkItems.Domain)

- [ ] Create `UserNotFoundError` record implementing `IError`
- [ ] Add factory method `Create(Guid userId)`
- [ ] Add XML documentation

### Phase 3: Integration (WorkItems.Application)

- [ ] Inject `IUserService` into `AssignWorkItem` constructor
- [ ] Add user existence check before assignment logic
- [ ] Return `UserNotFoundError` if user doesn't exist
- [ ] Update existing logic to proceed only if user exists

### Phase 4: Testing (Tests.WorkItems)

- [ ] Create unit test: `AssignWorkItem_WhenUserExists_ShouldSucceed`
- [ ] Create unit test: `AssignWorkItem_WhenUserDoesNotExist_ShouldReturnUserNotFoundError`
- [ ] Mock `IUserService` behavior in tests
- [ ] Create `ModuleBoundaryTests.cs` architecture tests
- [ ] Add test: WorkItems should not depend on Users internal types
- [ ] Verify all tests pass

### Phase 5: Documentation

- [ ] Add XML doc comments to `IUserService`
- [ ] Update README if necessary
- [ ] Mark PRD as completed

---

**Created:** 28. Oktober 2025  
**Status:** Ready for Implementation  
**Assignee:** TBD  
**Estimated Effort:** 2-3 hours
