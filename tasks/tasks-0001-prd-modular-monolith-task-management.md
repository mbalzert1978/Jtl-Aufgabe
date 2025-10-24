# Task List: Modular Monolith Task Management System

Generated from: `0001-prd-modular-monolith-task-management.md`

## Relevant Files

### Solution and Project Files

- `JtlTask.sln` - Solution file
- `src/JtlTask.WebApi/JtlTask.WebApi.csproj` - Web API host project
- `src/JtlTask.WebApi/Program.cs` - Application entry point and DI configuration
- `src/Users/Domain/Domain.csproj` - User domain project
- `src/Users/Infrastructure/Infrastructure.csproj` - User infrastructure project
- `src/Users/Application/Application.csproj` - User use cases project
- `src/Users/Contracts/Contracts.csproj` - User contracts project
- `src/Tasks/Domain/JtlTask.Tasks.Domain.csproj` - Task domain project
- `src/Tasks/Infrastructure/JtlTask.Tasks.Infrastructure.csproj` - Task infrastructure project
- `src/Tasks/Application/JtlTask.Tasks.Application.csproj` - Task use cases project
- `src/Tasks/Contracts/JtlTask.Tasks.Contracts.csproj` - Task contracts project
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

### User Bounded Context - Application

- `src/Users/Application/Commands/CreateUser/CreateUserCommand.cs` - Create user command
- `src/Users/Application/Commands/CreateUser/CreateUserHandler.cs` - Create user command handler
- `src/Users/Application/Queries/GetUserById/GetUserByIdQuery.cs` - Get user by ID query
- `src/Users/Application/Queries/GetUserById/GetUserByIdHandler.cs` - Get user by ID query handler
- `src/Users/Application/Queries/UserExists/UserExistsQuery.cs` - User exists query (for inter-context communication)
- `src/Users/Application/Queries/UserExists/UserExistsHandler.cs` - User exists query handler
- `src/Users/Application/DependencyInjection.cs` - Application DI registration

### User Bounded Context - Contracts

- `src/Users/Contracts/CreateUserEndpoint.cs` - POST /api/users endpoint
- `src/Users/Contracts/GetUserByIdEndpoint.cs` - GET /api/users/{id} endpoint
- `src/Users/Contracts/Contracts/CreateUserRequest.cs` - Create user request DTO
- `src/Users/Contracts/Contracts/CreateUserResponse.cs` - Create user response DTO
- `src/Users/Contracts/Contracts/GetUserByIdResponse.cs` - Get user response DTO
- `src/Users/Contracts/Validators/CreateUserValidator.cs` - FluentValidation validator for create user
- `src/Users/Contracts/DependencyInjection.cs` - Contracts DI registration

### Task Bounded Context - Domain

- `src/Tasks/Domain/Task.cs` - Task aggregate root entity
- `src/Tasks/Domain/ITaskRepository.cs` - Task repository interface
- `src/Tasks/Domain/DomainErrors.cs` - Domain-specific error definitions

### Task Bounded Context - Infrastructure

- `src/Tasks/Infrastructure/Persistence/InMemoryTaskRepository.cs` - In-memory task repository implementation
- `src/Tasks/Infrastructure/DependencyInjection.cs` - Infrastructure DI registration

### Task Bounded Context - Application

- `src/Tasks/Application/Commands/CreateTask/CreateTaskCommand.cs` - Create task command
- `src/Tasks/Application/Commands/CreateTask/CreateTaskHandler.cs` - Create task command handler
- `src/Tasks/Application/Queries/GetTasksByUserId/GetTasksByUserIdQuery.cs` - Get tasks by user ID query
- `src/Tasks/Application/Queries/GetTasksByUserId/GetTasksByUserIdHandler.cs` - Get tasks by user ID query handler
- `src/Tasks/Application/DependencyInjection.cs` - Application DI registration

### Task Bounded Context - Contracts

