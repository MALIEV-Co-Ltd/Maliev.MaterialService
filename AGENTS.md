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

*   **`Maliev.MaterialService.Api`**: Main Web API project (Controllers, Services, DTOs).
*   **`Maliev.MaterialService.Data`**: Data Access Layer (EF Core DbContext, Entities, Migrations).
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

## 4. Error Handling
*   Use structured logging via `ILogger`.
*   Return standard HTTP status codes:
    *   `200 OK`: Successful synchronous retrieval/update.
    *   `201 Created`: Successful creation.
    *   `204 No Content`: Successful deletion.
    *   `400 Bad Request`: Validation failures.
    *   `404 Not Found`: Resource does not exist.
    *   `409 Conflict`: Concurrency issues or duplicate keys.
