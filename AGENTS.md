# Maliev.MaterialService Repository Guide

This document contains instructions for AI agents operating in this codebase.

## 1. Environment & Build Commands

This is a **.NET 10.0** project using Visual Studio Solution (.slnx).

*   **Build Solution:**
    ```bash
    dotnet build
    ```
*   **Run All Tests:**
    ```bash
    dotnet test
    ```
*   **Run a Single Test:**
    ```bash
    dotnet test --filter "FullyQualifiedName~NameOfTestClass.NameOfTestMethod"
    ```
    *Example:* `dotnet test --filter "FullyQualifiedName~MaterialsControllerTests.GetMaterials_ReturnsSuccessStatusCode"`
*   **Run Tests in a specific project:**
    ```bash
    dotnet test Maliev.MaterialService.Tests/Maliev.MaterialService.Tests.csproj
    ```

## 2. Project Structure

**Architecture**: Clean Architecture (Api, Application, Domain, Infrastructure, Tests)

*   **`Maliev.MaterialService.Api`**: Controllers, Middleware.
*   **`Maliev.MaterialService.Application`**: Use cases, handlers, DTOs.
*   **`Maliev.MaterialService.Domain`**: Entities, interfaces, value objects.
*   **`Maliev.MaterialService.Infrastructure`**: EF Core, repositories.
*   **`Maliev.MaterialService.Tests`**: xUnit Test project (Integration and Unit tests).

## 3. Code Style & Conventions

Follow the existing patterns in the codebase strictly.

### Formatting & Syntax
*   **Namespaces:** Use **File-scoped namespaces** (e.g., `namespace Maliev.MaterialService.Api.Controllers;`).
*   **Nullable Types:** Nullable reference types are **enabled**. Use `string?` for optional values.
*   **Async/Await:** Use `async Task` for I/O bound operations. Avoid `.Result` or `.Wait()`.
*   **Braces:** Use standard C# bracing (Allman style - open brace on new line).
*   **Imports:** Remove unused usings. Place `System` directives first, then 3rd party, then solution namespaces.

### Naming Conventions
*   **Classes/Methods/Properties:** `PascalCase`.
*   **Parameters/Locals:** `camelCase`.
*   **Private Fields:** `_camelCase` (underscore prefix).
    ```csharp
    private readonly IMaterialService _materialService;
    ```
*   **Interfaces:** Prefix with `I` (e.g., `IMaterialService`).
*   **DTOs:** Suffix with `Request` or `Response` (e.g., `CreateMaterialRequest`, `MaterialResponse`).
*   **Async Methods:** Suffix with `Async` (e.g., `GetMaterialByIdAsync`).

### Architecture & Patterns
*   **Controllers:**
    *   Inherit from `ControllerBase`.
    *   Decorate with `[ApiController]`, `[ApiVersion]`, `[Route]`.
    *   Use `[RequirePermission]` for authorization.
    *   Return `ActionResult<T>`.
    *   Wrap logic in `try-catch` blocks for robust error handling.
*   **Data Access:**
    *   Use Entity Framework Core.
    *   Entities inherit from `BaseEntity` where applicable.
    *   Configure entities in `DbContext` or `IEntityTypeConfiguration`.
*   **Dependency Injection:**
    *   Register services in `Program.cs`.
    *   Use Constructor Injection.

### Comments & Documentation
*   **Controllers/Actions:** XML Documentation (`/// <summary>`) is **mandatory** for public API endpoints (used for OpenAPI/Swagger).
    *   Include `<remarks>` for implementation details.
    *   Include `<response>` tags for status codes.

### Testing
*   **Framework:** xUnit.
*   **Integration Tests:**
    *   Use `IClassFixture<IntegrationTestWebAppFactory>`.
    *   Create scoped services and DbContexts within tests if needed.
    *   Clean database state between tests.
*   **Naming:** `MethodName_StateUnder_ExpectedBehavior`.

### Testing Strategy (4-Tier Pyramid Context)

This service's tests cover **Tier 1 (Unit)** and **Tier 2 (Service Integration)** of the Maliev testing pyramid:

| Tier | What to Test | Infrastructure |
|------|-------------|---------------|
| **Unit** | Business logic, domain models, service methods with mocked dependencies | None (mocks only) |
| **Service Integration** | API endpoints, database persistence, permission enforcement, input validation | `BaseIntegrationTestFactory` + Testcontainers (Postgres/Redis/RabbitMQ) |

**Tier 3 (System Integration)** — cross-service workflows and event chains — is tested in `Maliev.Aspire.Tests/`.

#### Key Rules
- Use `BaseIntegrationTestFactory<TProgram, TDbContext>` for integration tests (real Testcontainers, never InMemoryDatabase)
- Test naming: `MethodName_StateUnderTest_ExpectedBehavior`
- Minimum 80% code coverage
- Use `[Fact]` for single cases, `[Theory]` for parameterized tests

> Full ecosystem test strategy: `Maliev.Aspire.Tests/TEST_PLAN.md`

## 4. Error Handling
*   Use structured logging via `ILogger`.
*   Return standard HTTP status codes:
    *   `200 OK`: Successful synchronous retrieval/update.
    *   `201 Created`: Successful creation.
    *   `204 No Content`: Successful deletion.
    *   `400 Bad Request`: Validation failures.
    *   `404 Not Found`: Resource does not exist.
    *   `409 Conflict`: Concurrency issues or duplicate keys.


## Git & Version Control — Mandatory Rules

### 🚨 CRITICAL: Always Commit Code Changes (Non-Negotiable)
- **You MUST commit your changes to the local repository after completing any meaningful unit of work.**
- **Never accumulate uncommitted changes.** Do not wait until end of session or until something breaks.
- **Commit early and often** — if a change is meaningful (even a small fix or refactor), commit it.
- **You do NOT need to push to remote** — local commits are sufficient to protect against accidental loss.
- **If you are unsure whether to commit, commit anyway.** Extra commits are harmless; lost work is irreversible.
- This rule applies even if you are just "testing" or "exploring" — use git branches to isolate experimental work and commit those changes too.

### 🚨 CRITICAL: Never Use `git checkout` to Restore Broken Files
- **NEVER use `git checkout` to restore or recover files.** This operation discards uncommitted changes permanently and will result in data loss.
- **To undo/recover from broken files: first commit your current changes, then use `git revert` or `git reset --soft` to safely undo.**

## Database & EF Core — Mandatory Rules

### EF Core Design Package
- ❌ `Microsoft.EntityFrameworkCore.Design` MUST NOT be in Api projects
- ✅ It belongs ONLY in the Infrastructure (or Data) project where migrations live
- Migration commands must target Infrastructure as both project and startup-project (since EF Core Design package is in Infrastructure):
  ```
  dotnet ef migrations add <Name> --project Maliev.<Domain>Service.Infrastructure --startup-project Maliev.<Domain>Service.Infrastructure
  ```

### PostgreSQL xmin Concurrency — Mandatory Pattern
Use shadow property ONLY. Never add a Xmin/xmin property to domain entities.
```csharp
entity.Property<uint>("xmin").HasColumnType("xid").IsRowVersion();
```
- ❌ Never use `UseXminAsConcurrencyToken()` (removed in Npgsql EF v7)
- ❌ Never use entity property `public uint Xmin { get; set; }` or `public uint xmin { get; set; }`
- ❌ Never use `.Ignore(e => e.Xmin)` — remove the entity property instead
