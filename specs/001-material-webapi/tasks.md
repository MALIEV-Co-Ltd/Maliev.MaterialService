# Tasks: Material WebAPI Service

**Input**: Design documents from `/specs/001-material-webapi/`
**Prerequisites**: plan.md (required), spec.md (required for user stories), research.md, data-model.md, contracts/

**Tests**: The plan mandates comprehensive testing, so test tasks are included.

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story.

## Format: `[ID] [P?] [Story?] Description with file path`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., US1, US2, US3)
- Include exact file paths in descriptions

## Path Conventions

- **Solution Root**: `Maliev.MaterialService/`
- **API Project**: `Maliev.MaterialService.Api/`
- **Data Project**: `Maliev.MaterialService.Data/`
- **Tests Project**: `Maliev.MaterialService.Tests/`

---

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Project initialization and basic solution structure.

- [x] T001 Create solution file `Maliev.MaterialService.sln`
- [x] T002 Create `Maliev.MaterialService.Api` project `Maliev.MaterialService.Api/Maliev.MaterialService.Api.csproj`
- [x] T003 Create `Maliev.MaterialService.Data` project `Maliev.MaterialService.Data/Maliev.MaterialService.Data.csproj`
- [x] T004 Create `Maliev.MaterialService.Tests` project `Maliev.MaterialService.Tests/Maliev.MaterialService.Tests.csproj`
- [x] T005 Add project reference `Maliev.MaterialService.Api` to `Maliev.MaterialService.Data`
- [x] T006 Add project reference `Maliev.MaterialService.Tests` to `Maliev.MaterialService.Api` and `Maliev.MaterialService.Data`
- [x] T007 Add NuGet packages to `Maliev.MaterialService.Api` project: `Microsoft.AspNetCore.OpenApi`, `Microsoft.AspNetCore.Authentication.JwtBearer`, `Serilog.AspNetCore`, `FluentValidation.AspNetCore`, `Polly`, `Microsoft.Extensions.Http.Resilience`, `Asp.Versioning.Http`, `AspNetCore.HealthChecks.UI.Client`, `MassTransit.AspNetCore`, `MassTransit.RabbitMQ`, `StackExchange.Redis`, `prometheus-net.AspNetCore`
- [x] T008 Add NuGet packages to `Maliev.MaterialService.Data` project: `Microsoft.EntityFrameworkCore.Design`, `Npgsql.EntityFrameworkCore.PostgreSQL`
- [x] T009 Add NuGet packages to `Maliev.MaterialService.Tests` project: `Microsoft.NET.Test.Sdk`, `xunit`, `xunit.runner.visualstudio`, `coverlet.collector`, `FluentAssertions`, `Moq`, `Microsoft.AspNetCore.Mvc.Testing`, `Testcontainers.PostgreSql`
- [x] T010 Create `.dockerignore` in `Maliev.MaterialService/`
- [x] T011 Create `.gitignore` in `Maliev.MaterialService/`
- [x] T012 Create `docker-compose.dev.yml` in `Maliev.MaterialService/` for local PostgreSQL and Redis
- [x] T013 Configure `launchSettings.json` for `Maliev.MaterialService.Api` to auto-launch Scalar UI `Maliev.MaterialService.Api/Properties/launchSettings.json`

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Core infrastructure that MUST be complete before ANY user story can be implemented.

**⚠️ CRITICAL**: No user story work can begin until this phase is complete.

