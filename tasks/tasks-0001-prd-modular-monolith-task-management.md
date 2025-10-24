# Task List: Modular Monolith Task Management System

Generated from: `0001-prd-modular-monolith-task-management.md`

## Relevant Files

### Solution and Project Files

- `JtlTask.slnx` - Solution file
- `src/JtlTask.WebApi/JtlTask.WebApi.csproj` - Web API host project (Presentation Layer)
- `src/JtlTask.WebApi/Program.cs` - Application entry point and DI configuration
- `src/Users/Domain/Domain.csproj` - User domain project
- `src/Users/Infrastructure/Infrastructure.csproj` - User infrastructure project
- `src/Users/Application/Application.csproj` - User application project (Commands, Queries, Handlers)
- `src/Tasks/Domain/Domain.csproj` - Task domain project
- `src/Tasks/Infrastructure/Infrastructure.csproj` - Task infrastructure project
- `src/Tasks/Application/Application.csproj` - Task application project (Commands, Queries, Handlers)
- `src/SharedKernel/SharedKernel.csproj` - Shared kernel (Result types, base classes)
- `tests/Test.JtlTask.WebApi/Test.JtlTask.WebApi.csproj` - WebApi integration tests
- `tests/Tests.Users/Tests.Users.csproj` - User module tests
- `tests/Tests.Tasks/Tests.Tasks.csproj` - Task module tests

### User Bounded Context - Domain

- `src/Users/Domain/User.cs` - User aggregate root entity
- `src/Users/Domain/Username.cs` - Username value object
- `src/Users/Domain/IUserRepository.cs` - User repository interface
- `src/Users/Domain/DomainErrors.cs` - Domain-specific error definitions

### User Bounded Context - Infrastructure

- `src/Users/Infrastructure/Persistence/InMemoryUserRepository.cs` - In-memory user repository implementation
- `src/Users/Infrastructure/DependencyInjection.cs` - Infrastructure DI registration

### User Bounded Context - Application

- `src/Users/Application/Commands/CreateUser/CreateUserCommand.cs` - Create user command
- `src/Users/Application/Commands/CreateUser/CreateUserHandler.cs` - Create user command handler
- `src/Users/Application/Queries/GetUserById/GetUserByIdQuery.cs` - Get user by ID query
- `src/Users/Application/Queries/GetUserById/GetUserByIdHandler.cs` - Get user by ID query handler
- `src/Users/Application/Queries/UserExists/UserExistsQuery.cs` - User exists query (public API for inter-module communication)
- `src/Users/Application/Queries/UserExists/UserExistsHandler.cs` - User exists query handler
- `src/Users/Application/DependencyInjection.cs` - Application DI registration

### User Module - Presentation Layer (in JtlTask.WebApi)

- `src/JtlTask.WebApi/Users/CreateUserEndpoint.cs` - POST /api/users endpoint
- `src/JtlTask.WebApi/Users/GetUserByIdEndpoint.cs` - GET /api/users/{id} endpoint
- `src/JtlTask.WebApi/Users/Contracts/CreateUserRequest.cs` - Create user request DTO
- `src/JtlTask.WebApi/Users/Contracts/CreateUserResponse.cs` - Create user response DTO
- `src/JtlTask.WebApi/Users/Contracts/GetUserByIdResponse.cs` - Get user response DTO
- `src/JtlTask.WebApi/Users/Validators/CreateUserValidator.cs` - FluentValidation validator for create user

### Task Bounded Context - Domain

- `src/Tasks/Domain/Task.cs` - Task aggregate root entity
- `src/Tasks/Domain/ITaskRepository.cs` - Task repository interface
- `src/Tasks/Domain/DomainErrors.cs` - Domain-specific error definitions

### Task Bounded Context - Infrastructure

- `src/Tasks/Infrastructure/Persistence/InMemoryTaskRepository.cs` - In-memory task repository implementation
- `src/Tasks/Infrastructure/DependencyInjection.cs` - Infrastructure DI registration

### Task Bounded Context - Application

- `src/Tasks/Application/Commands/CreateTask/CreateTaskCommand.cs` - Create task command
- `src/Tasks/Application/Commands/CreateTask/CreateTaskHandler.cs` - Create task command handler (uses Users.Application public API)
- `src/Tasks/Application/Queries/GetTasksByUserId/GetTasksByUserIdQuery.cs` - Get tasks by user ID query
- `src/Tasks/Application/Queries/GetTasksByUserId/GetTasksByUserIdHandler.cs` - Get tasks by user ID query handler
- `src/Tasks/Application/DependencyInjection.cs` - Application DI registration

