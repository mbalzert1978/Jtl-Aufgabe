# JtlTask - Modular Monolith Task Management System

A demonstration of modern .NET 8 backend architecture using **Clean Architecture**, **Domain-Driven Design (DDD)**, **CQRS**, and **FastEndpoints** within a modular monolith structure.

## 🎯 Project Overview

JtlTask is a task management system consisting of two bounded contexts (modules) organized as a modular monolith:

- **Users Module**: Create and retrieve users
- **WorkItems Module**: Create and retrieve work items (tasks) assigned to users

The project emphasizes architectural quality, maintainability, and testability through proper layering, dependency management, and design patterns.

## 🏗️ Architecture

### Layered Architecture (Clean Architecture)

```bash
┌─────────────────────────────────────────────────────────────┐
│                    Presentation Layer                       │
│              (JtlTask.WebApi - FastEndpoints)               │
│                                                             │
│  • Endpoints (HTTP)                                         │
│  • Request/Response DTOs                                    │
│  • Validators (FluentValidation)                            │
└─────────────────────────────────────────────────────────────┘
        │                                          │
        │ depends on                               │ depends on
        ↓                                          ↓
┌──────────────────────────────┐    ┌──────────────────────────────┐
│    Application Layer         │    │   Infrastructure Layer       │
│  (*.Application)             │    │  (*.Infrastructure)          │
│                              │    │                              │
│  • Command Handlers          │    │  • EF Core DbContext         │
│  • Query Handlers            │    │  • Repository Implementations│
│  • DTOs/Adapters             │    │  • External Services         │
└──────────────────────────────┘    └──────────────────────────────┘
        │                                          │
        │ depends on                               │ depends on
        ↓                                          ↓
┌─────────────────────────────────────────────────────────────┐
│                      Domain Layer                           │
│                    (*.Domain - Core)                        │
│                                                             │
│  • Entities & Aggregates                                    │
│  • Value Objects                                            │
│  • Domain Services                                          │
│  • Repository Interfaces                                    │
│  • Business Rules                                           │
│                                                             │
│  ⚠️  NO dependencies on outer layers                        │
└─────────────────────────────────────────────────────────────┘
```

**Dependency Rule (The Dependency Inversion Principle):**

- **Domain Layer**: No dependencies → Application core
- **Application Layer**: Depends on Domain
- **Infrastructure Layer**: Depends on Domain (implements Repository interfaces)
- **Presentation Layer**: Depends on Application + Infrastructure (for DI registration)

**Why Infrastructure is not "below" Domain:**

1. **Domain defines interfaces** (`IUserRepository`)
2. **Infrastructure implements them** (`UserRepository : IUserRepository`)
3. **Dependency direction**: Infrastructure → Domain (not the other way!)
4. **Dependency Injection**: Presentation Layer registers Infrastructure implementations for Domain interfaces

**Runtime call flow** (different from dependency direction):

```bash
HTTP Request
    ↓
Presentation (Endpoint)
    ↓
Application (Handler)
    ↓
Domain (Business Logic)
    ↓
Infrastructure (Repository) ← via Interface abstraction
    ↓
Database
```

### Bounded Contexts (Modules)

#### Users Module

- **Domain**: User aggregate, Username value object
- **Application**: RegisterUser, GetUserById commands/queries
- **Infrastructure**: EF Core DbContext, SQLite database
- **Presentation**: Endpoints in JtlTask.WebApi

#### WorkItems Module

- **Domain**: WorkItem aggregate, Description value object
- **Application**: AssignWorkItem, GetWorkItemsByUserId commands/queries
- **Infrastructure**: EF Core DbContext, SQLite database
- **Presentation**: Endpoints in JtlTask.WebApi

## 📁 Project Structure

```bash
JtlTask.slnx
├── src/
│   ├── JtlTask.WebApi/                    # Presentation Layer + Host
│   ├── SharedKernel/                      # Shared abstractions
│   ├── Users.Domain/                      # Users bounded context - Domain
│   ├── Users.Application/                 # Users bounded context - Application
│   ├── Users.Infrastructure/              # Users bounded context - Infrastructure
│   ├── WorkItems.Domain/                  # WorkItems bounded context - Domain
│   ├── WorkItems.Application/             # WorkItems bounded context - Application
│   └── WorkItems.Infrastructure/          # WorkItems bounded context - Infrastructure
│
├── tests/
│   ├── Test.JtlTask.WebApi/               # End to End tests
│   ├── Tests.Users/                       # Users module architecture and unit tests
│   └── Tests.WorkItems/                   # WorkItems module architecture and unit tests
│
├── data/                                  # SQLite databases (persistent volume)
│   ├── users.db
│   └── workitems.db
│
├── docker-compose.yml                     # Production configuration
├── docker-compose.override.yml            # Development configuration
├── Dockerfile                             # Multi-stage build
└── README.md
```