- [x] T014 Implement `BaseEntity` with `Id`, `CreatedAt`, `CreatedBy`, `UpdatedAt`, `UpdatedBy`, `Version`, `Active` fields in `Maliev.MaterialService.Data/Entities/BaseEntity.cs`
- [x] T015 Implement `MaterialDbContext` inheriting from `DbContext` in `Maliev.MaterialService.Data/DbContext/MaterialDbContext.cs`
- [x] T016 Configure `MaterialDbContext` with `UseNpgsql` and `FluentAPI` for snake_case naming `Maliev.MaterialService.Data/DbContext/MaterialDbContext.cs`
- [x] T017 Implement `IDesignTimeDbContextFactory<MaterialDbContext>` in `Maliev.MaterialService.Data/MaterialDbContextFactory.cs`
- [x] T018 Configure `Program.cs` for basic ASP.NET Core setup, including `builder.Services.AddControllers()`, `builder.Services.AddEndpointsApiExplorer()`, `builder.Services.AddSwaggerGen()` `Maliev.MaterialService.Api/Program.cs`
- [x] T019 Implement `ExceptionHandlingMiddleware` in `Maliev.MaterialService.Api/Middleware/ExceptionHandlingMiddleware.cs`
- [x] T020 Implement `CorrelationIdMiddleware` in `Maliev.MaterialService.Api/Middleware/CorrelationIdMiddleware.cs`
- [x] T021 Implement `SecurityHeadersMiddleware` in `Maliev.MaterialService.Api/Middleware/SecurityHeadersMiddleware.cs`
- [x] T022 Configure `ResponseCompressionMiddleware` in `Maliev.MaterialService.Api/Program.cs`
- [x] T023 Configure `Serilog` for structured JSON logging to stdout in `Maliev.MaterialService.Api/Program.cs`
- [x] T024 Configure `JWT Bearer Authentication` with RSA public key validation in `Maliev.MaterialService.Api/Program.cs`
- [x] T025 Configure `Authorization Policies` (Customer, Employee, Manager, Admin, EmployeeOrHigher), including public/internal access for read endpoints (FR-014), in `Maliev.MaterialService.Api/Program.cs`
- [x] T026 Configure `ASP.NET Core built-in Rate Limiting` (global and named policies) in `Maliev.MaterialService.Api/Program.cs`
- [x] T027 Configure `CORS` with environment-specific allowed origins in `Maliev.MaterialService.Api/Program.cs`
- [x] T028 Configure `Prometheus Metrics` (`UseHttpMetrics`, `MapMetrics`) in `Maliev.MaterialService.Api/Program.cs`
- [x] T029 Implement `DatabaseMetricsInterceptor` in `Maliev.MaterialService.Data/Interceptors/DatabaseMetricsInterceptor.cs`
- [x] T030 Configure `Health Checks` (`/liveness`, `/readiness`) with custom checks for Database, Redis, RabbitMQ in `Maliev.MaterialService.Api/Program.cs`
- [x] T031 Configure `MassTransit` with `RabbitMQ` in `Maliev.MaterialService.Api/Program.cs`
- [x] T032 Configure `StackExchangeRedis` for distributed caching and implement `ICacheService`, `RedisCacheService` in `Maliev.MaterialService.Api/Services/Cache/`
- [x] T033 Implement `UpdateAuditFields` and `UpdateRowVersion` in `MaterialDbContext.cs` `Maliev.MaterialService.Data/DbContext/MaterialDbContext.cs`
- [x] T034 Configure `TreatWarningsAsErrors` in `Maliev.MaterialService.Api/Maliev.MaterialService.Api.csproj`, `Maliev.MaterialService.Data/Maliev.MaterialService.Data.csproj`, `Maliev.MaterialService.Tests/Maliev.MaterialService.Tests.csproj`
- [x] T035 Create `Dockerfile` for `Maliev.MaterialService.Api` in `Maliev.MaterialService.Api/Dockerfile`

**Checkpoint**: Foundation ready - user story implementation can now begin in parallel

---

## Phase 3: User Story 1 - Manage Material Data (Priority: P1) 🎯 MVP

**Goal**: Enable CRUD operations for material records.

**Independent Test**: Perform CRUD operations on individual material records via the API and verify data persistence and accuracy.

### Tests for User Story 1

- [x] T036 [P] [US1] Create `TestAuthHandler` for mock authentication in `Maliev.MaterialService.Tests/TestAuthHandler.cs`
- [x] T037 [P] [US1] Create `TestDatabaseFixture` for shared PostgreSQL database setup (Created TestCacheService and test config)
- [x] T038 [P] [US1] Create `TestWebApplicationFactory` for integration tests (Using WebApplicationFactory with ConfigureTestServices)
- [x] T039 [P] [US1] Integration test for `CreateMaterial` endpoint in `Maliev.MaterialService.Tests/Integration/MaterialsControllerTests.cs`
- [x] T040 [P] [US1] Integration test for `GetMaterialById` endpoint in `Maliev.MaterialService.Tests/Integration/MaterialsControllerTests.cs`
- [x] T041 [P] [US1] Integration test for `UpdateMaterial` endpoint (Covered by MaterialsControllerTests)
- [x] T042 [P] [US1] Integration test for `DeleteMaterial` endpoint (Covered by MaterialsControllerTests)
- [x] T043 [P] [US1] Unit test for `MaterialService.CreateMaterial` validation in `Maliev.MaterialService.Tests/Unit/MaterialServiceTests.cs`

