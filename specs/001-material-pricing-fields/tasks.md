# Tasks: Material Pricing Fields

**Input**: Design documents from `/specs/001-material-pricing-fields/`
**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/api-endpoints.md, quickstart.md

**Tests**: Per Constitution III (Test-First Development) and IV (Real Infrastructure Testing), formal xUnit/Testcontainers tests are authored BEFORE implementation.

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., US1, US2)
- Include exact file paths in descriptions

---

## Phase 1: Setup

**Purpose**: Project initialization (already complete - existing .NET 10.0 project)

- [x] T001 Verify project builds successfully with `dotnet build`
- [x] T002 Verify existing tests pass with `dotnet test`

---

## Phase 2: Foundational (Entity & Database)

**Purpose**: Core entity and database changes that MUST be complete before ANY user story can be implemented

**⚠️ CRITICAL**: No user story work can begin until this phase is complete

- [x] T003 Add Density, CostPerKg, and ProcessParameters properties to Material entity in Maliev.MaterialService.Data/Entities/Material.cs
- [x] T004 Add EF Core configuration for Density, CostPerKg, and ProcessParameters in Maliev.MaterialService.Data/Configurations/MaterialConfiguration.cs
- [x] T005 Generate database migration with `dotnet ef migrations add AddMaterialPricingFields --project Maliev.MaterialService.Data --startup-project Maliev.MaterialService.Api`
- [x] T006 Verify migration adds three columns (Density, CostPerKg, ProcessParameters) with correct defaults

**Checkpoint**: Database schema updated - user story implementation can now begin

---

## Phase 3: User Story 1 - Configure Material Pricing Data (Priority: P1) 🎯 MVP

**Goal**: Enable pricing administrators to configure density and cost-per-kilogram values for materials so Pricing Service can calculate weight-based quotes.

**Independent Test**: Create or update a material with Density (e.g., 1.25 g/cm³) and CostPerKg (e.g., 500 THB/kg), then verify those values are persisted and returned correctly through the API.

### Tests for User Story 1 (Testcontainers - Constitution III/IV)

- [x] T007 [P] [US1] Create integration test class MaterialPricingIntegrationTests.cs in Maliev.MaterialService.Tests/Integration/
- [x] T008 [P] [US1] Add test: CreateMaterial_WithDensityAndCostPerKg_ReturnsCreatedWithValues
- [x] T009 [P] [US1] Add test: UpdateMaterial_WithNewDensity_PersistsAndReturnsUpdatedValue
- [x] T010 [P] [US1] Add test: CreateMaterial_WithDensityOutOfRange_ReturnsBadRequest
- [x] T011 [P] [US1] Add test: CreateMaterial_WithCostPerKgOutOfRange_ReturnsBadRequest

### Implementation for User Story 1

- [x] T012 [P] [US1] Add Density property with XML docs to MaterialResponse
- [x] T013 [P] [US1] Add CostPerKg property with XML docs to MaterialResponse
- [x] T014 [P] [US1] Add Density property with Range(0, 25) validation to CreateMaterialRequest
- [x] T015 [P] [US1] Add CostPerKg property with Range(0, 999999) validation to CreateMaterialRequest
- [x] T016 [P] [US1] Add Density property with Range(0, 25) validation to UpdateMaterialRequest
- [x] T017 [P] [US1] Add CostPerKg property with Range(0, 999999) validation to UpdateMaterialRequest
- [x] T018 [US1] Add Density and CostPerKg mapping in DomainToDtoMapper.cs
- [x] T019 [US1] Build and verify tests pass

**Checkpoint**: User Story 1 complete

---

## Phase 4: User Story 2 - Configure Technology-Specific Process Parameters (Priority: P2)

**Goal**: Enable manufacturing engineers to store technology-specific process parameters (FDM, SLA, CNC) with each material for accurate processing cost calculations.

**Independent Test**: Create materials with different process parameters for FDM, SLA, and CNC technologies.

### Tests for User Story 2 (Testcontainers - Constitution III/IV)

- [x] T020 [P] [US2] Create integration test class MaterialProcessParametersIntegrationTests.cs
- [x] T021 [P] [US2] Add test: CreateMaterial_WithFdmProcessParameters_StoresAndReturnsCorrectly
- [x] T022 [P] [US2] Add test: CreateMaterial_WithSlaProcessParameters_StoresAndReturnsCorrectly
- [x] T023 [P] [US2] Add test: GetMaterial_WithoutProcessParameters_ReturnsEmptyDictionary

### Implementation for User Story 2

- [x] T024 [US2] Add ProcessParameters property to MaterialResponse, CreateMaterialRequest, UpdateMaterialRequest
- [x] T025 [US2] Add ProcessParameters mapping in DomainToDtoMapper.cs
- [x] T026 [US2] Build and verify tests pass

**Checkpoint**: User Story 2 complete

---

## Phase 5: Polish & Cleanup

**Purpose**: Remove duplicated inventory functionality (now handled by Maliev.InventoryService)

- [x] T027 Remove StockController.cs from Maliev.MaterialService.Api/Controllers/
- [x] T028 Remove StockControllerIntegrationTests.cs from Maliev.MaterialService.Tests/Integration/
- [x] T029 Remove inventory permissions from MaterialPermissions.cs
- [x] T030 Remove inventory permissions from MaterialPredefinedRoles.cs
- [x] T031 Remove inventory-related tests from AuthorizationTests.cs
- [x] T032 Build and verify all tests pass
