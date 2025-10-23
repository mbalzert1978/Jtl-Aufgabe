# Task List: Modular Monolith Task Management System

Generated from: `0001-prd-modular-monolith-task-management.md`

## Relevant Files

### Solution and Project Files

- `JtlTask.sln` - Solution file
- `src/JtlTask.WebApi/JtlTask.WebApi.csproj` - Web API host project
- `src/JtlTask.WebApi/Program.cs` - Application entry point and DI configuration
- `src/Users/Domain/JtlTask.Users.Domain.csproj` - User domain project
- `src/Users/Infrastructure/JtlTask.Users.Infrastructure.csproj` - User infrastructure project
- `src/Users/UseCases/JtlTask.Users.UseCases.csproj` - User use cases project
- `src/Users/Endpoints/JtlTask.Users.Endpoints.csproj` - User endpoints project
- `src/Tasks/Domain/JtlTask.Tasks.Domain.csproj` - Task domain project
- `src/Tasks/Infrastructure/JtlTask.Tasks.Infrastructure.csproj` - Task infrastructure project
- `src/Tasks/UseCases/JtlTask.Tasks.UseCases.csproj` - Task use cases project
- `src/Tasks/Endpoints/JtlTask.Tasks.Endpoints.csproj` - Task endpoints project
- `src/Shared/JtlTask.Shared.Kernel/JtlTask.Shared.Kernel.csproj` - Shared kernel (Result types, base classes)
- `tests/Tests.Users/JtlTask.Tests.Users.csproj` - User module tests
- `tests/Tests.Tasks/JtlTask.Tests.Tasks.csproj` - Task module tests

### User Bounded Context - Domain

- `src/Users/Domain/User.cs` - User aggregate root entity
- `src/Users/Domain/Username.cs` - Username value object
- `src/Users/Domain/IUserRepository.cs` - User repository interface
- `src/Users/Domain/DomainErrors.cs` - Domain-specific error definitions

### User Bounded Context - Infrastructure

- `src/Users/Infrastructure/Persistence/InMemoryUserRepository.cs` - In-memory user repository implementation
- `src/Users/Infrastructure/DependencyInjection.cs` - Infrastructure DI registration

### User Bounded Context - UseCases

- `src/Users/UseCases/Commands/CreateUser/CreateUserCommand.cs` - Create user command
- `src/Users/UseCases/Commands/CreateUser/CreateUserHandler.cs` - Create user command handler
- `src/Users/UseCases/Queries/GetUserById/GetUserByIdQuery.cs` - Get user by ID query
- `src/Users/UseCases/Queries/GetUserById/GetUserByIdHandler.cs` - Get user by ID query handler
- `src/Users/UseCases/Queries/UserExists/UserExistsQuery.cs` - User exists query (for inter-context communication)
- `src/Users/UseCases/Queries/UserExists/UserExistsHandler.cs` - User exists query handler
- `src/Users/UseCases/DependencyInjection.cs` - UseCases DI registration

### User Bounded Context - Endpoints

- `src/Users/Endpoints/CreateUserEndpoint.cs` - POST /api/users endpoint
- `src/Users/Endpoints/GetUserByIdEndpoint.cs` - GET /api/users/{id} endpoint
- `src/Users/Endpoints/Contracts/CreateUserRequest.cs` - Create user request DTO
- `src/Users/Endpoints/Contracts/CreateUserResponse.cs` - Create user response DTO
- `src/Users/Endpoints/Contracts/GetUserByIdResponse.cs` - Get user response DTO
- `src/Users/Endpoints/Validators/CreateUserValidator.cs` - FluentValidation validator for create user
- `src/Users/Endpoints/DependencyInjection.cs` - Endpoints DI registration

### Task Bounded Context - Domain

- `src/Tasks/Domain/Task.cs` - Task aggregate root entity
- `src/Tasks/Domain/ITaskRepository.cs` - Task repository interface
- `src/Tasks/Domain/DomainErrors.cs` - Domain-specific error definitions

### Task Bounded Context - Infrastructure

- `src/Tasks/Infrastructure/Persistence/InMemoryTaskRepository.cs` - In-memory task repository implementation
- `src/Tasks/Infrastructure/DependencyInjection.cs` - Infrastructure DI registration

### Task Bounded Context - UseCases