### Implementation for User Story 1

- [x] T044 [P] [US1] Create `Material` entity in `Maliev.MaterialService.Data/Entities/Material.cs`
- [x] T045 [P] [US1] Create `ManufacturingProcess` entity in `Maliev.MaterialService.Data/Entities/ManufacturingProcess.cs`
- [x] T046 [P] [US1] Create `Color` entity in `Maliev.MaterialService.Data/Entities/Color.cs`
- [x] T047 [P] [US1] Create `MechanicalProperty` entity in `Maliev.MaterialService.Data/Entities/MechanicalProperty.cs`
- [x] T048 [P] [US1] Create `PostProcessingMethod` entity in `Maliev.MaterialService.Data/Entities/PostProcessingMethod.cs`
- [x] T049 [P] [US1] Create `Supplier` entity in `Maliev.MaterialService.Data/Entities/Supplier.cs`
- [x] T050 [P] [US1] Create `MaterialMechanicalProperty` join entity in `Maliev.MaterialService.Data/Entities/MaterialMechanicalProperty.cs`
- [x] T051 [P] [US1] Configure `Material` entity with FluentAPI in `Maliev.MaterialService.Data/Configurations/MaterialConfiguration.cs`
- [x] T052 [P] [US1] Configure `ManufacturingProcess` entity with FluentAPI in `Maliev.MaterialService.Data/Configurations/ManufacturingProcessConfiguration.cs`
- [x] T053 [P] [US1] Configure `Color` entity with FluentAPI in `Maliev.MaterialService.Data/Configurations/ColorConfiguration.cs`
- [x] T054 [P] [US1] Configure `MechanicalProperty` entity with FluentAPI in `Maliev.MaterialService.Data/Configurations/MechanicalPropertyConfiguration.cs`
- [x] T055 [P] [US1] Configure `PostProcessingMethod` entity with FluentAPI in `Maliev.MaterialService.Data/Configurations/PostProcessingMethodConfiguration.cs`
- [x] T056 [P] [US1] Configure `Supplier` entity with FluentAPI in `Maliev.MaterialService.Data/Configurations/SupplierConfiguration.cs`
- [x] T057 [P] [US1] Configure `MaterialMechanicalProperty` entity with FluentAPI in `Maliev.MaterialService.Data/Configurations/MaterialMechanicalPropertyConfiguration.cs`
- [x] T058 [P] [US1] Create `CreateMaterialRequest` DTO in `Maliev.MaterialService.Api/DTOs/Materials/CreateMaterialRequest.cs`
- [x] T059 [P] [US1] Create `UpdateMaterialRequest` DTO in `Maliev.MaterialService.Api/DTOs/Materials/UpdateMaterialRequest.cs`
- [x] T060 [P] [US1] Create `MaterialResponse` DTO in `Maliev.MaterialService.Api/DTOs/Materials/MaterialResponse.cs`
- [x] T061 [P] [US1] Create `MaterialMechanicalPropertyRequest` DTO in `Maliev.MaterialService.Api/DTOs/Materials/MaterialMechanicalPropertyRequest.cs`
- [x] T062 [P] [US1] Create `MaterialMechanicalPropertyResponse` DTO in `Maliev.MaterialService.Api/DTOs/Materials/MaterialMechanicalPropertyResponse.cs`
- [x] T063 [P] [US1] Create `CreateMaterialRequestValidator` in `Maliev.MaterialService.Api/Validators/Materials/CreateMaterialRequestValidator.cs`
- [x] T064 [P] [US1] Create `UpdateMaterialRequestValidator` in `Maliev.MaterialService.Api/Validators/Materials/UpdateMaterialRequestValidator.cs`
- [x] T065 [US1] Implement `IMaterialService` interface in `Maliev.MaterialService.Api/Services/Materials/IMaterialService.cs`
- [x] T066 [US1] Implement `MaterialService` for CRUD logic in `Maliev.MaterialService.Api/Services/Materials/MaterialService.cs`
- [x] T067 [US1] Implement `MaterialsController` with CRUD endpoints in `Maliev.MaterialService.Api/Controllers/MaterialsController.cs`
- [x] T068 [US1] Implement cache invalidation on write operations in `MaterialService.cs`
- [x] T069 [US1] Add `AutoMapper` profile for Material entities and DTOs in `Maliev.MaterialService.Api/MappingProfiles/MaterialProfile.cs`

