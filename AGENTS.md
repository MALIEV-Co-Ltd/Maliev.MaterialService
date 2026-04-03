# Maliev.MaterialService Repository Guide

This document contains instructions for AI agents operating in this codebase.

## 1. Environment & Build Commands

This is a **.NET 10.0** project using Visual Studio Solution (.slnx).

All commands run from within this service directory (`B:\maliev\Maliev.MaterialService`).

```powershell
# Build (treats warnings as errors — all must be fixed)
dotnet build Maliev.MaterialService.slnx

# Run all tests
dotnet test Maliev.MaterialService.slnx --verbosity normal

# Run a single test method
dotnet test --filter "FullyQualifiedName~MaterialsControllerTests.GetMaterials_ReturnsSuccessStatusCode"

# Run all tests in a class
dotnet test --filter "FullyQualifiedName~MaterialsControllerTests"

# Run with code coverage
dotnet test Maliev.MaterialService.slnx --collect:"XPlat Code Coverage"

# Format check
dotnet format Maliev.MaterialService.slnx

# EF Core migrations (Infrastructure project only)
dotnet ef migrations add <Name> --project Maliev.MaterialService.Infrastructure --startup-project Maliev.MaterialService.Infrastructure
```

## 2. Project Structure

**Architecture**: Clean Architecture (Api, Application, Domain, Infrastructure, Tests)

```
Maliev.MaterialService/
├── Maliev.MaterialService.Api/           # Controllers, Middleware
├── Maliev.MaterialService.Application/   # Use cases, DTOs, Interfaces, Handlers
├── Maliev.MaterialService.Domain/        # Entities, value objects, domain interfaces
├── Maliev.MaterialService.Infrastructure/ # EF Core DbContext, repositories
├── Maliev.MaterialService.Tests/         # Unit + Integration tests (xUnit)
├── Directory.Build.props                 # Central package versioning
└── Maliev.MaterialService.slnx          # Solution file (.slnx preferred over .sln)
```

## 3. Code Style & Conventions

### C# Naming & Formatting

- **Namespaces**: File-scoped (`namespace Maliev.MaterialService.Api.Controllers;`)
- **Classes/Methods/Properties**: `PascalCase`
- **Private fields**: `_camelCase` (underscore prefix)
- **Parameters/locals**: `camelCase`
- **Async methods**: Suffix with `Async` (e.g., `GetMaterialByIdAsync`)
- **Interfaces**: Prefix with `I` (e.g., `IMaterialService`)
- **DTOs**: Suffix with `Request` or `Response` (e.g., `CreateMaterialRequest`, `MaterialResponse`)
- **Permissions**: GCP-style `{domain}.{plural-resource}.{action}` as `public const string` in a `Permissions` static class
  - Valid: `material.materials.create`, `material.suppliers.update`
  - Invalid: `material.material.create` (singular), `material.create` (missing resource)
- **XML docs**: Required on ALL public methods and properties
- **Nullable**: Enabled (`<Nullable>enable</Nullable>`). Use `?` explicitly
- **Imports**: System first, then third-party, then local. Alphabetize within groups. Remove unused `using`
- **Braces**: Allman style (new line) for methods and control structures. Expression-bodied for properties/accessors
- **Indentation**: 4 spaces, LF line endings, UTF-8, trim trailing whitespace

### C# Patterns

- **DI**: Constructor injection with `private readonly` fields
- **Controllers**: `[ApiController]`, `[ApiVersion("1")]`, `[Route("material/v{version:apiVersion}")]`
  - Inherit from `ControllerBase`. Use `[RequirePermission]` for authorization. Return `ActionResult<T>`.
- **Logging**: `ILogger<T>` with structured placeholders (never interpolate): `_logger.LogInformation("Processing {MaterialId}", materialId)`
- **Error handling**: Global exception middleware. Return `ProblemDetails` / `ErrorResponse` DTOs. Never expose stack traces
- **Manual mapping**: Static extension methods (`ToDto()`, `ToEntity()`). AutoMapper is banned
- **Validation**: `System.ComponentModel.DataAnnotations` on DTOs. FluentValidation is banned
- **Data Access**: Use Entity Framework Core. Entities inherit from `BaseEntity` where applicable. Configure entities in `DbContext` or `IEntityTypeConfiguration`.

