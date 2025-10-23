# PRD: Modular Monolith Task Management System

## 1. Introduction/Overview

Dieses Projekt demonstriert die Umsetzung eines modularen Monolithen in .NET 8 unter Verwendung moderner Architektur-Patterns. Das System besteht aus zwei fachlich getrennten Modulen (User und Task), die innerhalb einer einzigen Solution strukturiert sind. Der Fokus liegt auf Architekturqualität, Wartbarkeit und Testbarkeit - nicht auf Funktionsumfang.

**Problem:** Demonstration der Fähigkeit, Backend-Software nach modernen Architekturprinzipien zu strukturieren.

**Goal:** Implementierung eines wartbaren, testbaren .NET-Systems nach Modular Monolith Pattern mit Clean Architecture, DDD, CQRS und FastEndpoints.

## 2. Goals

1. **Architektur-Exzellenz**: Klare Trennung von Verantwortlichkeiten durch Clean Architecture
2. **Modulare Struktur**: Zwei fachlich getrennte Module (User, Task) mit klaren Grenzen
3. **Pattern-Umsetzung**: Korrekte Anwendung von DDD, CQRS und Repository Pattern
4. **Testbarkeit**: TDD-freundliche Struktur, die kontinuierliches Testen ermöglicht
5. **Einfachheit**: Fokus auf klare, verständliche Implementierung ohne Overengineering

## 3. User Stories

### User Module

- **US-001**: Als Entwickler möchte ich einen User mit Benutzername erstellen können, sodass dieser im System verfügbar ist.
- **US-002**: Als Entwickler möchte ich einen User anhand seiner ID abrufen können, sodass ich seine Daten verwenden kann.

### Task Module

- **US-003**: Als Entwickler möchte ich einen Task mit Namen und Assignee (User ID) erstellen können, sodass Aufgaben im System erfasst werden.
- **US-004**: Als Entwickler möchte ich alle Tasks eines Users anhand seiner ID abrufen können, sodass ich sehe, welche Aufgaben diesem User zugewiesen sind.

## 4. Functional Requirements

### FR-001: Solution Setup

Das System muss als .NET 8 Solution mit folgender Projektstruktur erstellt werden:

- `JtlTask.sln` - Solution-Datei
- `src/JtlTask.WebApi` - ASP.NET Core Web API Projekt (Einstiegspunkt)
- `src/JtlTask.UseCases` - Class Library für Application Layer
- `src/JtlTask.Domain` - Class Library für Domain Layer
- `src/JtlTask.Infrastructure` - Class Library für Infrastructure Layer
- `tests/JtlTask.Tests` - Test-Projekt (xUnit oder NUnit)

### FR-002: Clean Architecture Dependencies

Die Abhängigkeiten zwischen den Projekten müssen folgende Regeln einhalten:

- **Domain**: Keine Abhängigkeiten zu anderen Projekten
- **UseCases**: Abhängigkeit nur zu Domain
- **Infrastructure**: Abhängigkeiten zu Domain und UseCases
- **WebApi**: Abhängigkeiten zu allen anderen Projekten
- Dependency Flow: WebApi → Infrastructure → UseCases → Domain

### FR-003: User Module - Create User

Das System muss einen Endpoint `POST /api/users` bereitstellen mit:

- **Request**: `{ "username": "string" }`
- **Validierung**: Username darf nicht leer sein und muss 3-50 Zeichen lang sein
- **Response Success (201)**: `{ "id": "guid", "username": "string" }`
- **Response Error (400)**: Problem Details Format bei Validierungsfehlern
- **Business Rules**:
  - Username muss unique sein
  - Username darf keine Sonderzeichen außer Unterstrich und Bindestrich enthalten

### FR-004: User Module - Get User by ID

Das System muss einen Endpoint `GET /api/users/{id}` bereitstellen mit:

- **Request**: User ID als Route Parameter (GUID)
- **Response Success (200)**: `{ "id": "guid", "username": "string" }`
- **Response Error (404)**: Problem Details Format wenn User nicht gefunden wurde

### FR-005: Task Module - Create Task

Das System muss einen Endpoint `POST /api/tasks` bereitstellen mit:

