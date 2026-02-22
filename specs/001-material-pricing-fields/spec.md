# Feature Specification: Material Pricing Fields

**Feature Branch**: `001-material-pricing-fields`  
**Created**: 2026-02-21  
**Status**: Draft  
**Input**: User description: "Add Density, CostPerKg, and ProcessParameters fields to Material entity for PricingService integration"

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Configure Material Pricing Data (Priority: P1)

As a pricing administrator, I need to configure density and cost-per-kilogram values for each material so that the Pricing Service can calculate accurate, weight-based quotes for customer orders.

**Why this priority**: This is the core functionality required for the Pricing Service to function. Without these fields, the Pricing Service cannot calculate material costs at all.

**Independent Test**: Can be fully tested by creating or updating a material with Density and CostPerKg values, then verifying those values are persisted and returned correctly through the API. Delivers immediate value by enabling cost calculations.

**Acceptance Scenarios**:

1. **Given** a new material being created, **When** I provide Density (1.25 g/cm³) and CostPerKg (500 THB/kg), **Then** the material is saved with those values and they are returned in the response
2. **Given** an existing material, **When** I update its Density from 1.0 to 1.5 g/cm³, **Then** the updated value is persisted and reflected in subsequent API responses
3. **Given** a material with CostPerKg of 1000 THB/kg, **When** the Pricing Service retrieves the material, **Then** the CostPerKg value is available for quote calculations

---

### User Story 2 - Configure Technology-Specific Process Parameters (Priority: P2)

As a manufacturing engineer, I need to store technology-specific process parameters (like flow rates for FDM, exposure times for SLA, or machinability ratings for CNC) with each material so that the Pricing Service can calculate accurate processing costs.

**Why this priority**: This enables accurate pricing for different manufacturing technologies. Less critical than basic cost data but essential for precise quotes.

**Independent Test**: Can be fully tested by creating materials with different process parameters for FDM, SLA, and CNC technologies, then verifying the parameters are stored and retrieved correctly.

**Acceptance Scenarios**:

1. **Given** an FDM material (PLA), **When** I set ProcessParameters to {"FdmVolumetricFlowRate": "15", "FdmMinLayerTime": "5"}, **Then** the parameters are stored and returned correctly
2. **Given** an SLA material (Standard Resin), **When** I set ProcessParameters to {"SlaLayerExposure": "3", "SlaLiftTime": "4"}, **Then** the parameters are stored and returned correctly
3. **Given** a CNC material (Aluminum 6061), **When** I set ProcessParameters to {"CncMachinabilityRating": "0.9"}, **Then** the parameter is stored and returned correctly
4. **Given** a material without process parameters, **When** I retrieve it, **Then** an empty dictionary is returned (not null)

---

### Edge Cases

- What happens when Density is set to 0? The value is accepted (default for existing rows) but Pricing Service should handle this gracefully in calculations.
- What happens when CostPerKg is set to 0? The value is accepted (default for existing rows) representing free materials or unconfigured pricing.
- What happens when ProcessParameters contains invalid keys? Keys are stored as-is; validation is the responsibility of the Pricing Service.
- What happens when ProcessParameters contains values that cannot be parsed as numbers? Values are stored as strings; parsing is the responsibility of the Pricing Service.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: System MUST store Density value (in g/cm³) for each material
- **FR-002**: System MUST store CostPerKg value (in THB per kilogram) for each material  
- **FR-003**: System MUST store ProcessParameters as a collection of key-value pairs for each material
- **FR-004**: System MUST persist ProcessParameters in a structured format that supports querying
- **FR-005**: System MUST allow Density values between 0 and 25 g/cm³ (reasonable physical range)
- **FR-006**: System MUST allow CostPerKg values between 0 and 999,999 THB/kg
- **FR-007**: System MUST return Density, CostPerKg, and ProcessParameters in material API responses
- **FR-008**: System MUST accept Density, CostPerKg, and ProcessParameters in material create/update requests
- **FR-009**: System MUST preserve existing fields (PricePerUnit, StockLevel) without modification
- **FR-010**: System MUST provide sensible defaults (0) for Density and CostPerKg on existing rows after migration
- **FR-011**: System MUST provide empty dictionary default for ProcessParameters

### Key Entities

- **Material**: Represents a manufacturing material with properties for identification (Name, Code), pricing (PricePerUnit, CostPerKg), physical properties (Density), manufacturing parameters (ProcessParameters), and supplier relationships. Note: Inventory management is handled by the separate Maliev.InventoryService microservice.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: New materials can be created with Density and CostPerKg values in a single API call
- **SC-002**: Existing materials can be updated with process parameters without data loss
- **SC-003**: All deliverable files are updated and the solution builds without errors
- **SC-004**: Database migration applies successfully and existing data remains intact
- **SC-005**: API responses include all three new fields (Density, CostPerKg, ProcessParameters)
- **SC-006**: Validation rejects Density values outside 0-25 range
- **SC-007**: Validation rejects CostPerKg values outside 0-999999 range

## Assumptions

- The Pricing Service will handle validation of ProcessParameters keys and values
- Density is measured in g/cm³ (grams per cubic centimeter)
- CostPerKg is measured in THB (Thai Baht) per kilogram
- Existing materials with Density=0 or CostPerKg=0 represent unconfigured pricing data
- The migration will not require data backfill for existing records
- Inventory operations are handled by Maliev.InventoryService, not MaterialService
