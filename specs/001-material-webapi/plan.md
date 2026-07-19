# Implementation Plan: Material WebAPI Service

**Branch**: `001-material-webapi` | **Date**: 2024-11-18 | **Spec**: specs/001-material-webapi/spec.md
**Input**: Feature specification from `/specs/001-material-webapi/spec.md`

## Summary

Implement a Material WebAPI service as the central authoritative source for all material data within Maliev Manufacturing. This service will provide full CRUD operations for materials, store rich metadata, enforce strict validation, and utilize a multi-layered caching strategy (in-process LRU and distributed Redis) to ensure sub-50ms response times for common read operations. It will support bulk import/export, role-based access control for administrative functions, and expose health, readiness, and Prometheus metrics endpoints. The implementation will adhere to Clean Architecture principles, leveraging .NET 10, PostgreSQL 18, Entity Framework Core 9.0.10, and a comprehensive suite of modern libraries for authentication (JWT), logging (Serilog), resilience (Polly), messaging (MassTransit with RabbitMQ), and API versioning.

## Technical Context

**Language/Version**: .NET 10 (C#)
**Primary Dependencies**: Entity Framework Core 9.0.10, Npgsql 9.0.4, Scalar 1.2.42 (Microsoft.AspNetCore.OpenApi 9.0.0), Microsoft.AspNetCore.Authentication.JwtBearer 9.0.8, Serilog 8.0.2, FluentValidation 11.3.0, Polly 8.5.0 (Microsoft.Extensions.Http.Resilience 9.0.0), ASP.NET Core 10.0 built-in rate limiting, Asp.Versioning.Http 8.1.0, AspNetCore.HealthChecks.UI.Client 9.0.0, MassTransit 8.3.4, RabbitMQ 7.0.0, StackExchangeRedis 9.0.0, Prometheus.AspNetCore 8.2.1.
**Storage**: PostgreSQL 18
**Testing**: Unit tests, integration tests, contract tests using TestWebApplicationFactory, actual PostgreSQL database (Testcontainers for isolation), mock authentication (TestAuthHandler), mocked external services. Minimum 80% coverage for critical functionality.
**Target Platform**: Containerized (Docker) for Kubernetes deployment.
**Project Type**: WebAPI (Microservice)
**Performance Goals**: Sub-50ms response times for 95% of common read operations.
**Constraints**:
*   Stateless microservice.
*   Optimistic concurrency control using RowVersion.
*   Audit trail for all create, update, cancel, approve actions.
*   Validation using FluentValidation.
*   Global exception handling via ExceptionHandlingMiddleware.
*   Request logging via RequestLoggingMiddleware.
*   Scalar UI exposed at `/material/scalar/v1` (development only).
*   All secrets injected via environment variables from Google Secret Manager.
*   Database connection string: `ConnectionStrings__ServiceDbContext`.
*   Database name: `material_app_db` (snake_case postfix).
*   EF Core migrations created but NOT auto-applied on startup.
*   Health check endpoints at `/material/liveness` and `/material/readiness`.
*   Direct path prefixes for routing (NO `UsePathBase`).
*   Middleware pipeline order is mandatory.
*   External service clients use typed `HttpClient` with Polly v8 `AddStandardResilienceHandler`.
*   PostgreSQL Testcontainers for integration tests.
*   Database cleanup using DELETE with C# exception handling.
*   `UserManager` and `SignInManager` require complex mock setup for testing.
*   Services using `DbContext` must implement `IAsyncDisposable`.
*   JSON null serialization configured to ignore `WhenWritingNull`.
*   Metrics PII regex precision.
*   `#pragma warning disable` used sparingly and documented.
*   Background services tested by invoking methods directly.
*   `IDesignTimeDbContextFactory<TContext>` required for migrations.
*   PostgreSQL `RowVersion` requires manual increment in `DbContext.SaveChanges` override.
*   Transactional database patterns for multi-step operations.
*   Nested transaction handling with `CurrentTransaction` check.
*   FluentAPI for EF Core configurations.
*   Enums stored as strings using `HasConversion`.
*   `RowVersion` passed as Base64 string in update requests, validated before conversion.
*   Mechanical properties implemented as flexible key-value pairs with unit of measurement support in MaterialMechanicalProperty junction table.
*   Extensibility achieved through configurable lookup tables for processes, methods, and properties.
*   Security policy configurable for read endpoints (public vs internal) via environment variables.
*   Optimistic concurrency control implemented using RowVersion with conflict resolution via HTTP 409 responses.
*   Database backup and recovery procedures implemented with PostgreSQL native tools for point-in-time recovery.
*   Controllers inherit from `ControllerBase`, use `ApiController`, `ApiVersion`, `Route`, `Authorize`, `EnableRateLimiting` attributes with URL-based versioning (e.g., /api/v1/).
*   Separate DTOs for Request and Response.
*   `AutoMapper` is optional.
*   Global exception handling maps exception types to HTTP status codes.
*   Structured logging using Serilog.
*   Correlation ID middleware for request tracking.
*   Security headers middleware.
*   Response compression middleware.
*   `AsNoTracking()` for read-only queries.
*   State machine pattern for entity lifecycle.
*   Polymorphic relationship pattern (`owner_type`, `owner_id`).
*   Deferred deletion pattern with retry.
*   Caching implements stale-while-revalidate pattern with circuit breaker to prevent cache stampede.
*   Messaging system uses MassTransit with RabbitMQ for event-driven communication and eventual consistency.
*   CI/CD workflows create pull requests for deployments.
*   `ServiceMonitor` for Prometheus metrics.
*   Comprehensive `README.md`.
*   Multiple related entities structured as separate controllers/services.
*   Cascade operations defined explicitly.
*   `TreatWarningsAsErrors` enabled.

**Scale/Scope**: Maliev Manufacturing services (enterprise-level).

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

-   **I. Service Autonomy**: **PASS**. The plan emphasizes a microservice architecture with its own database and API interactions.
-   **II. Explicit Contracts**: **PASS**. OpenAPI/Swagger documentation is explicitly mentioned, and API versioning is included.
-   **III. Test-First Development**: **PASS**. The plan mandates tests immediately after spec approval, covers unit, integration, and contract tests, and specifies 80% coverage.
-   **IV. PostgreSQL-Only Testing**: **PASS**. Explicitly states all tests must use PostgreSQL, no in-memory databases, and uses Testcontainers for real instances.
-   **V. Auditability & Observability**: **PASS**. Structured JSON logging, audit trail, health checks, and Prometheus metrics are all included.
-   **VI. Security & Compliance**: **PASS**. JWT authentication, RBAC, secrets management via Google Secret Manager, and security headers are covered.
-   **VII. Secrets Management & Configuration Security**: **PASS**. Explicitly states no secrets in source code and injection from Google Secret Manager.
-   **VIII. Zero Warnings Policy**: **PASS**. `TreatWarningsAsErrors` is enabled, and builds must produce zero warnings.
-   **IX. Clean Project Artifacts**: **PASS**. `.dockerignore` and `.gitignore` are mentioned for excluding artifacts.
-   **X. Docker Best Practices**: **PASS**. Uses built-in `app` user, `chown` before `USER`, multi-stage builds, .NET 10 images, health checks, and layer optimization.
-   **XI. Simplicity & Maintainability**: **PASS**. Clean Architecture, stateless design, and consistent patterns contribute to simplicity.
-   **XII. Business Metrics & Analytics**: **PASS**. Prometheus metrics are explicitly required, including custom business metrics such as material_created_total, material_updated_total, material_deleted_total, and query_operation_duration_seconds to support analytics pipeline requirements.

## Project Structure

### Documentation (this feature)

```text
specs/001-material-webapi/
├── plan.md              # This file (/speckit.plan command output)
├── research.md          # Phase 0 output (/speckit.plan command)
├── data-model.md        # Phase 1 output (/speckit.plan command)
├── quickstart.md        # Phase 1 output (/speckit.plan command)
├── contracts/           # Phase 1 output (/speckit.plan command)
└── tasks.md             # Phase 2 output (/speckit.tasks command - NOT created by /speckit.plan)
```

### Source Code (repository root)

```text
Maliev.MaterialService/
├── Maliev.MaterialService.Api/
│   ├── Controllers/
│   ├── DTOs/
│   ├── Services/
│   ├── Middleware/
│   ├── Validators/
│   ├── Program.cs
│   ├── Dockerfile
│   └── launchSettings.json
├── Maliev.MaterialService.Data/
│   ├── DbContext/
│   ├── Entities/
│   │   ├── Material.cs
│   │   ├── MaterialMechanicalProperty.cs  # Junction table with flexible property values
│   │   └── ...
│   ├── Configurations/
│   ├── Migrations/
│   └── ServiceDbContextFactory.cs
├── Maliev.MaterialService.Tests/
│   ├── Unit/
│   ├── Integration/
│   ├── Contract/
│   └── TestAuthHandler.cs
├── .dockerignore
├── .gitignore
└── Maliev.MaterialService.sln
```

**Structure Decision**: The project will follow a multi-project solution structure with dedicated projects for API, Data, and Tests, adhering to the specified naming conventions. This aligns with Clean Architecture principles and promotes separation of concerns.

## Complexity Tracking

(No violations found that require justification.)