- **Request**: `{ "name": "string", "assigneeId": "guid" }`
- **Validierung**:
  - Name darf nicht leer sein und muss 3-200 Zeichen lang sein
  - AssigneeId muss eine gültige GUID sein
- **Response Success (201)**: `{ "id": "guid", "name": "string", "assigneeId": "guid" }`
- **Response Error (400)**: Problem Details Format bei Validierungsfehlern
- **Business Rules**:
  - AssigneeId muss auf einen existierenden User verweisen (Referenzielle Integrität)

### FR-006: Task Module - Get Tasks by User ID

Das System muss einen Endpoint `GET /api/tasks/by-user/{userId}` bereitstellen mit:

- **Request**: User ID als Route Parameter (GUID)
- **Response Success (200)**: `{ "tasks": [{ "id": "guid", "name": "string", "assigneeId": "guid" }] }`
- **Response Error (404)**: Problem Details Format wenn User nicht gefunden wurde
- **Verhalten**: Leeres Array wenn User keine Tasks zugewiesen hat

### FR-007: FastEndpoints Integration

Alle Endpoints müssen mit FastEndpoints implementiert werden:

- Verwendung von `Endpoint<TRequest>` oder `Endpoint<TRequest, TResponse>` Base Classes
- Request DTOs als separate Klassen
- Endpoint-Konfiguration in `Configure()` Methode

### FR-008: CQRS mit FastEndpoints Command Bus

Commands und Queries müssen mit FastEndpoints Command Bus implementiert werden:

- **Commands**: `ICommand` oder `ICommand<TResult>` für Schreiboperationen
- **Handlers**: `ICommandHandler<TCommand>` oder `ICommandHandler<TCommand, TResult>`
- Keine separate MediatR oder ähnliche Bibliothek
- Commands und Handlers pro Modul organisiert

Beispielstruktur:

```bash
UseCases/
  Users/
    Commands/
      CreateUser/
        CreateUserCommand.cs
        CreateUserHandler.cs
    Queries/
      GetUserById/
        GetUserByIdQuery.cs
        GetUserByIdHandler.cs
```

### FR-009: FluentValidation

Request-Validierung muss mit FluentValidation über FastEndpoints erfolgen:

- Validators erben von `Validator<TRequest>`
- Validatoren werden automatisch registriert
- DTOs werden vor Handler-Ausführung validiert

### FR-010: Domain-Driven Design

Das Domain-Modell muss DDD-Prinzipien folgen:

- **Entities**: `User` und `Task` als Aggregate Roots mit Identity (GUID)
- **Value Objects**: Wo sinnvoll (z.B. Username als Value Object)
- **Domain Events**: Optional, falls Zeit vorhanden
- **Repository Interfaces**: Im Domain Layer definiert
- **Business Logic**: In den Entities/Aggregates, nicht in Services

Beispiel Domain Entities:

```csharp
// Domain/Users/User.cs
public class User
{
    public Guid Id { get; private set; }
    public Username Username { get; private set; }
    
    private User() { } // EF Core
    
    public static Result<User> Create(string username)
    {
        // Validierung und Erstellung
    }
}

// Domain/Tasks/Task.cs
public class Task
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public Guid AssigneeId { get; private set; }
    
    private Task() { } // EF Core
    
    public static Result<Task> Create(string name, Guid assigneeId)
    {
        // Validierung und Erstellung
    }
}
```

### FR-011: Repository Pattern mit In-Memory Storage

Data Persistence muss über Repository Pattern mit In-Memory Collections erfolgen:

- **Repository Interfaces**: Im Domain Layer definiert
- **Repository Implementations**: Im Infrastructure Layer
- **Storage**: `Dictionary<Guid, TEntity>` oder `List<TEntity>`
- **Thread-Safety**: Verwendung von `ConcurrentDictionary` wenn nötig

Repository Interfaces:

```csharp
// Domain/Users/IUserRepository.cs
public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task<bool> ExistsByUsernameAsync(string username);
    Task AddAsync(User user);
}

// Domain/Tasks/ITaskRepository.cs
public interface ITaskRepository
{
    Task<IReadOnlyList<Task>> GetByAssigneeIdAsync(Guid assigneeId);
    Task AddAsync(Task task);
}
```

### FR-012: Result Monad Pattern