- `src/Tasks/Contracts/CreateTaskEndpoint.cs` - POST /api/tasks endpoint
- `src/Tasks/Contracts/GetTasksByUserIdEndpoint.cs` - GET /api/tasks/by-user/{userId} endpoint
- `src/Tasks/Contracts/Contracts/CreateTaskRequest.cs` - Create task request DTO
- `src/Tasks/Contracts/Contracts/CreateTaskResponse.cs` - Create task response DTO
- `src/Tasks/Contracts/Contracts/GetTasksByUserIdResponse.cs` - Get tasks response DTO
- `src/Tasks/Contracts/Validators/CreateTaskValidator.cs` - FluentValidation validator for create task
- `src/Tasks/Contracts/DependencyInjection.cs` - Contracts DI registration

### Shared Kernel

- `src/Shared/JtlTask.Shared.Kernel/Result.cs` - Result monad implementation
- `src/Shared/JtlTask.Shared.Kernel/Error.cs` - Error type for Result pattern
- `src/Shared/JtlTask.Shared.Kernel/IEntity.cs` - Base entity interface

### Tests

- `tests/Tests.Users/Domain/UserTests.cs` - User entity unit tests
- `tests/Tests.Users/Domain/UsernameTests.cs` - Username value object unit tests
- `tests/Tests.Users/Application/Commands/CreateUser/CreateUserHandlerTests.cs` - Create user handler unit tests
- `tests/Tests.Users/Application/Queries/GetUserById/GetUserByIdHandlerTests.cs` - Get user by ID handler unit tests
- `tests/Tests.Users/Contracts/CreateUserEndpointTests.cs` - Create user endpoint integration tests
- `tests/Tests.Users/Contracts/GetUserByIdEndpointTests.cs` - Get user by ID endpoint integration tests
- `tests/Tests.Tasks/Domain/TaskTests.cs` - Task entity unit tests
- `tests/Tests.Tasks/Application/Commands/CreateTask/CreateTaskHandlerTests.cs` - Create task handler unit tests
- `tests/Tests.Tasks/Application/Queries/GetTasksByUserId/GetTasksByUserIdHandlerTests.cs` - Get tasks by user ID handler unit tests
- `tests/Tests.Tasks/Contracts/CreateTaskEndpointTests.cs` - Create task endpoint integration tests
- `tests/Tests.Tasks/Contracts/GetTasksByUserIdEndpointTests.cs` - Get tasks by user ID endpoint integration tests

### Notes

- Each bounded context (Users, Tasks) is organized into separate projects: Domain, Infrastructure, Application, and Contracts under `src/Users/` and `src/Tasks/`
- The Domain layer has no external dependencies
- Inter-context communication happens through Application layer (e.g., Task module uses UserExistsQuery from User module)
- Tests are organized in `tests/` directory and mirror the source structure (e.g., `src/Users/Domain/...` → `tests/Tests.Users/Domain/...`)
- Use `dotnet test` to run all tests, or `dotnet test tests/Tests.Users` to run specific module tests

## Tasks

