# Data Model: Material WebAPI Service

**Branch**: `001-material-webapi` | **Date**: 2024-11-18 | **Plan**: [link to plan.md]
**Source**: Feature specification from `/specs/001-material-webapi/spec.md`

## Entities

### Material

Represents a single material used in manufacturing.

**Fields**:

*   `Id`: Unique identifier (e.g., GUID). Primary Key.
*   `Name`: String, required, unique.
*   `Code`: String, required, unique identifier for the material (e.g., SKU, internal code).
*   `Description`: String, optional.
*   `ManufacturingProcesses`: Collection of `ManufacturingProcess` (many-to-many relationship).
*   `AvailableColors`: Collection of `Color` (many-to-many relationship).
*   `MechanicalProperties`: Collection of `MaterialMechanicalProperty` (one-to-many relationship to a join entity).
*   `PostProcessingMethods`: Collection of `PostProcessingMethod` (many-to-many relationship).
*   `PricePerUnit`: Decimal, required, positive value.
*   `StockLevel`: Integer, required, non-negative value.
*   `SupplierId`: GUID, optional, Foreign Key to `Supplier`.
*   `CreatedBy`: String, required, user who created the record.
*   `CreatedAt`: DateTime (UTC), required, timestamp of creation.
*   `UpdatedBy`: String, required, user who last updated the record.
*   `UpdatedAt`: DateTime (UTC), required, timestamp of last update.
*   `Version`: Byte array (`byte[]`), required, for optimistic concurrency control (RowVersion).
*   `Active`: Boolean, required, indicates if the material is active (soft delete).

**Relationships**:

*   Many-to-many with `ManufacturingProcess`
*   Many-to-many with `Color`
*   One-to-many with `MaterialMechanicalProperty`
*   Many-to-many with `PostProcessingMethod`
*   Many-to-one with `Supplier` (optional)

**Validation Rules**:

*   `Id`: Must be a valid GUID.
*   `Name`: Must be unique, not empty.
*   `Code`: Must be unique, not empty.
*   `PricePerUnit`: Must be greater than 0.
*   `StockLevel`: Must be greater than or equal to 0.
*   `ManufacturingProcesses`: Must contain valid, supported processes.
*   `MechanicalProperties`: Values must be within valid ranges.
*   `Version`: Must be a valid Base64 string on update requests.

### ManufacturingProcess

A lookup entity representing a specific manufacturing technique.

**Fields**:

*   `Id`: Unique identifier (e.g., GUID). Primary Key.
*   `Name`: String, required, unique (e.g., "3D Printing", "CNC Machining").

**Relationships**:

*   Many-to-many with `Material`

### Color

A lookup entity representing an available color for materials.

**Fields**:

*   `Id`: Unique identifier (e.g., GUID). Primary Key.
*   `Name`: String, required, unique (e.g., "Red", "Blue", "RAL 7016").
*   `HexCode`: String, optional, unique (e.g., "#FF0000").

**Relationships**:

*   Many-to-many with `Material`

### MechanicalProperty

A lookup entity representing a specific mechanical characteristic.

**Fields**:

*   `Id`: Unique identifier (e.g., GUID). Primary Key.
*   `Name`: String, required, unique (e.g., "Tensile Strength", "Hardness", "Elasticity").
*   `Unit`: String, required (e.g., "MPa", "Shore D", "GPa").

**Relationships**:

*   One-to-many with `MaterialMechanicalProperty`

### MaterialMechanicalProperty (Join Entity)

Represents the specific value of a `MechanicalProperty` for a `Material`.

**Fields**:

*   `MaterialId`: GUID, Foreign Key to `Material`.
*   `MechanicalPropertyId`: GUID, Foreign Key to `MechanicalProperty`.
*   `Value`: Decimal, required, the measured value of the property.

**Relationships**:

*   Many-to-one with `Material`
*   Many-to-one with `MechanicalProperty`

**Validation Rules**:

*   `Value`: Must be within a valid range defined for the specific `MechanicalProperty`.

### PostProcessingMethod

A lookup entity representing a method applied after manufacturing.

**Fields**:

*   `Id`: Unique identifier (e.g., GUID). Primary Key.
*   `Name`: String, required, unique (e.g., "Sanding", "Polishing", "Annealing").

**Relationships**:

*   Many-to-many with `Material`

### Supplier (Optional)

Represents a supplier of materials.

**Fields**:

*   `Id`: Unique identifier (e.g., GUID). Primary Key.
*   `Name`: String, required.
*   `ContactInfo`: String, optional.

**Relationships**:

*   One-to-many with `Material`

## State Transitions

### Material `Active` Flag

*   **Initial State**: `true` (when created)
*   **Transition**: `true` -> `false` (when deleted, representing a soft delete)
*   **Terminal State**: `false` (cannot transition back to `true` without explicit administrative action, which is outside the scope of standard CRUD).

**Validation**: Only active materials are returned by default in queries. Deletion sets `Active` to `false`.
