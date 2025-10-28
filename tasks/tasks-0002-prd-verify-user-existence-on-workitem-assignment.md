# Tasks: Verify User Existence on WorkItem Assignment

## Relevant Files

### Users Module - Domain Layer

- `src/Users.Domain/Abstractions/IUserExistenceService.cs` - Domain service interface for checking user existence (internal to Users module)

### Users Module - Application Layer

- `src/Users.Application/Adapters/TestFramework/InMemoryUsersDatabase.cs` - TestFramework implementation of `IUsersDatabase` (test double for moving parts)
- `src/Users.Application/Abstractions/IUserService.cs` - **Public API** interface exposing user existence checking to other modules
- `src/Users.Application/Services/UserService.cs` - Implementation of `IUserService` delegating to domain service
- `src/Users.Application/DependencyInjection.cs` - Register `IUserService` implementation

### Users Module - Infrastructure Layer

- `src/Users.Infrastructure/Services/UserExistenceService.cs` - Implementation of `IUserExistenceService` querying database
- `src/Users.Infrastructure/DependencyInjection.cs` - Register `IUserExistenceService` implementation

### WorkItems Module - Domain Layer

- `src/WorkItems.Domain/Abstractions/IUserExistenceChecker.cs` - **Port** interface defining what WorkItems domain needs
- `src/WorkItems.Domain/Specifications/UserMustExistSpecification.cs` - Specification encapsulating "user must exist" business rule
- `src/WorkItems.Domain/Models/WorkItems/WorkItem.cs` - Update to add `AssignAsync` static factory method using specification
- **Note**: Use existing `SharedKernel.Models.Common.DomainError` with `DomainErrorFactory.NotFound()` for "user not found" errors (both modules have access)

### WorkItems Module - Application Layer

- `src/WorkItems.Application/Adapters/TestFramework/InMemoryWorkItemsDatabase.cs` - TestFramework implementation of `IWorkItemsDatabase` (test double for moving parts)
- `src/WorkItems.Application/Adapters/UserExistenceAdapter.cs` - **Adapter** implementing `IUserExistenceChecker` and calling `IUserService`
- `src/WorkItems.Application/UseCases/AssignWorkItem/AssignWorkItemHandler.cs` - Update to use specification
- `src/WorkItems.Application/DependencyInjection.cs` - Register adapter and specification

### Test Files

- `tests/Tests.Users/Domain/UserExistenceServiceTests.cs` - Tests for user existence domain service (using TestFramework from Application layer)
- `tests/Tests.WorkItems/Domain/Specifications/UserMustExistSpecificationTests.cs` - Tests for specification logic (using TestFramework from Application layer)
- `tests/Tests.WorkItems/Domain/Models/WorkItemTests.cs` - Tests for WorkItem.AssignAsync with specification
- `tests/Tests.WorkItems/Application/Adapters/UserExistenceAdapterTests.cs` - Tests for adapter translation (using TestFramework with real services)
- `tests/Tests.WorkItems/Application/UseCases/AssignWorkItemHandlerTests.cs` - Tests for use case orchestration (using TestFramework)
- `tests/Tests.WorkItems/Architecture/ModuleBoundaryTests.cs` - Architecture tests enforcing module boundaries

### Notes