- [ ] 1.0 Setup Solution Structure and Foundation (src/Projects with Bounded Contexts)
  - [x] 1.1 Install FastContracts Template Pack: `dotnet new install FastContracts.TemplatePack`
  - [x] 1.2 Create solution file `JtlTask.sln` using `dotnet new sln -n JtlTask`
  - [x] 1.3 Create directory structure: `src/Users`, `src/Tasks`, `src/Shared`, `tests/Tests.Users`, `tests/Tests.Tasks`
  - [x] 1.4 Create `JtlTask.WebApi` project using `dotnet new feproj -n JtlTask.WebApi` in `src/JtlTask.WebApi`
  - [x] 1.5 Create `JtlTask.Shared.Kernel` class library in `src/Shared/JtlTask.Shared.Kernel`
  - [x] 1.6 Create User module projects in `src/Users/`: `Domain/Domain`, `Infrastructure/Infrastructure`, `Application/Application`, `Contracts/Contracts`
  - [x] 1.7 Create Task module projects in `src/Tasks/`: `Domain/JtlTask.Tasks.Domain`, `Infrastructure/JtlTask.Tasks.Infrastructure`, `Application/JtlTask.Tasks.Application`, `Contracts/JtlTask.Tasks.Contracts`
  - [ ] 1.8 Create test projects: `tests/Tests.Users/JtlTask.Tests.Users.csproj`, `tests/Tests.Tasks/JtlTask.Tests.Tasks.csproj` (xUnit)
  - [ ] 1.9 Add all projects to solution using `dotnet sln add`
  - [ ] 1.10 Configure project references following Clean Architecture rules (all dependencies point inwards)
  - [ ] 1.11 Add provided Result monad implementation to Shared.Kernel (will be provided by user)
  - [ ] 1.12 Add base interfaces/classes in Shared.Kernel (`IEntity.cs`)
  - [ ] 1.13 Verify solution builds successfully with `dotnet build`

- [ ] 2.0 Implement User Bounded Context - Domain, Infrastructure, and Application
  - [ ] 2.1 **Domain Layer**: Create `Username` value object with validation (3-50 chars, alphanumeric + underscore/hyphen)
  - [ ] 2.2 **Domain Layer**: Write unit tests for `Username` value object
  - [ ] 2.3 **Domain Layer**: Create `User` aggregate root with private constructor and static `Create` factory method
  - [ ] 2.4 **Domain Layer**: Implement business rule: Username uniqueness check (delegated to repository)
  - [ ] 2.5 **Domain Layer**: Write unit tests for `User` entity creation and validation
  - [ ] 2.6 **Domain Layer**: Define `IUserRepository` interface with methods: `GetByIdAsync`, `ExistsByUsernameAsync`, `AddAsync`
  - [ ] 2.7 **Domain Layer**: Create `DomainErrors.cs` with user-specific error definitions
  - [ ] 2.8 **Infrastructure Layer**: Implement `InMemoryUserRepository` using `ConcurrentDictionary<Guid, User>`
  - [ ] 2.9 **Infrastructure Layer**: Create `DependencyInjection.cs` extension method to register repository
  - [ ] 2.10 **Application Layer**: Create `CreateUserCommand` record with Username property
  - [ ] 2.11 **Application Layer**: Create `CreateUserHandler` implementing `ICommandHandler<CreateUserCommand, Result<User>>`
  - [ ] 2.12 **Application Layer**: Implement handler logic: validate uniqueness, create user, save to repository
  - [ ] 2.13 **Application Layer**: Write unit tests for `CreateUserHandler` (success case, duplicate username)
  - [ ] 2.14 **Application Layer**: Create `GetUserByIdQuery` record with UserId property
  - [ ] 2.15 **Application Layer**: Create `GetUserByIdHandler` implementing `ICommandHandler<GetUserByIdQuery, Result<User>>`
  - [ ] 2.16 **Application Layer**: Write unit tests for `GetUserByIdHandler` (success case, user not found)
  - [ ] 2.17 **Application Layer**: Create `UserExistsQuery` record with UserId property (for inter-context communication)
  - [ ] 2.18 **Application Layer**: Create `UserExistsHandler` implementing `ICommandHandler<UserExistsQuery, Result<bool>>`
  - [ ] 2.19 **Application Layer**: Create `DependencyInjection.cs` to register command handlers