### Task Module - Presentation Layer (in JtlTask.WebApi)

- `src/JtlTask.WebApi/Tasks/CreateTaskEndpoint.cs` - POST /api/tasks endpoint
- `src/JtlTask.WebApi/Tasks/GetTasksByUserIdEndpoint.cs` - GET /api/tasks/by-user/{userId} endpoint
- `src/JtlTask.WebApi/Tasks/Contracts/CreateTaskRequest.cs` - Create task request DTO
- `src/JtlTask.WebApi/Tasks/Contracts/CreateTaskResponse.cs` - Create task response DTO
- `src/JtlTask.WebApi/Tasks/Contracts/GetTasksByUserIdResponse.cs` - Get tasks response DTO
- `src/JtlTask.WebApi/Tasks/Validators/CreateTaskValidator.cs` - FluentValidation validator for create task

### Shared Kernel

- `src/SharedKernel/Models/Results/Result{T, E}.cs` - Result monad implementation
- `src/SharedKernel/Models/Unit.cs` - Unit type for void-like operations
- `src/SharedKernel/Extensions/Results/` - Extension methods for Result (Map, Bind, Match, etc.) with sync and async support
- `src/SharedKernel/IEntity.cs` - Base entity interface with Id property
- **Documentation**: [`.agent/docs/Result/result-monad-guide.md`](/.agent/docs/Result/result-monad-guide.md) - LLM-optimized Result Monad quick reference

### Tests

- `tests/Tests.Users/Domain/UserTests.cs` - User entity unit tests
- `tests/Tests.Users/Domain/UsernameTests.cs` - Username value object unit tests
- `tests/Tests.Users/Application/Commands/CreateUser/CreateUserHandlerTests.cs` - Create user handler unit tests
- `tests/Tests.Users/Application/Queries/GetUserById/GetUserByIdHandlerTests.cs` - Get user by ID handler unit tests
- `tests/Test.JtlTask.WebApi/Users/CreateUserEndpointTests.cs` - Create user endpoint integration tests
- `tests/Test.JtlTask.WebApi/Users/GetUserByIdEndpointTests.cs` - Get user by ID endpoint integration tests
- `tests/Tests.Tasks/Domain/TaskTests.cs` - Task entity unit tests
- `tests/Tests.Tasks/Application/Commands/CreateTask/CreateTaskHandlerTests.cs` - Create task handler unit tests
- `tests/Tests.Tasks/Application/Queries/GetTasksByUserId/GetTasksByUserIdHandlerTests.cs` - Get tasks by user ID handler unit tests
- `tests/Test.JtlTask.WebApi/Tasks/CreateTaskEndpointTests.cs` - Create task endpoint integration tests
- `tests/Test.JtlTask.WebApi/Tasks/GetTasksByUserIdEndpointTests.cs` - Get tasks by user ID endpoint integration tests

### Notes

- **Clean Architecture in Modular Monolith**: Each bounded context (Users, Tasks) follows Clean Architecture with layers: Domain → Application → Infrastructure. Presentation Layer (Endpoints, Validators, DTOs) is in `JtlTask.WebApi` organized by module.
- **Module Structure**:
  - `src/Users/`, `src/Tasks/` contain: Domain, Application, Infrastructure
  - `src/JtlTask.WebApi/Users/`, `src/JtlTask.WebApi/Tasks/` contain: Endpoints, Validators, DTOs (Presentation Layer)