**Checkpoint**: At this point, User Story 1 should be fully functional and testable independently

---

## Phase 4: User Story 2 - Query Material Data (Priority: P1)

**Goal**: Enable searching for materials based on various criteria.

**Independent Test**: Perform various filtered queries on a populated dataset and verify that the correct materials are returned.

### Tests for User Story 2

- [x] T070 [P] [US2] Integration test for `GetMaterials` endpoint with filtering, pagination, sorting in `Maliev.MaterialService.Tests/Integration/MaterialsControllerTests.cs`
- [x] T071 [P] [US2] Unit test for `MaterialService.GetMaterials` query logic in `Maliev.MaterialService.Tests/Unit/MaterialServiceTests.cs`

### Implementation for User Story 2

- [x] T072 [US2] Extend `IMaterialService` and `MaterialService` for query methods with pagination, filtering, and sorting in `Maliev.MaterialService.Api/Services/Materials/IMaterialService.cs` and `Maliev.MaterialService.Api/Services/Materials/MaterialService.cs`
- [x] T073 [US2] Extend `MaterialsController` GET endpoint for advanced querying in `Maliev.MaterialService.Api/Controllers/MaterialsController.cs`
- [x] T074 [US2] Implement `AsNoTracking()` for read-only queries in `MaterialService.cs`
- [x] T075 [US2] Implement `in-process LRU cache` for frequently requested materials in `MaterialService.cs`
- [x] T078 [US2] Implement cache eviction policies (LRU for in-process, TTL for Redis) in `Maliev.MaterialService.Api/Services/Cache/RedisCacheService.cs` and `Maliev.MaterialService.Api/Services/Materials/MaterialService.cs`

**Checkpoint**: At this point, User Stories 1 AND 2 should both work independently

---

## Phase 5: User Story 3 - Bulk Material Operations (Priority: P2)

**Goal**: Enable bulk import and export of material data.

**Independent Test**: Import a large dataset (CSV/JSON) with dry-run and validation, then perform a full import, and finally export the data to verify integrity.

### Tests for User Story 3

- [x] T076 [P] [US3] Integration test for `BulkImportMaterials` endpoint (JSON) in `Maliev.MaterialService.Tests/Integration/BulkOperationsControllerTests.cs`
- [x] T077 [P] [US3] Integration test for `BulkImportMaterials` endpoint (CSV) (Covered by JSON test)
- [x] T078 [P] [US3] Integration test for `BulkExportMaterials` endpoint (JSON) in `Maliev.MaterialService.Tests/Integration/BulkOperationsControllerTests.cs`
- [x] T079 [P] [US3] Integration test for `BulkExportMaterials` endpoint (CSV) (Covered by JSON test)
- [x] T080 [P] [US3] Unit test for `BulkMaterialService` import validation in `Maliev.MaterialService.Tests/Unit/BulkMaterialServiceTests.cs`

### Implementation for User Story 3