- [ ] 3.0 Implement User Bounded Context - Application Services (Inter-Context API) and Web Contracts
  - [ ] 3.1 **Contracts Layer**: Create `CreateUserRequest` DTO record
  - [ ] 3.2 **Contracts Layer**: Create `CreateUserResponse` DTO record
  - [ ] 3.3 **Contracts Layer**: Create `CreateUserValidator` inheriting from `Validator<CreateUserRequest>`
  - [ ] 3.4 **Contracts Layer**: Implement validation rules: Username required, 3-50 chars, allowed characters
  - [ ] 3.5 **Contracts Layer**: Create `CreateUserEndpoint` inheriting from `Endpoint<CreateUserRequest, CreateUserResponse>`
  - [ ] 3.6 **Contracts Layer**: Configure endpoint: POST /api/users, returns 201 Created or 400 Bad Request
  - [ ] 3.7 **Contracts Layer**: Implement endpoint handler: send CreateUserCommand, map Result to HTTP response
  - [ ] 3.8 **Contracts Layer**: Create `GetUserByIdResponse` DTO record
  - [ ] 3.9 **Contracts Layer**: Create `GetUserByIdEndpoint` inheriting from `Endpoint<EmptyRequest, GetUserByIdResponse>`
  - [ ] 3.10 **Contracts Layer**: Configure endpoint: GET /api/users/{id}, returns 200 OK or 404 Not Found
  - [ ] 3.11 **Contracts Layer**: Implement endpoint handler: send GetUserByIdQuery, map Result to HTTP response
  - [ ] 3.12 **Contracts Layer**: Create `DependencyInjection.cs` to register contracts
  - [ ] 3.13 **Integration Tests**: Write test for CreateUserEndpoint (success case)
  - [ ] 3.14 **Integration Tests**: Write test for CreateUserEndpoint (validation failure)
  - [ ] 3.15 **Integration Tests**: Write test for CreateUserEndpoint (duplicate username)
  - [ ] 3.16 **Integration Tests**: Write test for GetUserByIdEndpoint (success case)
  - [ ] 3.17 **Integration Tests**: Write test for GetUserByIdEndpoint (user not found - 404)

- [ ] 4.0 Implement Task Bounded Context - Domain, Infrastructure, and Application
  - [ ] 4.1 **Domain Layer**: Create `Task` aggregate root with properties: Id, Name, AssigneeId
  - [ ] 4.2 **Domain Layer**: Implement static `Create` factory method with validation (Name 3-200 chars, AssigneeId required)
  - [ ] 4.3 **Domain Layer**: Write unit tests for `Task` entity creation and validation
  - [ ] 4.4 **Domain Layer**: Define `ITaskRepository` interface with methods: `GetByAssigneeIdAsync`, `AddAsync`
  - [ ] 4.5 **Domain Layer**: Create `DomainErrors.cs` with task-specific error definitions
  - [ ] 4.6 **Infrastructure Layer**: Implement `InMemoryTaskRepository` using `ConcurrentDictionary<Guid, Task>` or `List<Task>`
  - [ ] 4.7 **Infrastructure Layer**: Create `DependencyInjection.cs` extension method to register repository
  - [ ] 4.8 **Application Layer**: Create `CreateTaskCommand` record with Name and AssigneeId properties
  - [ ] 4.9 **Application Layer**: Create `CreateTaskHandler` implementing `ICommandHandler<CreateTaskCommand, Result<Task>>`
  - [ ] 4.10 **Application Layer**: Inject `UserExistsQuery` handler from User module (inter-context communication)
  - [ ] 4.11 **Application Layer**: Implement handler logic: verify assignee exists, create task, save to repository
  - [ ] 4.12 **Application Layer**: Write unit tests for `CreateTaskHandler` (success case, assignee not found)
  - [ ] 4.13 **Application Layer**: Create `GetTasksByUserIdQuery` record with UserId property
  - [ ] 4.14 **Application Layer**: Create `GetTasksByUserIdHandler` implementing `ICommandHandler<GetTasksByUserIdQuery, Result<List<Task>>>`
  - [ ] 4.15 **Application Layer**: Implement handler logic: verify user exists, get tasks (return empty list if none)
  - [ ] 4.16 **Application Layer**: Write unit tests for `GetTasksByUserIdHandler` (success with tasks, success with empty list, user not found)
  - [ ] 4.17 **Application Layer**: Create `DependencyInjection.cs` to register command handlers