- Following **TDD approach**: Write tests first, then implement to make tests pass
- Using **Hexagonal Architecture**: Port (IUserExistenceChecker) in domain, Adapter in application
- Using **Specification Pattern**: Encapsulates business rule in domain layer
- **Module boundaries**: WorkItems can only access Users.Application public types
- **Error Handling**: Use existing `SharedKernel.Models.Common.DomainError` and `DomainErrorFactory.NotFound(userId, typeof(User))` for domain errors
- **Application Layer**: Convert domain errors using `ApplicationErrorFactory.FromDomainError()` to `IError` for API responses
- **Testing Strategy**: Implement a TestFramework (Martin Fowler's CleanCode principles) in ApplicationLayer that provides test doubles for moving parts like `IDatabase`/`IUsersDatabase`, while using real implementations for all other components (services, use cases, adapters)
- Tests use **xUnit**, **Shouldly**, and **NetArchTest.Rules** (for architecture tests)
- All services registered as **Scoped** in DependencyInjection classes
- Follow existing patterns: Result<T, E> for error handling, factory methods for domain objects

## Tasks

- [ ] 1.0 Setup Users Module - Domain Service for User Existence Checking (TDD)
  - [ ] 1.1 **RED**: Create `InMemoryUsersDatabase.cs` TestFramework in `src/Users.Application/Adapters/TestFramework/` implementing `IUsersDatabase` for in-memory testing
  - [ ] 1.2 **RED**: Write failing test `UserExistenceService_ExistsAsync_WhenUserExists_ShouldReturnTrue` in `tests/Tests.Users/Domain/UserExistenceServiceTests.cs` (test will fail - service doesn't exist yet)
  - [ ] 1.3 **GREEN**: Create `IUserExistenceService.cs` interface in `src/Users.Domain/Abstractions/` with `ExistsAsync(Guid userId, CancellationToken)` method (internal)
  - [ ] 1.4 **GREEN**: Implement minimal `UserExistenceService.cs` in `src/Users.Infrastructure/Services/` to make test pass (use `IUsersDatabase.Query<UserEntity>()`)
  - [ ] 1.5 **GREEN**: Register `IUserExistenceService` → `UserExistenceService` as Scoped in `src/Users.Infrastructure/DependencyInjection.cs`
  - [ ] 1.6 **GREEN**: Run test - verify it passes
  - [ ] 1.7 **RED**: Write failing test `UserExistenceService_ExistsAsync_WhenUserDoesNotExist_ShouldReturnFalse`
  - [ ] 1.8 **GREEN**: Adjust implementation if needed to make test pass
  - [ ] 1.9 **REFACTOR**: Review and refactor service implementation for clarity and performance

- [ ] 2.0 Setup Users Module - Public API (IUserService) (TDD)
  - [ ] 2.1 **RED**: Write failing test `UserService_UserExistsAsync_WhenUserExists_ShouldReturnTrue` in `tests/Tests.Users/Application/UserServiceTests.cs` (test will fail - public API doesn't exist yet)
  - [ ] 2.2 **GREEN**: Create public interface `IUserService.cs` in `src/Users.Application/Abstractions/` with `UserExistsAsync(Guid userId, CancellationToken)` method (mark as `public`)
  - [ ] 2.3 **GREEN**: Create minimal `UserService.cs` implementation in `src/Users.Application/Services/` (internal sealed class) delegating to `IUserExistenceService`
  - [ ] 2.4 **GREEN**: Register `IUserService` → `UserService` as Scoped in `src/Users.Application/DependencyInjection.cs`
  - [ ] 2.5 **GREEN**: Run test - verify it passes
  - [ ] 2.6 **RED**: Write failing test `UserService_UserExistsAsync_WhenUserDoesNotExist_ShouldReturnFalse`
  - [ ] 2.7 **GREEN**: Verify implementation handles this case (should already pass)
  - [ ] 2.8 **REFACTOR**: Review and refactor for clarity if needed

- [ ] 3.0 Setup WorkItems Module - Domain Layer (Port & Specification) (TDD)
  - [ ] 3.1 **RED**: Create `InMemoryWorkItemsDatabase.cs` TestFramework in `src/WorkItems.Application/Adapters/TestFramework/` implementing `IWorkItemsDatabase` for in-memory testing
  - [ ] 3.2 **RED**: Write failing test `UserMustExistSpecification_IsSatisfiedByAsync_WhenUserExists_ShouldReturnTrue` in `tests/Tests.WorkItems/Domain/Specifications/UserMustExistSpecificationTests.cs` (using test double for `IUserExistenceChecker`)
  - [ ] 3.3 **GREEN**: Create Port interface `IUserExistenceChecker.cs` in `src/WorkItems.Domain/Abstractions/` with `ExistsAsync(AssigneeId userId, CancellationToken)` method (internal)
  - [ ] 3.4 **GREEN**: Create minimal `UserMustExistSpecification.cs` class in `src/WorkItems.Domain/Specifications/` accepting `IUserExistenceChecker` in constructor
  - [ ] 3.5 **GREEN**: Add `IsSatisfiedByAsync(AssigneeId userId, CancellationToken)` method that calls checker
  - [ ] 3.6 **GREEN**: Run test - verify it passes
  - [ ] 3.7 **RED**: Write failing test `UserMustExistSpecification_IsSatisfiedByAsync_WhenUserDoesNotExist_ShouldReturnFalse`
  - [ ] 3.8 **GREEN**: Verify implementation handles this case (should already pass)
  - [ ] 3.9 **REFACTOR**: Review specification for clarity and adherence to Specification Pattern principles

- [ ] 4.0 Setup WorkItems Module - Domain Model Integration (WorkItem.AssignAsync) (TDD)
  - [ ] 4.1 **RED**: Write failing test `WorkItem_AssignAsync_WhenUserExists_ShouldReturnSuccessWithWorkItem` in `tests/Tests.WorkItems/Domain/Models/WorkItemTests.cs`
  - [ ] 4.2 **GREEN**: Add static factory method `AssignAsync(WorkItemId id, AssigneeId assigneeId, Title title, Description description, Priority priority, UserMustExistSpecification spec, TimeProvider timeProvider, CancellationToken ct)` to `WorkItem` class
  - [ ] 4.3 **GREEN**: Implement minimal logic: call `spec.IsSatisfiedByAsync(assigneeId, ct)`, if true create WorkItem and return `Result.Ok<WorkItem, DomainError>(workItem)`
  - [ ] 4.4 **GREEN**: Run test - verify it passes
  - [ ] 4.5 **RED**: Write failing test `WorkItem_AssignAsync_WhenUserDoesNotExist_ShouldReturnNotFoundError`
  - [ ] 4.6 **GREEN**: Update `AssignAsync`: if spec not satisfied, return `Result.Fail<WorkItem, DomainError>(DomainErrorFactory.NotFound(assigneeId.Value, typeof(User)))`
  - [ ] 4.7 **GREEN**: Run test - verify it passes
  - [ ] 4.8 **REFACTOR**: Review domain method for clarity, ensure proper use of Result pattern and domain error handling

- [ ] 5.0 Setup WorkItems Module - Application Layer (Adapter, Use Case Update) (TDD)
  - [ ] 5.1 **RED**: Write failing test `UserExistenceAdapter_ExistsAsync_ShouldReturnTrueWhenUserExists` in `tests/Tests.WorkItems/Application/Adapters/UserExistenceAdapterTests.cs` using TestFramework
  - [ ] 5.2 **GREEN**: Create Adapter `UserExistenceAdapter.cs` in `src/WorkItems.Application/Adapters/` implementing `IUserExistenceChecker` (internal sealed class)
  - [ ] 5.3 **GREEN**: Inject `IUserService` from Users.Application into adapter constructor
  - [ ] 5.4 **GREEN**: Implement minimal `ExistsAsync(AssigneeId userId, CancellationToken)` translating `AssigneeId` to `Guid` and calling `IUserService.UserExistsAsync()`
  - [ ] 5.5 **GREEN**: Run test - verify it passes
  - [ ] 5.6 **RED**: Write failing test `UserExistenceAdapter_ExistsAsync_ShouldReturnFalseWhenUserDoesNotExist`
  - [ ] 5.7 **GREEN**: Verify implementation handles this case (should already pass)
  - [ ] 5.8 **RED**: Write failing test `UserExistenceAdapter_ExistsAsync_ShouldCallUserServiceWithCorrectGuid`
  - [ ] 5.9 **GREEN**: Verify translation logic is correct (should already pass)
  - [ ] 5.10 **GREEN**: Register `IUserExistenceChecker` → `UserExistenceAdapter` as Scoped in `src/WorkItems.Application/DependencyInjection.cs`
  - [ ] 5.11 **GREEN**: Register `UserMustExistSpecification` as Scoped in DependencyInjection
  - [ ] 5.12 **REFACTOR**: Review adapter for clarity
  - [ ] 5.13 **RED**: Write failing test `AssignWorkItemHandler_Handle_WhenUserExists_ShouldCreateAndPersistWorkItem` in `tests/Tests.WorkItems/Application/UseCases/AssignWorkItemHandlerTests.cs` using TestFramework
  - [ ] 5.14 **GREEN**: Update `AssignWorkItemHandler.cs` to inject `UserMustExistSpecification` and `TimeProvider`
  - [ ] 5.15 **GREEN**: Update handler to call `WorkItem.AssignAsync()` instead of direct creation
  - [ ] 5.16 **GREEN**: Run test - verify it passes
  - [ ] 5.17 **RED**: Write failing test `AssignWorkItemHandler_Handle_WhenUserDoesNotExist_ShouldReturnNotFoundError`
  - [ ] 5.18 **GREEN**: Verify handler properly propagates domain error (should already work via Result pattern)
  - [ ] 5.19 **GREEN**: Run test - verify it passes
  - [ ] 5.20 **REFACTOR**: Review use case handler for clarity and proper error handling

- [ ] 6.0 Architecture Tests - Module Boundary Enforcement (TDD)
  - [ ] 6.1 **RED**: Create `ModuleBoundaryTests.cs` in `tests/Tests.WorkItems/Architecture/`
  - [ ] 6.2 **RED**: Write failing test `WorkItemsApplication_ShouldOnlyAccessPublicTypesFrom_UsersApplication` using NetArchTest.Rules
  - [ ] 6.3 **GREEN**: Verify test passes (architecture should already be correct due to proper implementation)
  - [ ] 6.4 **RED**: Write failing test `WorkItemsDomain_ShouldNotHaveDependencyOn_UsersModule`
  - [ ] 6.5 **GREEN**: Verify test passes (domain should have no Users dependencies)
  - [ ] 6.6 **RED**: Write failing test `WorkItemsModule_ShouldNotAccess_UsersInfrastructure`
  - [ ] 6.7 **GREEN**: Verify test passes
  - [ ] 6.8 **RED**: Write failing test `WorkItemsModule_ShouldNotAccess_UsersDomainInternals`
  - [ ] 6.9 **GREEN**: Verify test passes
  - [ ] 6.10 **GREEN**: Run full test suite to ensure all tests pass and feature is complete
  - [ ] 6.11 **REFACTOR**: Review entire implementation for code quality, documentation, and adherence to clean code principles