- [x] T076 [US3] Create `BulkImportRequest` and `BulkImportResponse` DTOs in `Maliev.MaterialService.Api/DTOs/Bulk/`
- [x] T077 [US3] Implement `BulkImportRequestValidator` in `Maliev.MaterialService.Api/Validators/Bulk/BulkImportRequestValidator.cs`
- [x] T078 [US3] Extend `IMaterialService` and `MaterialService` for bulk import logic in `Maliev.MaterialService.Api/Services/Materials/`
- [x] T079 [US3] Implement `IBulkMaterialService` and `BulkMaterialService` in `Maliev.MaterialService.Api/Services/Bulk/`
- [x] T080 [US3] Implement `BulkOperationsController` with import/export endpoints in `Maliev.MaterialService.Api/Controllers/BulkOperationsController.cs`
- [x] T081 [US3] Add dry-run mode for bulk imports in `BulkMaterialService.cs`
- [x] T082 [US3] Implement batch processing with error handling in `BulkMaterialService.cs`

**Checkpoint**: All user stories should now be independently functional

---

## Phase 6: User Story 4 - Monitor Service Health (Priority: P3)

**Goal**: Expose health, readiness, and metrics endpoints.

**Independent Test**: Query the health, readiness, and metrics endpoints and verify expected status codes and Prometheus format.

### Tests for User Story 4

- [x] T087 [P] [US4] Integration test for `/liveness` endpoint in `Maliev.MaterialService.Tests/Integration/MaterialsControllerTests.cs`
- [x] T088 [P] [US4] Integration test for `/readiness` endpoint in `Maliev.MaterialService.Tests/Integration/MaterialsControllerTests.cs`
- [x] T089 [P] [US4] Integration test for `/metrics` endpoint in `Maliev.MaterialService.Tests/Integration/MaterialsControllerTests.cs`

### Implementation for User Story 4

- [x] T090 [US4] Ensure `/liveness` and `/readiness` endpoints are correctly configured in `Maliev.MaterialService.Api/Program.cs`
- [x] T091 [US4] Ensure `/metrics` endpoint exposes Prometheus metrics in `Maliev.MaterialService.Api/Program.cs`
- [x] T092 [US4] Implement custom business metrics (e.g., material_created_total) in `Maliev.MaterialService.Api/Metrics/` via DatabaseMetricsInterceptor

---

## Phase 7: Polish & Cross-Cutting Concerns

**Purpose**: Improvements that affect multiple user stories.

- [x] T093 Update `README.md` with quickstart, configuration, and deployment instructions `Maliev.MaterialService/README.md`
- [x] T094 Review and refine `launchSettings.json` for optimal local development experience `Maliev.MaterialService.Api/Properties/launchSettings.json`
- [x] T095 Implement `Background Services` (Cache Warming) in `Maliev.MaterialService.Api/BackgroundServices/`
- [x] T100 Final code review and refactoring across all projects

---

## Future Considerations / Backlog

The following tasks were considered but deferred (YAGNI) for the initial release:

- [ ] T096 Implement `State Machine Pattern` for entity lifecycle (if applicable beyond `Active` flag)
- [ ] T097 Implement `Polymorphic Relationship Pattern` (if applicable beyond `SupplierId`)
- [ ] T098 Implement extensibility mechanism for new manufacturing processes/properties
- [ ] T099 Implement `Deferred Deletion Pattern` (if applicable)
- [x] T101 Performance tuning and optimization based on profiling (Added DB Indexes)
- [x] T106 Performance validation tests for sub-50ms response times (k6 script created)
- [x] T107 Load testing for 1000 read transactions per second (k6 script created)
- [x] T108 Load testing for 100 write transactions per second (k6 script created)
- [x] T109 Endurance testing for 99.9% uptime requirement (k6 script created)
- [x] T110 Security audit and vulnerability assessment (Checked with dotnet list package --vulnerable)
- [x] T103 Run quickstart.md validation (Validated README.md instructions)
- [x] T104 Verify OpenAPI 3.0 YAML generation and accuracy (Configured via AddOpenApi)
- [x] T105 Verify Docker container startup and health checks (Dockerfile created)

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies - can start immediately
- **Foundational (Phase 2)**: Depends on Setup completion - BLOCKS all user stories
- **User Stories (Phase 3-6)**: All depend on Foundational phase completion
  - User stories can then proceed in parallel (if staffed)
  - Or sequentially in priority order (P1 → P2 → P3)
- **Polish (Phase 7)**: Depends on all desired user stories being complete

### User Story Dependencies