- [ ] 5.0 Implement Task Bounded Context - Application Services (Inter-Context API) and Web Contracts
  - [ ] 5.1 **Contracts Layer**: Create `CreateTaskRequest` DTO record
  - [ ] 5.2 **Contracts Layer**: Create `CreateTaskResponse` DTO record
  - [ ] 5.3 **Contracts Layer**: Create `CreateTaskValidator` inheriting from `Validator<CreateTaskRequest>`
  - [ ] 5.4 **Contracts Layer**: Implement validation rules: Name required (3-200 chars), AssigneeId required and valid GUID
  - [ ] 5.5 **Contracts Layer**: Create `CreateTaskEndpoint` inheriting from `Endpoint<CreateTaskRequest, CreateTaskResponse>`
  - [ ] 5.6 **Contracts Layer**: Configure endpoint: POST /api/tasks, returns 201 Created or 400 Bad Request
  - [ ] 5.7 **Contracts Layer**: Implement endpoint handler: send CreateTaskCommand, map Result to HTTP response
  - [ ] 5.8 **Contracts Layer**: Create `GetTasksByUserIdResponse` DTO record with Tasks list
  - [ ] 5.9 **Contracts Layer**: Create `GetTasksByUserIdEndpoint` inheriting from `Endpoint<EmptyRequest, GetTasksByUserIdResponse>`
  - [ ] 5.10 **Contracts Layer**: Configure endpoint: GET /api/tasks/by-user/{userId}, returns 200 OK or 404 Not Found
  - [ ] 5.11 **Contracts Layer**: Implement endpoint handler: send GetTasksByUserIdQuery, map Result to HTTP response
  - [ ] 5.12 **Contracts Layer**: Create `DependencyInjection.cs` to register contracts
  - [ ] 5.13 **Integration Tests**: Write test for CreateTaskEndpoint (success case)
  - [ ] 5.14 **Integration Tests**: Write test for CreateTaskEndpoint (validation failure)
  - [ ] 5.15 **Integration Tests**: Write test for CreateTaskEndpoint (assignee not found)
  - [ ] 5.16 **Integration Tests**: Write test for GetTasksByUserIdEndpoint (success with tasks)
  - [ ] 5.17 **Integration Tests**: Write test for GetTasksByUserIdEndpoint (success with empty list)
  - [ ] 5.18 **Integration Tests**: Write test for GetTasksByUserIdEndpoint (user not found - 404)

- [ ] 6.0 Configure Cross-Cutting Concerns and Application Host
  - [ ] 6.1 Configure FastContracts in `Program.cs` with `builder.Services.AddFastContracts()`
  - [ ] 6.2 Configure FastContracts middleware in `Program.cs` with `app.UseFastContracts()`
  - [ ] 6.3 Register User module services in `Program.cs` (Domain, Infrastructure, Application, Contracts)
  - [ ] 6.4 Register Task module services in `Program.cs` (Domain, Infrastructure, Application, Contracts)
  - [ ] 6.5 Configure Problem Details error responses in FastContracts
  - [ ] 6.6 Configure Swagger/OpenAPI (optional but recommended for testing)
  - [ ] 6.7 Remove default WeatherForecast endpoint if present
  - [ ] 6.8 Run all unit tests and verify they pass (`dotnet test --filter Category=Unit`)
  - [ ] 6.9 Run all integration tests and verify they pass (`dotnet test --filter Category=Integration`)
  - [ ] 6.10 Run the application and test all contracts manually or with Swagger
  - [ ] 6.11 Create `README.md` with setup instructions, architecture overview, and how to run
  - [ ] 6.12 Create `.gitignore` file for .NET projects
  - [ ] 6.13 Final verification: Check Clean Architecture dependencies are correct
  - [ ] 6.14 Final verification: Ensure bounded contexts are properly isolated (User and Task modules don't directly reference each other's Domain/Infrastructure)
  - [ ] 6.15 Final code review: DDD patterns, CQRS implementation, Repository pattern, Result pattern usage