- `src/Tasks/UseCases/Commands/CreateTask/CreateTaskCommand.cs` - Create task command
- `src/Tasks/UseCases/Commands/CreateTask/CreateTaskHandler.cs` - Create task command handler
- `src/Tasks/UseCases/Queries/GetTasksByUserId/GetTasksByUserIdQuery.cs` - Get tasks by user ID query
- `src/Tasks/UseCases/Queries/GetTasksByUserId/GetTasksByUserIdHandler.cs` - Get tasks by user ID query handler
- `src/Tasks/UseCases/DependencyInjection.cs` - UseCases DI registration

### Task Bounded Context - Endpoints

- `src/Tasks/Endpoints/CreateTaskEndpoint.cs` - POST /api/tasks endpoint
- `src/Tasks/Endpoints/GetTasksByUserIdEndpoint.cs` - GET /api/tasks/by-user/{userId} endpoint
- `src/Tasks/Endpoints/Contracts/CreateTaskRequest.cs` - Create task request DTO
- `src/Tasks/Endpoints/Contracts/CreateTaskResponse.cs` - Create task response DTO
- `src/Tasks/Endpoints/Contracts/GetTasksByUserIdResponse.cs` - Get tasks response DTO
- `src/Tasks/Endpoints/Validators/CreateTaskValidator.cs` - FluentValidation validator for create task
- `src/Tasks/Endpoints/DependencyInjection.cs` - Endpoints DI registration

### Shared Kernel

- `src/Shared/JtlTask.Shared.Kernel/Result.cs` - Result monad implementation
- `src/Shared/JtlTask.Shared.Kernel/Error.cs` - Error type for Result pattern
- `src/Shared/JtlTask.Shared.Kernel/IEntity.cs` - Base entity interface

### Tests

- `tests/Tests.Users/Domain/UserTests.cs` - User entity unit tests
- `tests/Tests.Users/Domain/UsernameTests.cs` - Username value object unit tests
- `tests/Tests.Users/UseCases/Commands/CreateUser/CreateUserHandlerTests.cs` - Create user handler unit tests
- `tests/Tests.Users/UseCases/Queries/GetUserById/GetUserByIdHandlerTests.cs` - Get user by ID handler unit tests
- `tests/Tests.Users/Endpoints/CreateUserEndpointTests.cs` - Create user endpoint integration tests
- `tests/Tests.Users/Endpoints/GetUserByIdEndpointTests.cs` - Get user by ID endpoint integration tests
- `tests/Tests.Tasks/Domain/TaskTests.cs` - Task entity unit tests
- `tests/Tests.Tasks/UseCases/Commands/CreateTask/CreateTaskHandlerTests.cs` - Create task handler unit tests
- `tests/Tests.Tasks/UseCases/Queries/GetTasksByUserId/GetTasksByUserIdHandlerTests.cs` - Get tasks by user ID handler unit tests
- `tests/Tests.Tasks/Endpoints/CreateTaskEndpointTests.cs` - Create task endpoint integration tests
- `tests/Tests.Tasks/Endpoints/GetTasksByUserIdEndpointTests.cs` - Get tasks by user ID endpoint integration tests

### Notes

- Each bounded context (Users, Tasks) is organized into separate projects: Domain, Infrastructure, UseCases, and Endpoints under `src/Users/` and `src/Tasks/`
- The Domain layer has no external dependencies
- Inter-context communication happens through UseCases layer (e.g., Task module uses UserExistsQuery from User module)
- Tests are organized in `tests/` directory and mirror the source structure (e.g., `src/Users/Domain/...` → `tests/Tests.Users/Domain/...`)
- Use `dotnet test` to run all tests, or `dotnet test tests/Tests.Users` to run specific module tests

## Tasks

