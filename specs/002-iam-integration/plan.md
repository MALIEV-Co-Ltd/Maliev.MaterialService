# Implementation Plan: Permission-Based Authorization Migration

**Branch**: `002-iam-integration` | **Date**: 2025-12-22 | **Spec**: [specs/002-iam-integration/spec.md](spec.md)
**Input**: Feature specification from `/specs/002-iam-integration/spec.md`

## Summary

Migrate the existing MaterialService authorization to a granular permission-based model integrated with the centralized Maliev IAM Service. Service permissions (14 total) and 4 predefined roles (`material-admin`, `material-manager`, `material-clerk`, `material-viewer`) will be registered with IAM on startup via a hosted `MaterialIAMRegistrationService`. Enforcement will be implemented at the Controller/Action level using the standard `[RequirePermission(...)]` attribute from `Maliev.Aspire.ServiceDefaults`, which evaluates claims directly from the JWT.

## Technical Context

**Language/Version**: C# / .NET 10  
**Primary Dependencies**: 
- Maliev.Aspire.ServiceDefaults (NuGet)
- Microsoft.AspNetCore.Authentication.JwtBearer
- MassTransit.RabbitMQ
**Storage**: Redis (Cache)  
**Testing**: xUnit, Testcontainers (Redis)  
**Target Platform**: Linux (Docker)
**Project Type**: Web API (ASP.NET Core)  
**Performance Goals**: Permission check overhead < 1ms (claims-based evaluation)
**Constraints**: Zero warnings policy, no AutoMapper/FluentValidation/FluentAssertions.  
**Scale/Scope**: 14 permissions, 4 roles, covering all MaterialService endpoints.

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- [x] **Service Autonomy**: Service owns its own database for Role-Permission mappings.
- [x] **Explicit Contracts**: OpenAPI/Scalar documentation will be updated for auth requirements.
- [x] **Test-First Development**: Integration tests using Testcontainers will be written before implementation.
- [x] **Real Infrastructure Testing**: Testcontainers for PostgreSQL and Redis are mandatory.
- [x] **No AutoMapper/FluentValidation/FluentAssertions**: Manual mapping and standard xUnit/DataAnnotations used.
- [x] **Zero Warnings Policy**: Build configured to treat warnings as errors.
- [x] **Flat Project Structure**: Projects located at root (Maliev.MaterialService.Api, Maliev.MaterialService.Data, Maliev.MaterialService.Tests).
- [x] **Business Metrics**: Telemetry for auth success/failures and performance included.

## Project Structure

### Documentation (this feature)

```text
specs/002-iam-integration/
├── plan.md              # This file
├── research.md          # Phase 0 output
├── data-model.md        # Phase 1 output
├── quickstart.md        # Phase 1 output
├── contracts/           # Phase 1 output
└── tasks.md             # Phase 2 output (generated via /speckit.tasks)
```

### Source Code (repository root)

```text
Maliev.MaterialService.Api/
├── Controllers/         # Updated with [RequirePermission(...)] attributes
├── Services/
│   ├── Auth/            # MaterialIAMRegistrationService
│   └── Messaging/       # Event publishing
├── appsettings.json     # IAM & Redis config

Maliev.MaterialService.Data/
├── Entities/            # Material, Category, etc (No RBAC tables)
├── DbContext/           
└── Configurations/

Maliev.MaterialService.Tests/
├── Integration/         # Updated with permission-based tests
└── Fixtures/            # Testcontainers configuration
```

**Structure Decision**: Standard flat .NET microservice structure as per Constitution XV.

## Complexity Tracking

> **Fill ONLY if Constitution Check has violations that must be justified**

(No violations currently identified)