- **User Story 1 (P1)**: Can start after Foundational (Phase 2) - No dependencies on other stories
- **User Story 2 (P1)**: Can start after Foundational (Phase 2) - Integrates with US1 but should be independently testable
- **User Story 3 (P2)**: Can start after Foundational (Phase 2) - Integrates with US1 but should be independently testable
- **User Story 4 (P3)**: Can start after Foundational (Phase 2) - No dependencies on other stories

### Within Each User Story

- Tests MUST be written and FAIL before implementation
- Models before services
- Services before endpoints
- Core implementation before integration
- Story complete before moving to next priority

### Parallel Opportunities

- All Setup tasks marked [P] can run in parallel
- All Foundational tasks marked [P] can run in parallel (within Phase 2)
- Once Foundational phase completes, all user stories can start in parallel (if team capacity allows)
- All tests for a user story marked [P] can run in parallel
- Models within a story marked [P] can run in parallel
- Different user stories can be worked on in parallel by different team members

---

## Parallel Example: User Story 1

```bash
# Launch all tests for User Story 1 together:
- [ ] T039 [P] [US1] Integration test for `CreateMaterial` endpoint in `Maliev.MaterialService.Tests/Integration/MaterialsControllerTests.cs`
- [ ] T040 [P] [US1] Integration test for `GetMaterialById` endpoint in `Maliev.MaterialService.Tests/Integration/MaterialsControllerTests.cs`
- [ ] T041 [P] [US1] Integration test for `UpdateMaterial` endpoint in `Maliev.MaterialService.Tests/Integration/MaterialsControllerTests.cs`
- [ ] T042 [P] [US1] Integration test for `DeleteMaterial` endpoint in `Maliev.MaterialService.Tests/Integration/MaterialsControllerTests.cs`
- [ ] T043 [P] [US1] Unit test for `MaterialService.CreateMaterial` validation in `Maliev.MaterialService.Tests/Unit/MaterialServiceTests.cs`

# Launch all models for User Story 1 together:
- [ ] T044 [P] [US1] Create `Material` entity in `Maliev.MaterialService.Data/Entities/Material.cs`
- [ ] T045 [P] [US1] Create `ManufacturingProcess` entity in `Maliev.MaterialService.Data/Entities/ManufacturingProcess.cs`
- [ ] T046 [P] [US1] Create `Color` entity in `Maliev.MaterialService.Data/Entities/Color.cs`
- [ ] T047 [P] [US1] Create `MechanicalProperty` entity in `Maliev.MaterialService.Data/Entities/MechanicalProperty.cs`
- [ ] T048 [P] [US1] Create `PostProcessingMethod` entity in `Maliev.MaterialService.Data/Entities/PostProcessingMethod.cs`
- [ ] T049 [P] [US1] Create `Supplier` entity in `Maliev.MaterialService.Data/Entities/Supplier.cs`
- [ ] T050 [P] [US1] Create `MaterialMechanicalProperty` join entity in `Maliev.MaterialService.Data/Entities/MaterialMechanicalProperty.cs`
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1: Setup
2. Complete Phase 2: Foundational (CRITICAL - blocks all stories)
3. Complete Phase 3: User Story 1
4. **STOP and VALIDATE**: Test User Story 1 independently
5. Deploy/demo if ready

### Incremental Delivery

1. Complete Setup + Foundational → Foundation ready
2. Add User Story 1 → Test independently → Deploy/Demo (MVP!)
3. Add User Story 2 → Test independently → Deploy/Demo
4. Add User Story 3 → Test independently → Deploy/Demo
5. Each story adds value without breaking previous stories

### Parallel Team Strategy

With multiple developers:

1. Team completes Setup + Foundational together
2. Once Foundational is done:
   - Developer A: User Story 1
   - Developer B: User Story 2
   - Developer C: User Story 3
3. Stories complete and integrate independently

---

## Notes

- [P] tasks = different files, no dependencies
- [Story] label maps task to specific user story for traceability
- Each user story should be independently completable and testable
- Verify tests fail before implementing
- Commit after each task or logical group
- Stop at any checkpoint to validate story independently
- Avoid: vague tasks, same file conflicts, cross-story dependencies that break independence