- [ ] 1.0 Setup Solution Structure and Foundation (src/Projects with Bounded Contexts)
  - [ ] 1.1 Install FastEndpoints Template Pack: `dotnet new install FastEndpoints.TemplatePack`
  - [ ] 1.2 Create solution file `JtlTask.sln` using `dotnet new sln -n JtlTask`
  - [ ] 1.3 Create directory structure: `src/Users`, `src/Tasks`, `src/Shared`, `tests/Tests.Users`, `tests/Tests.Tasks`
  - [ ] 1.4 Create `JtlTask.WebApi` project using `dotnet new feproj -n JtlTask.WebApi` in `src/JtlTask.WebApi`
  - [ ] 1.5 Create `JtlTask.Shared.Kernel` class library in `src/Shared/JtlTask.Shared.Kernel`
  - [ ] 1.6 Create User module projects in `src/Users/`: `Domain/JtlTask.Users.Domain`, `Infrastructure/JtlTask.Users.Infrastructure`, `UseCases/JtlTask.Users.UseCases`, `Endpoints/JtlTask.Users.Endpoints`
  - [ ] 1.7 Create Task module projects in `src/Tasks/`: `Domain/JtlTask.Tasks.Domain`, `Infrastructure/JtlTask.Tasks.Infrastructure`, `UseCases/JtlTask.Tasks.UseCases`, `Endpoints/JtlTask.Tasks.Endpoints`
  - [ ] 1.8 Create test projects: `tests/Tests.Users/JtlTask.Tests.Users.csproj`, `tests/Tests.Tasks/JtlTask.Tests.Tasks.csproj` (xUnit)
  - [ ] 1.9 Add all projects to solution using `dotnet sln add`
  - [ ] 1.10 Configure project references following Clean Architecture rules (Domain → UseCases → Infrastructure → Endpoints → WebApi)
  - [ ] 1.11 Install NuGet packages: FastEndpoints, FastEndpoints.Swagger in WebApi and Endpoints projects (may already be included from template)
  - [ ] 1.12 Add provided Result monad implementation to Shared.Kernel (will be provided by user)
  - [ ] 1.13 Add base interfaces/classes in Shared.Kernel (`IEntity.cs`)
  - [ ] 1.14 Verify solution builds successfully with `dotnet build`

- [ ] 2.0 Implement User Bounded Context - Domain, Infrastructure, and UseCases
  - [ ] 2.1 **Domain Layer**: Create `Username` value object with validation (3-50 chars, alphanumeric + underscore/hyphen)
  - [ ] 2.2 **Domain Layer**: Write unit tests for `Username` value object
  - [ ] 2.3 **Domain Layer**: Create `User` aggregate root with private constructor and static `Create` factory method
  - [ ] 2.4 **Domain Layer**: Implement business rule: Username uniqueness check (delegated to repository)
  - [ ] 2.5 **Domain Layer**: Write unit tests for `User` entity creation and validation
  - [ ] 2.6 **Domain Layer**: Define `IUserRepository` interface with methods: `GetByIdAsync`, `ExistsByUsernameAsync`, `AddAsync`
  - [ ] 2.7 **Domain Layer**: Create `DomainErrors.cs` with user-specific error definitions
  - [ ] 2.8 **Infrastructure Layer**: Implement `InMemoryUserRepository` using `ConcurrentDictionary<Guid, User>`
  - [ ] 2.9 **Infrastructure Layer**: Create `DependencyInjection.cs` extension method to register repository
  - [ ] 2.10 **UseCases Layer**: Create `CreateUserCommand` record with Username property
  - [ ] 2.11 **UseCases Layer**: Create `CreateUserHandler` implementing `ICommandHandler<CreateUserCommand, Result<User>>`
  - [ ] 2.12 **UseCases Layer**: Implement handler logic: validate uniqueness, create user, save to repository
  - [ ] 2.13 **UseCases Layer**: Write unit tests for `CreateUserHandler` (success case, duplicate username)
  - [ ] 2.14 **UseCases Layer**: Create `GetUserByIdQuery` record with UserId property
  - [ ] 2.15 **UseCases Layer**: Create `GetUserByIdHandler` implementing `ICommandHandler<GetUserByIdQuery, Result<User>>`
  - [ ] 2.16 **UseCases Layer**: Write unit tests for `GetUserByIdHandler` (success case, user not found)
  - [ ] 2.17 **UseCases Layer**: Create `UserExistsQuery` record with UserId property (for inter-context communication)
  - [ ] 2.18 **UseCases Layer**: Create `UserExistsHandler` implementing `ICommandHandler<UserExistsQuery, Result<bool>>`
  - [ ] 2.19 **UseCases Layer**: Create `DependencyInjection.cs` to register command handlers