Fehlerbehandlung in der Business-Logik muss mit Result Monad erfolgen:

- Verwendung einer bereitgestellten Shared Library (z.B. ErrorOr, FluentResults, etc.)
- Commands/Queries geben `Result<T>` zurück
- Endpoints konvertieren Results in HTTP Responses
- Keine Exceptions für Business Logic Failures

### FR-013: Dependency Injection

DI muss korrekt konfiguriert werden:

- Repository Registrierung als Scoped oder Singleton (In-Memory)
- FastEndpoints Auto-Discovery aktiviert
- Services in `Program.cs` registriert

### FR-014: Modulare Trennung

Module müssen klar getrennt sein:

- User-Module darf keine direkten Abhängigkeiten zu Task-Module haben und umgekehrt
- Kommunikation nur über definierte Contracts/Interfaces
- Jedes Modul hat eigene Domain Entities, Repositories, Commands/Queries

Vorgeschlagene Ordnerstruktur:

```bash
Domain/
  Users/
    User.cs
    Username.cs (Value Object)
    IUserRepository.cs
  Tasks/
    Task.cs
    ITaskRepository.cs
    
UseCases/
  Users/
    Commands/...
    Queries/...
  Tasks/
    Commands/...
    Queries/...
    
Infrastructure/
  Repositories/
    UserRepository.cs
    TaskRepository.cs
  DependencyInjection.cs
    
WebApi/
  Endpoints/
    Users/
      CreateUserEndpoint.cs
      GetUserByIdEndpoint.cs
    Tasks/
      CreateTaskEndpoint.cs
      GetTasksByUserIdEndpoint.cs
  Program.cs
```

### FR-015: Test-Driven Development (TDD)

Das Projekt muss nach TDD-Prinzipien entwickelt werden:

- Tests vor Implementierung schreiben
- Red-Green-Refactor Cycle
- Unit Tests für Commands/Queries
- Integration Tests für Endpoints
- Test-Coverage für kritische Business Logic

Test-Kategorien:

- **Unit Tests**: Domain Entities, Value Objects, Command/Query Handlers
- **Integration Tests**: Endpoints mit FastEndpoints TestFramework oder WebApplicationFactory

### FR-016: Problem Details (RFC 7807)

Fehler-Responses müssen dem Problem Details Standard folgen:

- Verwendung von FastEndpoints' eingebautem Problem Details Support
- Konsistentes Error Response Format
- Schnelle Implementierung (ohne komplexes Custom Error Handling)

## 5. Non-Goals (Out of Scope)

❌ **Keine** komplexe CI/CD Pipeline  
❌ **Keine** Produktions-Datenbank (SQLite, SQL Server, PostgreSQL)  
❌ **Keine** Authentication/Authorization  
❌ **Keine** API Versioning  
❌ **Keine** Swagger/OpenAPI Documentation (optional, aber nicht erforderlich)  
❌ **Keine** Logging Infrastructure (Serilog, etc.)  
❌ **Keine** Monitoring/Observability  
❌ **Keine** Docker Containerization  
❌ **Keine** Performance Optimierung  
❌ **Keine** Task Update/Delete Operationen  
❌ **Keine** User Update/Delete Operationen  
❌ **Keine** komplexe Domain Events Infrastruktur  
❌ **Keine** API Rate Limiting  
❌ **Keine** Response Caching  

## 6. Technical Considerations

### Technology Stack

- **.NET Version**: .NET 8 SDK
- **Web Framework**: ASP.NET Core Web API
- **Endpoint Framework**: FastEndpoints (latest stable)
- **Validation**: FluentValidation (via FastEndpoints)
- **CQRS**: FastEndpoints Command Bus
- **Data Storage**: In-Memory Collections (ConcurrentDictionary)
- **Result Pattern**: Shared Library (wird bereitgestellt)
- **Testing**: xUnit oder NUnit

### NuGet Packages

Required packages per project:

**WebApi:**

- FastEndpoints
- FastEndpoints.Swagger (optional)

**UseCases:**

- Shared Result Library (bereitgestellt)

**Domain:**

- Keine externen Dependencies

**Infrastructure:**

- (keine zusätzlichen packages erforderlich)

**Tests:**