### Banned Libraries (Build Will Fail)

| Banned | Use Instead |
|--------|-------------|
| AutoMapper | Manual mapping extensions |
| FluentValidation | DataAnnotations or manual validation |
| FluentAssertions | Standard xUnit `Assert.*` |
| Swashbuckle/Swagger | Scalar (at `/material/scalar`) |
| InMemoryDatabase (EF Core) | Testcontainers with real PostgreSQL |

## 4. Testing Rules

- **Framework**: xUnit with standard `Assert` (`Assert.Equal`, `Assert.NotNull`, etc.)
- **Naming**: `MethodName_StateUnderTest_ExpectedBehavior` or `HTTP_METHOD_Path_Scenario_ExpectedStatus`
- **Coverage**: Minimum 80% per service
- **Integration tests**: `BaseIntegrationTestFactory<TProgram, TDbContext>` with Testcontainers (PostgreSQL, Redis, RabbitMQ). Never InMemoryDatabase
- **System tests** (Tier 3): `AspireTestFixture` with `[Collection("AspireDomainTests")]` — shared AppHost, never one per class
- **Eventual consistency**: Use `TestHelpers.WaitForAsync`. Never `Task.Delay`
- **MassTransit consumers**: Must have consumer tests using `AddMassTransitTestHarness()`

### Testing Strategy (4-Tier Pyramid Context)

This service's tests cover **Tier 1 (Unit)** and **Tier 2 (Service Integration)** of the Maliev testing pyramid:

| Tier | What to Test | Infrastructure |
|------|-------------|---------------|
| **Unit** | Business logic, domain models, service methods with mocked dependencies | None (mocks only) |
| **Service Integration** | API endpoints, database persistence, permission enforcement, input validation | `BaseIntegrationTestFactory` + Testcontainers (Postgres/Redis/RabbitMQ) |

**Tier 3 (System Integration)** — cross-service workflows and event chains — is tested in `Maliev.Aspire.Tests/`.

> Full ecosystem test strategy: `Maliev.Aspire.Tests/TEST_PLAN.md`

## 5. Error Handling

Return standard HTTP status codes:
- `200 OK`: Successful synchronous retrieval/update.
- `201 Created`: Successful creation.
- `204 No Content`: Successful deletion.
- `400 Bad Request`: Validation failures.
- `404 Not Found`: Resource does not exist.
- `409 Conflict`: Concurrency issues or duplicate keys.

## 6. Mandatory Rules

- **`TreatWarningsAsErrors = true`**: Zero warnings allowed. No suppression
- **`[RequirePermission("material.resources.action")]`**: On all endpoints, not plain `[Authorize]`
- **API versioning**: All routes versioned (`v1/`)
- **Service prefix**: Routes prefixed with `/material`
- **Scalar docs**: Configured at `/material/scalar`
- **Secrets**: Never hardcoded. Use GCP Secret Manager or environment variables
- **Async/await**: All the way down. Pass `CancellationToken`
- **EF Core Design package**: Only in Infrastructure project, never in Api
- **PostgreSQL xmin**: Shadow property only — `entity.Property<uint>("xmin").HasColumnType("xid").IsRowVersion()`. Never add entity property
- **Temporary files**: Generate in `/temp` folder, clean up afterwards

### EF Core / PostgreSQL xmin Concurrency — Mandatory Pattern

Use shadow property ONLY. Never add a Xmin/xmin property to domain entities.
```csharp
entity.Property<uint>("xmin").HasColumnType("xid").IsRowVersion();
```
- ❌ Never use `UseXminAsConcurrencyToken()` (removed in Npgsql EF v7)
- ❌ Never use entity property `public uint Xmin { get; set; }` or `public uint xmin { get; set; }`
- ❌ Never use `.Ignore(e => e.Xmin)` — remove the entity property instead

## 7. Git Rules

- Each `Maliev.*` folder is an independent git repo. `cd` into it before git commands
- **Commit early and often** after every meaningful unit of work. Do not accumulate changes
- **Never use `git checkout` to restore files** — commit first, then `git revert` or `git reset --soft`
- Feature branches merged to `develop` via PR. Do not push without being asked