## 🚀 Getting Started

### Prerequisites

- **Docker** 20.10+ and **Docker Compose** 2.0+
- **Optional**: .NET 8 SDK (for local development without Docker)

### Quick Start with Docker Compose

1. **Clone the repository**

   ```bash
   git clone <repository-url>
   cd JtlTask
   ```

2. **Start the application**

   ```bash
   # Production mode
   docker-compose up -d

   # Development mode with hot reload
   docker-compose -f docker-compose.yml -f docker-compose.override.yml up
   ```

3. **Access the application**
   - API: `http://localhost:5050`
   - Swagger UI: `http://localhost:5050/swagger`
   - Health Check: `http://localhost:5050/health`

4. **View logs**

   ```bash
   docker-compose logs -f web-api
   ```

5. **Stop the application**

   ```bash
   docker-compose down

   # Remove volumes (delete database)
   docker-compose down -v
   ```

### Development Modes

#### Production Mode

```bash
docker-compose up -d
```

- Uses port `5050`
- Persistent data volume
- Restart policy: `unless-stopped`
- Resource limits enforced

#### Development Mode

```bash
docker-compose -f docker-compose.yml -f docker-compose.override.yml up
```

- Uses ports `8080` (HTTP) and `8081` (HTTPS)
- Local data directory mounted at `./data`
- Hot reload enabled (source files mounted)
- No restart policy
- Environment: `Development`

### Local Development (without Docker)

```bash
# Restore dependencies
dotnet restore

# Build the solution
dotnet build

# Run the application
cd src/JtlTask.WebApi
dotnet run
```

The API will be available at `http://localhost:5000`.

### Running Tests

```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test tests/Tests.Users
dotnet test tests/Tests.WorkItems
dotnet test tests/Test.JtlTask.WebApi

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"
```

## 📋 API Endpoints

### Register a New User

```http
POST /api/v1/users
Content-Type: application/json

{
  "username": "john_doe"
}
```

**Response (201 Created):**

```json
{
  "userId": "550e8400-e29b-41d4-a716-446655440000",
  "username": "john_doe"
}
```

#### Get User by ID

```http
GET /api/v1/users/{userId}
```

**Response (200 OK):**

```json
{
  "userId": "550e8400-e29b-41d4-a716-446655440000",
  "username": "john_doe"
}
```

**Response (404 Not Found):** Empty body with 404 status

---

### Assign a Work Item

```http
POST /api/v1/users/{userId}/workitems
Content-Type: application/json

{
  "title": "Implement authentication",
  "estimatedHours": 8.5,
  "parentTaskId": "770e8400-e29b-41d4-a716-446655440002",
  "description": "Add JWT-based authentication to the API",
  "priority": "High",
  "dueDate": "2025-11-01T00:00:00Z",
  "tags": ["security", "authentication"]
}
```

**Response (201 Created):**

```json
{
  "id": "660e8400-e29b-41d4-a716-446655440001",
  "assigneeId": "550e8400-e29b-41d4-a716-446655440000",
  "title": "Implement authentication",
  "estimatedHours": 8.5,
  "parentTaskId": "770e8400-e29b-41d4-a716-446655440002",
  "description": "Add JWT-based authentication to the API",
  "priority": "High",
  "dueDate": "2025-11-01T00:00:00Z",
  "tags": ["security", "authentication"]
}
```

#### Get Work Items by Assignee

```http
GET /api/v1/users/{userId}/workitems
```

**Response (200 OK):**

```json
[
  {
    "id": "660e8400-e29b-41d4-a716-446655440001",
    "assigneeId": "550e8400-e29b-41d4-a716-446655440000",
    "title": "Implement authentication",
    "estimatedHours": 8.5,
    "parentTaskId": "770e8400-e29b-41d4-a716-446655440002",
    "description": "Add JWT-based authentication to the API",
    "priority": "High",
    "dueDate": "2025-11-01T00:00:00Z",
    "tags": ["security", "authentication"]
  }
]
```

**Response (200 OK):** Empty array `[]` if no work items found for the assignee

---

## 🏛️ Design Patterns & Principles

### 1. **Clean Architecture**

- **Dependency Rule**: Inner layers don't know about outer layers
- **Independent testability**: Each layer can be tested independently
- **Framework-agnostic**: Business logic is isolated from infrastructure