- xUnit oder NUnit
- FluentAssertions (empfohlen)
- Microsoft.AspNetCore.Mvc.Testing (für Integration Tests)

### Architecture Decisions

**ADR-001: Clean Architecture Layering**  
Wir verwenden Clean Architecture um klare Abhängigkeitsregeln durchzusetzen. Domain ist der Kern ohne externe Dependencies.

**ADR-002: In-Memory Storage**  
Verwendung von In-Memory Collections statt Datenbank ermöglicht schnelle Entwicklung und einfaches Testing ohne Setup-Overhead.

**ADR-003: FastEndpoints Command Bus statt MediatR**  
FastEndpoints hat ein eingebautes Command Bus Pattern, wodurch keine zusätzliche CQRS-Bibliothek nötig ist.

**ADR-004: Repository Pattern**  
Obwohl In-Memory Storage verwendet wird, abstrahieren wir über Repository Interfaces um Persistenz-Austauschbarkeit zu demonstrieren.

**ADR-005: Result Monad statt Exceptions**  
Business Logic Failures werden als Results modelliert, nicht als Exceptions, für explizites Error Handling.

**ADR-006: Modularer Monolith**  
Module sind logisch getrennt (User, Task) aber physisch in einer Solution, um Monolith-Architektur zu demonstrieren.

### Constraints

- Entwicklung unter .NET 8
- Keine externe Datenbank
- TDD-Ansatz erforderlich
- Alle geforderten Patterns müssen sichtbar sein

## 7. Success Metrics

### Code Quality Metrics

- ✅ **Klare Schichtentrennung**: Dependencies zeigen korrekt nach innen
- ✅ **Testbarkeit**: Alle Commands/Queries haben Unit Tests
- ✅ **Pattern-Umsetzung**: DDD, CQRS, Repository Pattern sind erkennbar implementiert
- ✅ **Modulare Struktur**: User und Task Module sind klar getrennt

### Functional Metrics

- ✅ Alle 4 Endpoints funktionieren korrekt
- ✅ Validierung funktioniert wie spezifiziert
- ✅ Business Rules werden durchgesetzt
- ✅ Error Handling liefert konsistente Responses

### Technical Metrics

- ✅ Solution kompiliert ohne Fehler
- ✅ Tests laufen erfolgreich durch
- ✅ FastEndpoints ist korrekt integriert
- ✅ Result Monad Pattern wird konsistent verwendet

## 8. Implementation Phases (Epic Breakdown)

### Phase 0: Solution Setup ⚙️

**Tasks:**

- [ ] Erstelle Solution-Datei `JtlTask.sln`
- [ ] Erstelle WebApi-Projekt
- [ ] Erstelle UseCases-Projekt (Class Library)
- [ ] Erstelle Domain-Projekt (Class Library)
- [ ] Erstelle Infrastructure-Projekt (Class Library)
- [ ] Erstelle Tests-Projekt
- [ ] Konfiguriere Projekt-Referenzen gemäß Clean Architecture
- [ ] Installiere NuGet Packages
- [ ] Füge Shared Result Library hinzu
- [ ] Verifiziere Solution kompiliert

### Phase 1: User Module - Foundation 👤

**Tasks:**

- [ ] Erstelle User Entity im Domain Layer
- [ ] Erstelle Username Value Object (optional)
- [ ] Erstelle IUserRepository Interface im Domain Layer
- [ ] Implementiere UserRepository (In-Memory) im Infrastructure Layer
- [ ] Schreibe Unit Tests für User Entity

### Phase 2: User Module - Create User Endpoint ➕

**Tasks:**

- [ ] Erstelle CreateUserCommand
- [ ] Erstelle CreateUserHandler
- [ ] Schreibe Unit Tests für CreateUserHandler
- [ ] Erstelle CreateUserRequest DTO
- [ ] Erstelle CreateUserValidator (FluentValidation)
- [ ] Erstelle CreateUserEndpoint (FastEndpoints)
- [ ] Schreibe Integration Test für Create User Endpoint
- [ ] Verifiziere: Username Uniqueness wird geprüft
- [ ] Verifiziere: Validation Errors werden korrekt zurückgegeben

### Phase 3: User Module - Get User Endpoint 🔍

**Tasks:**

