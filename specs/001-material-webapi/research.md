# Research Summary: Material WebAPI Service

**Branch**: `001-material-webapi` | **Date**: 2024-11-18 | **Plan**: [link to plan.md]

## Key Technical Decisions & Rationale

This section summarizes the core technical decisions made for the Material WebAPI Service, drawing directly from the provided technical and architectural plan. The detailed plan already specifies concrete technologies and patterns, thus eliminating the need for extensive research into alternatives for most components.

### 1. Core Technology Stack

*   **Decision**: .NET 10 WebAPI, Entity Framework Core 9.0.10, PostgreSQL 18, Npgsql 9.0.4.
*   **Rationale**: Adherence to organizational standards and leveraging a robust, high-performance ecosystem for microservice development. PostgreSQL is the mandated database for all persistent state.
*   **Alternatives Considered**: None, as these are prescribed technologies.

### 2. API Documentation & Discovery

*   **Decision**: Scalar 1.2.42 (Microsoft.AspNetCore.OpenApi 9.0.0) for API documentation.
*   **Rationale**: Provides interactive API documentation with a better user experience than traditional Swagger UI, aligning with internal preferences.
*   **Alternatives Considered**: Swagger UI (rejected due to preference for Scalar).

### 3. Authentication & Authorization

*   **Decision**: JWT Bearer tokens (Microsoft.AspNetCore.Authentication.JwtBearer 9.0.8) with RSA public key validation, RBAC using standard ASP.NET Core Identity claims.
*   **Rationale**: Standard, secure, and scalable approach for microservice authentication. Asymmetric cryptography (RSA) ensures token integrity and allows for distributed validation without sharing private keys. RBAC provides fine-grained access control.
*   **Alternatives Considered**: None, as this is a prescribed security pattern.

### 4. Logging & Observability

*   **Decision**: Serilog 8.0.2 for structured JSON logging to stdout, Prometheus.AspNetCore 8.2.1 for metrics, Correlation ID middleware.
*   **Rationale**: Structured logging facilitates centralized log analysis. Prometheus integration provides comprehensive monitoring and alerting capabilities. Correlation IDs enable tracing requests across distributed services.
*   **Alternatives Considered**: None, as these are prescribed observability tools.

### 5. Resilience & Performance

*   **Decision**: Polly 8.5.0 with Microsoft.Extensions.Http.Resilience 9.0.0 for HTTP client resilience (retry, circuit breaker), ASP.NET Core 10.0 built-in rate limiting, in-process LRU cache, distributed Redis cache (StackExchangeRedis 9.0.0), response compression.
*   **Rationale**: Essential for building robust microservices that can withstand transient failures, control traffic, and deliver high performance. Multi-layered caching minimizes database load and ensures low latency reads.
*   **Alternatives Considered**: Older Polly versions (rejected in favor of v8 `AddStandardResilienceHandler`).

### 6. Messaging

*   **Decision**: MassTransit 8.3.4 with RabbitMQ 7.0.0 for inter-service messaging.
*   **Rationale**: Enables event-driven architecture for asynchronous communication between services, promoting loose coupling and scalability.
*   **Alternatives Considered**: None, as this is a prescribed messaging solution.

### 7. Configuration Management

*   **Decision**: All secrets via Google Secret Manager, environment variables for configuration, double underscore naming convention for IConfiguration binding.
*   **Rationale**: Adherence to security best practices (no secrets in source code) and flexible, environment-agnostic deployment.
*   **Alternatives Considered**: None, as this is a prescribed configuration pattern.

### 8. Testing Strategy

*   **Decision**: Comprehensive testing including unit, integration, and contract tests. Use of `TestWebApplicationFactory`, actual PostgreSQL (via Testcontainers), mock authentication (`TestAuthHandler`), and mocked external services.
*   **Rationale**: Ensures high quality, validates real database behavior, and provides confidence in deployments. Testcontainers provide isolated and realistic database environments for integration tests.
*   **Alternatives Considered**: In-memory databases (explicitly rejected by constitution).

### 9. Routing & Deployment

*   **Decision**: Direct path prefixes for all routes (e.g., `/servicename/v1/resource`), explicit OpenAPI and Scalar configuration, Docker containerization, GitHub Actions CI/CD, GitOps deployment via Kustomize.
*   **Rationale**: Addresses critical routing issues identified in previous projects, ensures consistent and predictable API paths, and aligns with the organization's automated deployment strategy.
*   **Alternatives Considered**: `UsePathBase` (explicitly rejected due to known issues).

### 10. Data Persistence & Concurrency

*   **Decision**: EF Core FluentAPI for schema configuration, snake_case naming, optimistic concurrency using `RowVersion` (manual increment for PostgreSQL), transactional patterns, `IDesignTimeDbContextFactory` for migrations.
*   **Rationale**: Standardized and robust approach for data access, ensuring data integrity, handling concurrent updates, and supporting EF Core migration tooling.
*   **Alternatives Considered**: PostgreSQL `ValueGeneratedOnAddOrUpdate` for `RowVersion` (explicitly rejected due to known failure mode).