### 2. **Domain-Driven Design (DDD)**

- **Aggregate Roots**: `User`, `WorkItem` entities
- **Value Objects**: `Username`, `Description`
- **Ubiquitous Language**: Clear domain terminology
- **Repository Pattern**: Abstract data access

### 3. **CQRS (Command Query Responsibility Segregation)**

- **Commands**: Write operations (RegisterUser, AssignWorkItem)
- **Queries**: Read operations (GetUserById, GetWorkItemsByUserId)
- **Handlers**: Command/Query handlers implement business logic
- **Result Monad**: Functional error handling without exceptions

### 4. **FastEndpoints**

- **Endpoint-based routing**: Each endpoint is a distinct class
- **Request/Response DTOs**: Automatic mapping and validation
- **FluentValidation**: Built-in request validation
- **Auto-discovery**: Endpoints are automatically registered

### 5. **Repository Pattern**

- **Abstraction**: `IUserRepository`, `IWorkItemRepository`
- **SQLite Storage**: Persistent storage with EF Core
- **Persistence Abstraction**: Easy to swap database provider

### 6. **Modular Monolith**

- **Bounded Contexts**: Users and WorkItems are independent modules
- **Clear Boundaries**: No circular dependencies between modules
- **Isolated Concerns**: Each module has its own Domain, Application, Infrastructure

---

## 🔄 Data Flow Example: Create a User

```bash
1. API Request
   POST /api/v1/users
   { "username": "alice" }
           ↓
2. FastEndpoints Validation
   ✓ Username is not null
   ✓ Username is 3-50 characters
           ↓
3. Endpoint Handler
   RegisterUserRequest → RegisterUserCommand
           ↓
4. Application Layer (CQRS)
   RegisterUserCommandHandler receives command
           ↓
5. Domain Layer (DDD)
   User.Create("alice") validates business rules
   ✓ Check Username uniqueness via repository
           ↓
6. Infrastructure Layer
   UserRepository.AddAsync(user) persists to SQLite
           ↓
7. Result Mapping
   Success: UserEntity → UserResponse (201 Created)
   Error: DomainError → ProblemDetails (400/409/500)
           ↓
8. API Response
   {
     "userId": "...",
     "username": "alice"
   }
```

---

## 🧪 Testing Strategy

### Unit Tests (Domain & Application)

- Test domain entities and business logic
- Test command/query handlers
- Test repository implementations
- Location: `tests/Tests.Users`, `tests/Tests.WorkItems`

### Integration Tests (Endpoints)

- Test full request/response cycle
- Use `WebApplicationFactory` for in-memory testing
- Test validation, authorization, and error handling
- Location: `tests/Test.JtlTask.WebApi`

### Test Example

```csharp
[Fact]
public async Task RegisterUser_WithValidUsername_ReturnsCreatedResponse()
{
    // Arrange
    var builder = RegisterUserTestsBuilder.New(_app)
        .WithValidUser();

    // Act
    var result = await builder.ExecuteAsync();

    // Assert
    result.Response.Should().BeSuccessful();
    result.Response.StatusCode.Should().Be(StatusCodes.Status201Created);
}
```

---

## 📝 Architectural Decision Records (ADRs)

### ADR 001: Query Layer Placement

**Decision**: Queries placed in WebApi layer with direct DbContext access  
**Rationale**: Performance, simplicity for CRUD operations  
**Review Trigger**: Complex queries requiring reusable filtering

### ADR 004: Event Flow Direction

**Decision**: Unidirectional event flow: Users → WorkItems (no events back)  
**Rationale**: Loose coupling, prevents circular dependencies  

---

## 🔐 Error Handling

All errors follow **RFC 7807 Problem Details** format:

```json
{
  "type": "https://example.com/errors/user-not-found",
  "title": "User Not Found",
  "status": 404,
  "detail": "The requested user does not exist.",
  "instance": "/api/v1/users/550e8400-e29b-41d4-a716-446655440000"
}
```

### Result Monad Pattern

Business logic returns `Result<T, Error>` instead of throwing exceptions:

```csharp
// Domain Layer
public static Result<User, DomainError> Create(string username)
{
    if (string.IsNullOrWhiteSpace(username))
        return Failure<User, DomainError>(UsernameLengthError);
    
    return Success<User, DomainError>(new User(Guid.NewGuid(), username));
}

// Application Layer
var result = User.Create(command.Username);
return result
    .Bind(user => repository.AddAsync(user))
    .Map(_ => userAdapter);

// Presentation Layer
await result.Match(
    ok: response => SendSuccessResponseAsync(response),
    err: error => SendErrorResponseAsync(error)
);
```

