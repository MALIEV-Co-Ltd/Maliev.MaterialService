# Tasks: Permission-Based Authorization Migration

**Feature**: Permission-Based Authorization Migration
**Plan**: [specs/002-iam-integration/plan.md](plan.md)
**Spec**: [specs/002-iam-integration/spec.md](spec.md)

## Phase 1: Setup

- [x] T001 Update appsettings.json with OIDC and Redis configuration in Maliev.MaterialService.Api/appsettings.json
- [x] T002 Add required NuGet packages (Microsoft.AspNetCore.Authentication.OpenIdConnect) to Maliev.MaterialService.Api/Maliev.MaterialService.Api.csproj

## Phase 2: Foundational (IAM & Messaging)

- [x] T003 Configure IAM Client registration in Maliev.MaterialService.Api/Program.cs using `builder.Services.AddIAMClient(...)`
- [x] T004 Create MaterialIAMRegistrationService inheriting from `IAMRegistrationService` in Maliev.MaterialService.Api/Services/Auth/MaterialIAMRegistrationService.cs
- [x] T005 Define 14 granular permissions in MaterialIAMRegistrationService following standard format
- [x] T006 Define 4 predefined roles and their permission mappings in MaterialIAMRegistrationService
- [x] T007 Register MaterialIAMRegistrationService as a Hosted Service in Maliev.MaterialService.Api/Program.cs
- [x] T008 [P] Configure structured JSON logging for auth events in Maliev.MaterialService.Api/appsettings.json
- [x] T009 Register business metrics for authorization success/failure with mandatory tags in Maliev.MaterialService.Api/Program.cs
- [x] T010 [P] Configure MassTransit for publishing Material events using shared contracts
- [x] T011 [P] Implement `MaterialCreated` event publishing in IMaterialService
- [x] T012 [P] Implement `MaterialUpdated` event publishing in IMaterialService
- [x] T013 [P] Implement `MaterialDeleted` event publishing in IMaterialService

## Phase 3: User Story 1 - Full System Administration [US1]

**Goal**: System Admin has unrestricted access.
**Independent Test**: Authenticate as `material-admin` and successfully call delete and export endpoints.

- [x] T014 [US1] Apply [RequirePermission("material.materials.delete", IsCritical = true)] to MaterialsController in Maliev.MaterialService.Api/Controllers/MaterialsController.cs
- [x] T015 [US1] Apply [RequirePermission("material.materials.export")] to export actions in Maliev.MaterialService.Api/Controllers/BulkOperationsController.cs
- [x] T016 [P] [US1] Implement Admin integration tests using `IAMTestHelpers.WithTestAuth()` in Maliev.MaterialService.Tests/Integration/AuthorizationTests.cs

## Phase 4: User Story 2 - Material Management [US2]

**Goal**: Material Manager can manage entities but cannot delete materials.
**Independent Test**: `material-manager` can create/update materials/categories but gets 403 on delete material.

- [x] T017 [US2] Apply [RequirePermission("material.materials.create")] and [RequirePermission("material.materials.update")] to MaterialsController
- [x] T018 [US2] Apply [RequirePermission] to Category and Supplier endpoints in ReferenceDataController.cs and SuppliersController.cs
- [x] T019 [P] [US2] Implement Manager integration tests in Maliev.MaterialService.Tests/Integration/AuthorizationTests.cs

## Phase 5: User Story 3 - Daily Inventory Operations [US3]

**Goal**: Material Clerk can perform inventory counts and create materials.
**Independent Test**: `material-clerk` can perform count but cannot transfer or delete.

- [x] T020 [US3] Apply [RequirePermission("material.inventory.count")] and [RequirePermission("material.inventory.adjust")] to relevant actions
- [x] T021 [P] [US3] Implement Clerk integration tests in Maliev.MaterialService.Tests/Integration/AuthorizationTests.cs

## Phase 6: User Story 4 - Read-Only Auditing [US4]

**Goal**: Viewer has read access; Public has limited read access.
**Independent Test**: `material-viewer` can see everything; Anonymous can only see material list.

- [x] T022 [US4] Apply [RequirePermission("material.materials.read")] to read actions
- [x] T023 [US4] Ensure [AllowAnonymous] is correctly placed for public material read access
- [x] T024 [P] [US4] Implement Viewer and Anonymous integration tests in Maliev.MaterialService.Tests/Integration/AuthorizationTests.cs

## Phase 7: Polish & Cross-Cutting

- [x] T025 Verify enhanced audit logs for critical permission checks (marked with IsCritical = true)
- [x] T026 [P] Verify business metrics with mandatory tags are emitted correctly
- [x] T027 [P] Ensure zero compiler warnings in all projects
- [x] T028 Update OpenAPI documentation to reflect authorization requirements

## Dependency Graph

```text
Phase 1 (Setup)
      |
Phase 2 (Foundational)
      |
      +----------------------------+
      |                            |
Phase 3 (US1: Admin)         Phase 4 (US2: Manager)
      |                            |
Phase 5 (US3: Clerk)         Phase 6 (US4: Viewer/Public)
      |                            |
      +----------------------------+
      |
Phase 7 (Polish)
```

## Parallel Execution Examples

- **Foundational**: T006, T007, T008 can be done in parallel (configurations).
- **US1**: T018 and T019 can be done in parallel.
- **US2**: T021 and T022 can be done in parallel.
- **Cross-Story**: Once Phase 2 is done, US1, US2, US3, and US4 phases can technically be started in parallel if different controllers are involved.

## Implementation Strategy

1. **MVP**: Complete Phase 1 and Phase 2, then US1. This establishes the framework and proves the most critical role.
2. **Incremental**: Add US2, then US3, then US4. Each adds more granular control.
3. **Verify**: Use Testcontainers for PostgreSQL and Redis in all integration tests as per Constitution.
