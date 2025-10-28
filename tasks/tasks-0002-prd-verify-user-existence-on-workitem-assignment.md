# Tasks: Verify User Existence on WorkItem Assignment

## Relevant Files

### Users Module - Domain Layer

- `src/Users.Domain/Abstractions/IUserExistenceService.cs` - Domain service interface for checking user existence (internal to Users module)

### Users Module - Application Layer

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

- `src/WorkItems.Application/Adapters/UserExistenceAdapter.cs` - **Adapter** implementing `IUserExistenceChecker` and calling `IUserService`
- `src/WorkItems.Application/UseCases/AssignWorkItem/AssignWorkItemHandler.cs` - Update to use specification
- `src/WorkItems.Application/DependencyInjection.cs` - Register adapter and specification

### Test Files

- `tests/Tests.Users/TestFramework/InMemoryUsersDatabase.cs` - TestFramework implementation of `IUsersDatabase` (test double for moving parts)
- `tests/Tests.Users/Domain/UserExistenceServiceTests.cs` - Tests for user existence domain service (using TestFramework)
- `tests/Tests.WorkItems/Domain/Specifications/UserMustExistSpecificationTests.cs` - Tests for specification logic
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
- [ ] 2.0 Setup Users Module - Public API (IUserService) (TDD)
- [ ] 3.0 Setup WorkItems Module - Domain Layer (Port & Specification) (TDD)
- [ ] 4.0 Setup WorkItems Module - Domain Model Integration (WorkItem.AssignAsync) (TDD)
- [ ] 5.0 Setup WorkItems Module - Application Layer (Adapter, Use Case Update) (TDD)
- [ ] 6.0 Architecture Tests - Module Boundary Enforcement (TDD)