---

## 📦 Technology Stack

| Layer | Technology |
|-------|-----------|
| **Framework** | ASP.NET Core 8 |
| **API** | FastEndpoints |
| **Validation** | FluentValidation |
| **CQRS** | FastEndpoints Command Bus + Mediator library |
| **Database** | SQLite with EF Core |
| **Containerization** | Docker & Docker Compose |
| **Testing** | xUnit, Shouldly, FastEndpoints.Testing |
| **Error Handling** | Result Monad (Monads.Results) |

---

## 🛠️ Development Workflow

### Adding a New Feature (Example: Delete User)

1. **Domain Layer** (`Users.Domain`)
   - Add domain logic to `User` entity

2. **Application Layer** (`Users.Application`)
   - Create `DeleteUserCommand`
   - Create `DeleteUserCommandHandler`
   - Write unit tests

3. **Infrastructure Layer** (`Users.Infrastructure`)
   - Implement `DeleteAsync` in `UserRepository`

4. **Presentation Layer** (`JtlTask.WebApi`)
   - Create `DeleteUserEndpoint`
   - Create `DeleteUserValidator`
   - Write integration tests

5. **Dependency Injection** (`Program.cs`)
   - Register new handlers if needed

---

## 🐳 Docker Configuration

### Docker Compose Services

#### Production (`docker-compose.yml`)

```yaml
services:
  web-api:
    image: jtltask-webapi
    ports:
      - "5050:8080"
    volumes:
      - jtltask-data:/app/data  # Persistent SQLite databases
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
    restart: unless-stopped
    deploy:
      resources:
        limits:
          memory: 512M
          cpus: '1.0'
```

#### Development Override (`docker-compose.override.yml`)

```yaml
services:
  web-api:
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
    ports:
      - "8080:8080"
      - "8081:8081"
    volumes:
      - ./data:/app/data        # Mount local data directory
      - ./src:/src:ro           # Enable hot reload
    restart: no
```

### Docker Commands

```bash
# Build the image
docker-compose build

# Start in production mode
docker-compose up -d

# Start in development mode
docker-compose -f docker-compose.yml -f docker-compose.override.yml up

# View logs
docker-compose logs -f web-api

# Stop and remove containers
docker-compose down

# Stop and remove containers + volumes
docker-compose down -v

# Rebuild and restart
docker-compose up -d --build

# Access container shell
docker-compose exec web-api /bin/bash
```

### Volume Management

```bash
# List volumes
docker volume ls

# Inspect volume
docker volume inspect jtltask-data

# Backup database
docker cp jtltask-webapi:/app/data/users.db ./backup/

# Restore database
docker cp ./backup/users.db jtltask-webapi:/app/data/
```

---

## 📚 Resources & Documentation

- [Clean Architecture Principles](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [Domain-Driven Design](https://www.domainlanguage.com/ddd/)
- [CQRS Pattern](https://martinfowler.com/bliki/CQRS.html)
- [FastEndpoints Documentation](https://fast-endpoints.com/)
- [Docker Compose Documentation](https://docs.docker.com/compose/)
- [Result Monad Guide](./.agent/docs/Result/result-monad-guide.md)

---

## 🔧 Troubleshooting

### Port Already in Use

```bash
# Find process using port 5050
lsof -i :5050

# Kill the process
kill -9 <PID>
```

### Database Locked Error

```bash
# Stop all containers
docker-compose down

# Remove volumes
docker-compose down -v

# Restart
docker-compose up -d
```

### Container Won't Start

```bash
# View logs
docker-compose logs web-api

# Check container status
docker-compose ps

# Rebuild image
docker-compose build --no-cache
```

---

## 📄 License

This project is provided as-is for educational and demonstration purposes.

---

## ✨ Key Achievements

✅ Clean Architecture with proper layering and dependency flow  
✅ Domain-Driven Design with aggregates and value objects  
✅ CQRS pattern with command/query separation  
✅ FastEndpoints integration with automatic validation  
✅ Result Monad for functional error handling  
✅ Repository Pattern with abstraction  
✅ Modular structure with clear bounded contexts  
✅ Comprehensive test coverage (unit + integration)  
✅ RFC 7807 Problem Details error responses  
✅ Docker Compose orchestration with production and development modes  
✅ Persistent SQLite storage with Docker volumes  

---

**Version**: 1.0  
**Last Updated**: 2025  
**Maintainer**: Development Team