- [ ] 3.0 Implement User Bounded Context - Application Services (Inter-Context API) and Web Endpoints
  - [ ] 3.1 **Endpoints Layer**: Create `CreateUserRequest` DTO record
  - [ ] 3.2 **Endpoints Layer**: Create `CreateUserResponse` DTO record
  - [ ] 3.3 **Endpoints Layer**: Create `CreateUserValidator` inheriting from `Validator<CreateUserRequest>`
  - [ ] 3.4 **Endpoints Layer**: Implement validation rules: Username required, 3-50 chars, allowed characters
  - [ ] 3.5 **Endpoints Layer**: Create `CreateUserEndpoint` inheriting from `Endpoint<CreateUserRequest, CreateUserResponse>`
  - [ ] 3.6 **Endpoints Layer**: Configure endpoint: POST /api/users, returns 201 Created or 400 Bad Request
  - [ ] 3.7 **Endpoints Layer**: Implement endpoint handler: send CreateUserCommand, map Result to HTTP response
  - [ ] 3.8 **Endpoints Layer**: Create `GetUserByIdResponse` DTO record
  - [ ] 3.9 **Endpoints Layer**: Create `GetUserByIdEndpoint` inheriting from `Endpoint<EmptyRequest, GetUserByIdResponse>`
  - [ ] 3.10 **Endpoints Layer**: Configure endpoint: GET /api/users/{id}, returns 200 OK or 404 Not Found
  - [ ] 3.11 **Endpoints Layer**: Implement endpoint handler: send GetUserByIdQuery, map Result to HTTP response
  - [ ] 3.12 **Endpoints Layer**: Create `DependencyInjection.cs` to register endpoints
  - [ ] 3.13 **Integration Tests**: Write test for CreateUserEndpoint (success case)
  - [ ] 3.14 **Integration Tests**: Write test for CreateUserEndpoint (validation failure)
  - [ ] 3.15 **Integration Tests**: Write test for CreateUserEndpoint (duplicate username)
  - [ ] 3.16 **Integration Tests**: Write test for GetUserByIdEndpoint (success case)
  - [ ] 3.17 **Integration Tests**: Write test for GetUserByIdEndpoint (user not found - 404)

- [ ] 4.0 Implement Task Bounded Context - Domain, Infrastructure, and UseCases
  - [ ] 4.1 **Domain Layer**: Create `Task` aggregate root with properties: Id, Name, AssigneeId
  - [ ] 4.2 **Domain Layer**: Implement static `Create` factory method with validation (Name 3-200 chars, AssigneeId required)
  - [ ] 4.3 **Domain Layer**: Write unit tests for `Task` entity creation and validation
  - [ ] 4.4 **Domain Layer**: Define `ITaskRepository` interface with methods: `GetByAssigneeIdAsync`, `AddAsync`
  - [ ] 4.5 **Domain Layer**: Create `DomainErrors.cs` with task-specific error definitions
  - [ ] 4.6 **Infrastructure Layer**: Implement `InMemoryTaskRepository` using `ConcurrentDictionary<Guid, Task>` or `List<Task>`
  - [ ] 4.7 **Infrastructure Layer**: Create `DependencyInjection.cs` extension method to register repository
  - [ ] 4.8 **UseCases Layer**: Create `CreateTaskCommand` record with Name and AssigneeId properties
  - [ ] 4.9 **UseCases Layer**: Create `CreateTaskHandler` implementing `ICommandHandler<CreateTaskCommand, Result<Task>>`
  - [ ] 4.10 **UseCases Layer**: Inject `UserExistsQuery` handler from User module (inter-context communication)
  - [ ] 4.11 **UseCases Layer**: Implement handler logic: verify assignee exists, create task, save to repository
  - [ ] 4.12 **UseCases Layer**: Write unit tests for `CreateTaskHandler` (success case, assignee not found)
  - [ ] 4.13 **UseCases Layer**: Create `GetTasksByUserIdQuery` record with UserId property
  - [ ] 4.14 **UseCases Layer**: Create `GetTasksByUserIdHandler` implementing `ICommandHandler<GetTasksByUserIdQuery, Result<List<Task>>>`
  - [ ] 4.15 **UseCases Layer**: Implement handler logic: verify user exists, get tasks (return empty list if none)
  - [ ] 4.16 **UseCases Layer**: Write unit tests for `GetTasksByUserIdHandler` (success with tasks, success with empty list, user not found)
  - [ ] 4.17 **UseCases Layer**: Create `DependencyInjection.cs` to register command handlers