- **Domain Layer**: Contains only business logic. No format validation (e.g., string length) - this is done in Presentation Layer via FluentValidation. Domain validates business rules only (e.g., uniqueness).
- **Inter-Module Communication**: Modules communicate via public API of Application layer (e.g., Tasks.Application uses Users.Application's `UserExistsQuery`)
- **Dependencies**: Domain has no dependencies → Application depends on Domain → Infrastructure depends on Domain and Application → WebApi depends on all
- Tests are organized in `tests/` directory: `Tests.Users` and `Tests.Tasks` for unit tests, `Test.JtlTask.WebApi` for integration tests
- Use `dotnet test` to run all tests, or `dotnet test tests/Tests.Users` to run specific module tests

## Tasks

- [x] 1.0 Setup Solution Structure and Foundation (src/Projects with Bounded Contexts)
  - [x] 1.1 Install FastContracts Template Pack: `dotnet new install FastContracts.TemplatePack`
  - [x] 1.2 Create solution file `JtlTask.slnx` using `dotnet new sln -n JtlTask`
  - [x] 1.3 Create directory structure: `src/Users`, `src/Tasks`, `src/SharedKernel`, `tests/Tests.Users`, `tests/Tests.Tasks`, `tests/Test.JtlTask.WebApi`
  - [x] 1.4 Create `JtlTask.WebApi` project using `dotnet new feproj -n JtlTask.WebApi` in `src/JtlTask.WebApi`
  - [x] 1.5 Create `SharedKernel` class library in `src/SharedKernel`
  - [x] 1.6 Create User module projects in `src/Users/`: `Domain`, `Infrastructure`, `Application` (NO Contracts - that goes in WebApi)
  - [x] 1.7 Create Task module projects in `src/Tasks/`: `Domain`, `Infrastructure`, `Application` (NO Contracts - that goes in WebApi)
  - [x] 1.8 Create test projects: `tests/Tests.Users`, `tests/Tests.Tasks`, `tests/Test.JtlTask.WebApi` (xUnit)
  - [x] 1.9 Add all projects to solution using `dotnet sln add`
  - [x] 1.10 Configure project references following Clean Architecture rules: Domain (no deps) ← Application ← Infrastructure ← WebApi
  - [x] 1.11 Add provided Result monad implementation to SharedKernel
  - [x] 1.12 Add base interfaces/classes in SharedKernel (`Entity.cs`)
  - [x] 1.13 Verify solution builds successfully with `dotnet build`

- [ ] 2.0 Implement User Bounded Context - Domain, Infrastructure, and Application
  - [ ] 2.1 **Domain Layer**: Create `Username` value object (business logic only - NO format validation)
  - [ ] 2.2 **Domain Layer**: Write unit tests for `Username` value object
  - [ ] 2.3 **Domain Layer**: Create `User` aggregate root with private constructor and static `Create` factory method
  - [ ] 2.4 **Domain Layer**: Implement business rule validation in `User.Create` (business rules only, not format validation)
  - [ ] 2.5 **Domain Layer**: Write unit tests for `User` entity creation
  - [ ] 2.6 **Domain Layer**: Define `IUserRepository` interface with methods: `GetByIdAsync`, `ExistsByUsernameAsync`, `AddAsync`
  - [ ] 2.7 **Domain Layer**: Create `DomainErrors.cs` with user-specific error definitions
  - [ ] 2.8 **Infrastructure Layer**: Implement `InMemoryUserRepository` using `ConcurrentDictionary<Guid, User>`
  - [ ] 2.9 **Infrastructure Layer**: Create `DependencyInjection.cs` extension method to register repository
  - [ ] 2.10 **Application Layer**: Create `CreateUserCommand` record with Username property
  - [ ] 2.11 **Application Layer**: Create `CreateUserHandler` implementing `ICommandHandler<CreateUserCommand, Result<User>>`
  - [ ] 2.12 **Application Layer**: Implement handler logic: check uniqueness via repository, create user, save to repository
  - [ ] 2.13 **Application Layer**: Write unit tests for `CreateUserHandler` (success case, duplicate username)
  - [ ] 2.14 **Application Layer**: Create `GetUserByIdQuery` record with UserId property
  - [ ] 2.15 **Application Layer**: Create `GetUserByIdHandler` implementing `ICommandHandler<GetUserByIdQuery, Result<User>>`
  - [ ] 2.16 **Application Layer**: Write unit tests for `GetUserByIdHandler` (success case, user not found)
  - [ ] 2.17 **Application Layer**: Create `UserExistsQuery` record with UserId property (public API for inter-module communication)
  - [ ] 2.18 **Application Layer**: Create `UserExistsHandler` implementing `ICommandHandler<UserExistsQuery, Result<bool>>`
  - [ ] 2.19 **Application Layer**: Create `DependencyInjection.cs` to register command handlers

- [ ] 3.0 Implement User Module - Presentation Layer in JtlTask.WebApi
  - [ ] 3.1 **Presentation Layer**: Create `src/JtlTask.WebApi/Users/Contracts/CreateUserRequest.cs` DTO record
  - [ ] 3.2 **Presentation Layer**: Create `src/JtlTask.WebApi/Users/Contracts/CreateUserResponse.cs` DTO record
  - [ ] 3.3 **Presentation Layer**: Create `src/JtlTask.WebApi/Users/Validators/CreateUserValidator.cs` inheriting from `Validator<CreateUserRequest>`
  - [ ] 3.4 **Presentation Layer**: Implement validation rules in CreateUserValidator: Username required, 3-50 chars, alphanumeric + underscore/hyphen
  - [ ] 3.5 **Presentation Layer**: Create `src/JtlTask.WebApi/Users/CreateUserEndpoint.cs` inheriting from `Endpoint<CreateUserRequest, CreateUserResponse>`
  - [ ] 3.6 **Presentation Layer**: Configure endpoint: POST /api/users, returns 201 Created or 400 Bad Request
  - [ ] 3.7 **Presentation Layer**: Implement endpoint handler: send CreateUserCommand to Application layer, map Result to HTTP response
  - [ ] 3.8 **Presentation Layer**: Create `src/JtlTask.WebApi/Users/Contracts/GetUserByIdResponse.cs` DTO record
  - [ ] 3.9 **Presentation Layer**: Create `src/JtlTask.WebApi/Users/GetUserByIdEndpoint.cs` inheriting from `Endpoint<EmptyRequest, GetUserByIdResponse>`
  - [ ] 3.10 **Presentation Layer**: Configure endpoint: GET /api/users/{id}, returns 200 OK or 404 Not Found
  - [ ] 3.11 **Presentation Layer**: Implement endpoint handler: send GetUserByIdQuery to Application layer, map Result to HTTP response
  - [ ] 3.12 **Integration Tests**: Write test for CreateUserEndpoint in `tests/Test.JtlTask.WebApi/Users/CreateUserEndpointTests.cs` (success case)
  - [ ] 3.13 **Integration Tests**: Write test for CreateUserEndpoint (validation failure - format)
  - [ ] 3.14 **Integration Tests**: Write test for CreateUserEndpoint (duplicate username - business rule)
  - [ ] 3.15 **Integration Tests**: Write test for GetUserByIdEndpoint (success case)
  - [ ] 3.16 **Integration Tests**: Write test for GetUserByIdEndpoint (user not found - 404)

- [ ] 4.0 Implement Task Bounded Context - Domain, Infrastructure, and Application
  - [ ] 4.1 **Domain Layer**: Create `Task` aggregate root with properties: Id, Name, AssigneeId
  - [ ] 4.2 **Domain Layer**: Implement static `Create` factory method (business logic only - NO format validation)
  - [ ] 4.3 **Domain Layer**: Write unit tests for `Task` entity creation
  - [ ] 4.4 **Domain Layer**: Define `ITaskRepository` interface with methods: `GetByAssigneeIdAsync`, `AddAsync`
  - [ ] 4.5 **Domain Layer**: Create `DomainErrors.cs` with task-specific error definitions
  - [ ] 4.6 **Infrastructure Layer**: Implement `InMemoryTaskRepository` using `ConcurrentDictionary<Guid, Task>` or `List<Task>`
  - [ ] 4.7 **Infrastructure Layer**: Create `DependencyInjection.cs` extension method to register repository
  - [ ] 4.8 **Application Layer**: Create `CreateTaskCommand` record with Name and AssigneeId properties
  - [ ] 4.9 **Application Layer**: Create `CreateTaskHandler` implementing `ICommandHandler<CreateTaskCommand, Result<Task>>`
  - [ ] 4.10 **Application Layer**: Add project reference to Users.Application to access UserExistsQuery (inter-module communication via public API)
  - [ ] 4.11 **Application Layer**: Implement handler logic: use UserExistsQuery to verify assignee exists, create task, save to repository
  - [ ] 4.12 **Application Layer**: Write unit tests for `CreateTaskHandler` (success case, assignee not found)
  - [ ] 4.13 **Application Layer**: Create `GetTasksByUserIdQuery` record with UserId property
  - [ ] 4.14 **Application Layer**: Create `GetTasksByUserIdHandler` implementing `ICommandHandler<GetTasksByUserIdQuery, Result<List<Task>>>`
  - [ ] 4.15 **Application Layer**: Implement handler logic: use UserExistsQuery to verify user exists, get tasks (return empty list if none)
  - [ ] 4.16 **Application Layer**: Write unit tests for `GetTasksByUserIdHandler` (success with tasks, success with empty list, user not found)
  - [ ] 4.17 **Application Layer**: Create `DependencyInjection.cs` to register command handlers

- [ ] 5.0 Implement Task Module - Presentation Layer in JtlTask.WebApi
  - [ ] 5.1 **Presentation Layer**: Create `src/JtlTask.WebApi/Tasks/Contracts/CreateTaskRequest.cs` DTO record
  - [ ] 5.2 **Presentation Layer**: Create `src/JtlTask.WebApi/Tasks/Contracts/CreateTaskResponse.cs` DTO record
  - [ ] 5.3 **Presentation Layer**: Create `src/JtlTask.WebApi/Tasks/Validators/CreateTaskValidator.cs` inheriting from `Validator<CreateTaskRequest>`
  - [ ] 5.4 **Presentation Layer**: Implement validation rules in CreateTaskValidator: Name required (3-200 chars), AssigneeId required and valid GUID format
  - [ ] 5.5 **Presentation Layer**: Create `src/JtlTask.WebApi/Tasks/CreateTaskEndpoint.cs` inheriting from `Endpoint<CreateTaskRequest, CreateTaskResponse>`
  - [ ] 5.6 **Presentation Layer**: Configure endpoint: POST /api/tasks, returns 201 Created or 400 Bad Request
  - [ ] 5.7 **Presentation Layer**: Implement endpoint handler: send CreateTaskCommand to Application layer, map Result to HTTP response
  - [ ] 5.8 **Presentation Layer**: Create `src/JtlTask.WebApi/Tasks/Contracts/GetTasksByUserIdResponse.cs` DTO record with Tasks list
  - [ ] 5.9 **Presentation Layer**: Create `src/JtlTask.WebApi/Tasks/GetTasksByUserIdEndpoint.cs` inheriting from `Endpoint<EmptyRequest, GetTasksByUserIdResponse>`
  - [ ] 5.10 **Presentation Layer**: Configure endpoint: GET /api/tasks/by-user/{userId}, returns 200 OK or 404 Not Found
  - [ ] 5.11 **Presentation Layer**: Implement endpoint handler: send GetTasksByUserIdQuery to Application layer, map Result to HTTP response
  - [ ] 5.12 **Integration Tests**: Write test for CreateTaskEndpoint in `tests/Test.JtlTask.WebApi/Tasks/CreateTaskEndpointTests.cs` (success case)
  - [ ] 5.13 **Integration Tests**: Write test for CreateTaskEndpoint (validation failure - format)
  - [ ] 5.14 **Integration Tests**: Write test for CreateTaskEndpoint (assignee not found - business rule)
  - [ ] 5.15 **Integration Tests**: Write test for GetTasksByUserIdEndpoint (success with tasks)
  - [ ] 5.16 **Integration Tests**: Write test for GetTasksByUserIdEndpoint (success with empty list)
  - [ ] 5.17 **Integration Tests**: Write test for GetTasksByUserIdEndpoint (user not found - 404)

- [ ] 6.0 Configure Cross-Cutting Concerns and Application Host
  - [ ] 6.1 Configure FastContracts in `Program.cs` with `builder.Services.AddFastContracts()`
  - [ ] 6.2 Configure FastContracts middleware in `Program.cs` with `app.UseFastContracts()`
  - [ ] 6.3 Register User module services in `Program.cs`: Users.Domain, Users.Infrastructure, Users.Application
  - [ ] 6.4 Register Task module services in `Program.cs`: Tasks.Domain, Tasks.Infrastructure, Tasks.Application
  - [ ] 6.5 Note: Presentation layer (Endpoints, Validators) are auto-discovered by FastContracts in JtlTask.WebApi
  - [ ] 6.6 Configure Problem Details error responses in FastContracts
  - [ ] 6.7 Configure Swagger/OpenAPI (optional but recommended for testing)
  - [ ] 6.8 Remove default WeatherForecast endpoint if present
  - [ ] 6.9 Run all unit tests and verify they pass (`dotnet test tests/Tests.Users tests/Tests.Tasks`)
  - [ ] 6.10 Run all integration tests and verify they pass (`dotnet test tests/Test.JtlTask.WebApi`)
  - [ ] 6.11 Run the application and test all endpoints manually or with Swagger
  - [ ] 6.12 Create `README.md` with setup instructions, architecture overview, and how to run
  - [ ] 6.13 Create `.gitignore` file for .NET projects
  - [ ] 6.14 Final verification: Check Clean Architecture dependencies (Domain ← Application ← Infrastructure ← WebApi)
  - [ ] 6.15 Final verification: Ensure modules are properly isolated (Tasks.Application can reference Users.Application, but NOT Domain/Infrastructure cross-references)
  - [ ] 6.16 Final code review: DDD patterns (Aggregates, Value Objects), CQRS implementation, Repository pattern, Result pattern usage

