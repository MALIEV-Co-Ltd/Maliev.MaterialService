# Implementation Plan: Material Pricing Fields

**Branch**: `001-material-pricing-fields` | **Date**: 2026-02-22 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/001-material-pricing-fields/spec.md`

## Summary

Add `Density`, `CostPerKg`, and `ProcessParameters` fields to the Material entity to enable PricingService integration for weight-based quote calculations. Technical approach: JSONB for flexible ProcessParameters storage, decimal for precise pricing data, database migration with sensible defaults.

## Technical Context

**Language/Version**: .NET 10.0 / C# 14
**Primary Dependencies**: ASP.NET Core, Entity Framework Core 10, Npgsql (PostgreSQL)
**Storage**: PostgreSQL with JSONB support for ProcessParameters
**Testing**: xUnit with Testcontainers (PostgreSQL)
**Target Platform**: Linux server (containerized)
**Project Type**: web-service (REST API microservice)
**Performance Goals**: Standard REST API performance (<200ms p95)
**Constraints**: Must maintain backward compatibility, no breaking changes to existing API
**Scale/Scope**: Single microservice, ~10 entities

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

| Principle | Status | Notes |
|-----------|--------|-------|
| I. Service Autonomy | PASS | Changes are internal to MaterialService only |
| II. Explicit Contracts | PASS | API version maintained, additive changes only |
| III. Test-First Development | PASS | Tests authored before implementation (Red-Green-Refactor) |
| IV. Real Infrastructure Testing | PASS | Using Testcontainers with PostgreSQL, tests written first |
| V. Auditability & Observability | PASS | No changes to logging pattern |
| VI. Security & Compliance | PASS | No security-relevant changes |
| VII. Secrets Management | PASS | No secrets involved |
| VIII. Zero Warnings Policy | PASS | Build must have zero warnings |
| IX. Clean Project Artifacts | PASS | No new artifacts in root |
| X. Docker Best Practices | PASS | No Dockerfile changes needed |
| XI. Simplicity & Maintainability | PASS | Simple field additions, no over-engineering |
| XII. Business Metrics | PASS | No metrics changes required |
| XIII. .NET Aspire Integration | PASS | No changes to ServiceDefaults |
| XIV. Code Quality | PASS | No AutoMapper, FluentValidation, or FluentAssertions |
| XV. Project Structure | PASS | Follows flat structure convention |
| XVI. CI/CD Standards | PASS | No workflow changes needed |

## Project Structure

### Documentation (this feature)

```text
specs/001-material-pricing-fields/
├── plan.md              # This file
├── research.md          # Phase 0 output
├── data-model.md        # Phase 1 output
├── quickstart.md        # Phase 1 output
├── contracts/           # Phase 1 output
└── tasks.md             # Phase 2 output
```

### Source Code (repository root)

```text
Maliev.MaterialService.Api/
├── Controllers/
│   ├── MaterialsController.cs
│   ├── InventoryController.cs  → StockController.cs (rename)
│   └── ...
├── DTOs/Materials/
│   ├── MaterialResponse.cs      (modify)
│   ├── CreateMaterialRequest.cs (modify)
│   └── UpdateMaterialRequest.cs (modify)
├── Mapping/
│   └── DomainToDtoMapper.cs     (modify)
└── ...

Maliev.MaterialService.Data/
├── Entities/
│   └── Material.cs              (modify)
├── Configurations/
│   └── MaterialConfiguration.cs (modify)
└── Migrations/
    └── (new migration)

Maliev.MaterialService.Tests/
├── Integration/
│   ├── MaterialPricingIntegrationTests.cs (new)
│   ├── MaterialProcessParametersIntegrationTests.cs (new)
│   └── StockControllerIntegrationTests.cs (new)
└── ...
```

**Structure Decision**: Flat structure with Api, Data, and Tests projects at root level, following constitutional requirement XV.

## Complexity Tracking

> No violations to justify - all changes follow constitutional guidelines.