- [ ] Erstelle GetUserByIdQuery
- [ ] Erstelle GetUserByIdHandler
- [ ] Schreibe Unit Tests für GetUserByIdHandler
- [ ] Erstelle GetUserByIdRequest DTO
- [ ] Erstelle GetUserByIdEndpoint (FastEndpoints)
- [ ] Schreibe Integration Test für Get User Endpoint
- [ ] Verifiziere: 404 Response wenn User nicht existiert

### Phase 4: Task Module - Foundation 📋

**Tasks:**

- [ ] Erstelle Task Entity im Domain Layer
- [ ] Erstelle ITaskRepository Interface im Domain Layer
- [ ] Implementiere TaskRepository (In-Memory) im Infrastructure Layer
- [ ] Schreibe Unit Tests für Task Entity
- [ ] Implementiere referenzielle Integrität (AssigneeId → User)

### Phase 5: Task Module - Create Task Endpoint ➕

**Tasks:**

- [ ] Erstelle CreateTaskCommand
- [ ] Erstelle CreateTaskHandler
- [ ] Schreibe Unit Tests für CreateTaskHandler
- [ ] Erstelle CreateTaskRequest DTO
- [ ] Erstelle CreateTaskValidator (FluentValidation)
- [ ] Erstelle CreateTaskEndpoint (FastEndpoints)
- [ ] Schreibe Integration Test für Create Task Endpoint
- [ ] Verifiziere: Assignee Validation (User muss existieren)

### Phase 6: Task Module - Get Tasks by User Endpoint 🔍

**Tasks:**

- [ ] Erstelle GetTasksByUserIdQuery
- [ ] Erstelle GetTasksByUserIdHandler
- [ ] Schreibe Unit Tests für GetTasksByUserIdHandler
- [ ] Erstelle GetTasksByUserIdRequest DTO
- [ ] Erstelle GetTasksByUserIdEndpoint (FastEndpoints)
- [ ] Schreibe Integration Test für Get Tasks Endpoint
- [ ] Verifiziere: Leeres Array wenn keine Tasks zugewiesen

### Phase 7: Integration & Polishing ✨

**Tasks:**

- [ ] Konfiguriere FastEndpoints in Program.cs
- [ ] Registriere Repositories in DI Container
- [ ] Konfiguriere Problem Details Error Responses
- [ ] Führe alle Tests aus und verifiziere Success
- [ ] Code Review: Clean Architecture Dependencies
- [ ] Code Review: DDD Patterns
- [ ] Code Review: CQRS Implementation
- [ ] Erstelle README.md mit Setup und Run Anleitung

## 9. Open Questions

### Beantwortet durch User

✅ Clean Architecture Struktur mit 4 Layern  
✅ FastEndpoints Command Bus für CQRS  
✅ Repository Pattern mit In-Memory Collections  
✅ FluentValidation über FastEndpoints  
✅ Result Monad Pattern (Library wird bereitgestellt)  
✅ TDD-Ansatz  

### Noch offen

- [ ] Soll Swagger/OpenAPI aktiviert werden? (Empfehlung: Ja, für einfaches Testing)
- [ ] Welche Test-Framework Präferenz? xUnit oder NUnit? (Empfehlung: xUnit)
- [ ] Sollen Domain Events implementiert werden? (Optional, wenn Zeit)
- [ ] Namespace-Struktur: `JtlTask.Domain.Users` oder `JtlTask.Users.Domain`?

## 10. Definition of Done

Ein Feature gilt als "Done" wenn:

- ✅ Code kompiliert ohne Warnings
- ✅ Unit Tests für Handler existieren und sind grün
- ✅ Integration Tests für Endpoints existieren und sind grün
- ✅ FluentValidation ist korrekt implementiert
- ✅ Result Pattern wird verwendet
- ✅ Repository Pattern ist korrekt umgesetzt
- ✅ Clean Architecture Dependencies sind eingehalten
- ✅ Code folgt C# Coding Conventions
- ✅ DDD Patterns sind erkennbar (Entities, Value Objects, Repositories)
- ✅ Endpoint liefert korrekte HTTP Status Codes
- ✅ Error Responses folgen Problem Details Format

---

**Erstellt am**: 23. Oktober 2025  
**Version**: 1.0  
**Status**: Ready for Implementation