- [ ] 5.0 Implement Task Bounded Context - Application Services (Inter-Context API) and Web Endpoints
  - [ ] 5.1 **Endpoints Layer**: Create `CreateTaskRequest` DTO record
  - [ ] 5.2 **Endpoints Layer**: Create `CreateTaskResponse` DTO record
  - [ ] 5.3 **Endpoints Layer**: Create `CreateTaskValidator` inheriting from `Validator<CreateTaskRequest>`
  - [ ] 5.4 **Endpoints Layer**: Implement validation rules: Name required (3-200 chars), AssigneeId required and valid GUID
  - [ ] 5.5 **Endpoints Layer**: Create `CreateTaskEndpoint` inheriting from `Endpoint<CreateTaskRequest, CreateTaskResponse>`
  - [ ] 5.6 **Endpoints Layer**: Configure endpoint: POST /api/tasks, returns 201 Created or 400 Bad Request
  - [ ] 5.7 **Endpoints Layer**: Implement endpoint handler: send CreateTaskCommand, map Result to HTTP response
  - [ ] 5.8 **Endpoints Layer**: Create `GetTasksByUserIdResponse` DTO record with Tasks list
  - [ ] 5.9 **Endpoints Layer**: Create `GetTasksByUserIdEndpoint` inheriting from `Endpoint<EmptyRequest, GetTasksByUserIdResponse>`
  - [ ] 5.10 **Endpoints Layer**: Configure endpoint: GET /api/tasks/by-user/{userId}, returns 200 OK or 404 Not Found
  - [ ] 5.11 **Endpoints Layer**: Implement endpoint handler: send GetTasksByUserIdQuery, map Result to HTTP response
  - [ ] 5.12 **Endpoints Layer**: Create `DependencyInjection.cs` to register endpoints
  - [ ] 5.13 **Integration Tests**: Write test for CreateTaskEndpoint (success case)
  - [ ] 5.14 **Integration Tests**: Write test for CreateTaskEndpoint (validation failure)
  - [ ] 5.15 **Integration Tests**: Write test for CreateTaskEndpoint (assignee not found)
  - [ ] 5.16 **Integration Tests**: Write test for GetTasksByUserIdEndpoint (success with tasks)
  - [ ] 5.17 **Integration Tests**: Write test for GetTasksByUserIdEndpoint (success with empty list)
  - [ ] 5.18 **Integration Tests**: Write test for GetTasksByUserIdEndpoint (user not found - 404)

- [ ] 6.0 Configure Cross-Cutting Concerns and Application Host
  - [ ] 6.1 Configure FastEndpoints in `Program.cs` with `builder.Services.AddFastEndpoints()`
  - [ ] 6.2 Configure FastEndpoints middleware in `Program.cs` with `app.UseFastEndpoints()`
  - [ ] 6.3 Register User module services in `Program.cs` (Domain, Infrastructure, UseCases, Endpoints)
  - [ ] 6.4 Register Task module services in `Program.cs` (Domain, Infrastructure, UseCases, Endpoints)
  - [ ] 6.5 Configure Problem Details error responses in FastEndpoints
  - [ ] 6.6 Configure Swagger/OpenAPI (optional but recommended for testing)
  - [ ] 6.7 Remove default WeatherForecast endpoint if present
  - [ ] 6.8 Run all unit tests and verify they pass (`dotnet test --filter Category=Unit`)
  - [ ] 6.9 Run all integration tests and verify they pass (`dotnet test --filter Category=Integration`)
  - [ ] 6.10 Run the application and test all endpoints manually or with Swagger
  - [ ] 6.11 Create `README.md` with setup instructions, architecture overview, and how to run
  - [ ] 6.12 Create `.gitignore` file for .NET projects
  - [ ] 6.13 Final verification: Check Clean Architecture dependencies are correct
  - [ ] 6.14 Final verification: Ensure bounded contexts are properly isolated (User and Task modules don't directly reference each other's Domain/Infrastructure)
  - [ ] 6.15 Final code review: DDD patterns, CQRS implementation, Repository pattern, Result pattern usage
