# Data Model: Material Pricing Fields

**Feature**: 001-material-pricing-fields
**Date**: 2026-02-22

## Entity Changes

### Material Entity (Modified)

**File**: `Maliev.MaterialService.Data/Entities/Material.cs`

| Field | Type | Nullable | Constraints | Description |
|-------|------|----------|-------------|-------------|
| Density | `decimal` | No | Range: 0-25 g/cm³ | Material density for weight calculations |
| CostPerKg | `decimal` | No | Range: 0-999,999 THB | Cost per kilogram for pricing |
| ProcessParameters | `Dictionary<string, string>` | No | Default: empty `{}` | Technology-specific parameters |

**Existing Fields (Unchanged)**:
- `Id` (Guid, PK)
- `Name` (string, required, max 200)
- `Code` (string, required, max 100, unique)
- `Description` (string?, max 1000)
- `PricePerUnit` (decimal, precision 18,2)
- `StockLevel` (int, required)
- `SupplierId` (Guid?)
- `Active` (bool)
- `CreatedAt`, `CreatedBy`, `UpdatedAt`, `UpdatedBy` (audit fields)
- `Version` (byte[], concurrency)

## Entity Configuration

### MaterialConfiguration (Modified)

**File**: `Maliev.MaterialService.Data/Configurations/MaterialConfiguration.cs`

```csharp
// New configurations to add:

builder.Property(m => m.Density)
    .HasPrecision(18, 4)
    .HasDefaultValue(0m)
    .IsRequired();

builder.Property(m => m.CostPerKg)
    .HasPrecision(18, 2)
    .HasDefaultValue(0m)
    .IsRequired();

builder.Property(m => m.ProcessParameters)
    .HasColumnType("jsonb")
    .HasDefaultValueSql("'{}'::jsonb")
    .IsRequired();
```

## Database Migration

**Migration Name**: `AddMaterialPricingFields`

**Changes**:
1. Add column `Density` (decimal(18,4), NOT NULL, DEFAULT 0)
2. Add column `CostPerKg` (decimal(18,2), NOT NULL, DEFAULT 0)
3. Add column `ProcessParameters` (jsonb, NOT NULL, DEFAULT '{}'::jsonb)

**SQL Preview**:
```sql
ALTER TABLE "Materials" ADD COLUMN "Density" numeric(18,4) NOT NULL DEFAULT 0;
ALTER TABLE "Materials" ADD COLUMN "CostPerKg" numeric(18,2) NOT NULL DEFAULT 0;
ALTER TABLE "Materials" ADD COLUMN "ProcessParameters" jsonb NOT NULL DEFAULT '{}';
```

## DTO Changes

### MaterialResponse (Modified)

**File**: `Maliev.MaterialService.Api/DTOs/Materials/MaterialResponse.cs`

```csharp
/// <summary>
/// Material density in g/cm³
/// </summary>
public decimal Density { get; set; }

/// <summary>
/// Cost per kilogram in THB
/// </summary>
public decimal CostPerKg { get; set; }

/// <summary>
/// Technology-specific process parameters
/// </summary>
public Dictionary<string, string> ProcessParameters { get; set; } = new();
```

### CreateMaterialRequest (Modified)

**File**: `Maliev.MaterialService.Api/DTOs/Materials/CreateMaterialRequest.cs`

```csharp
/// <summary>
/// Material density in g/cm³ (0-25 range)
/// </summary>
[Range(0, 25)]
public decimal Density { get; set; }

/// <summary>
/// Cost per kilogram in THB (0-999999 range)
/// </summary>
[Range(0, 999999)]
public decimal CostPerKg { get; set; }

/// <summary>
/// Technology-specific process parameters
/// </summary>
public Dictionary<string, string> ProcessParameters { get; set; } = new();
```

### UpdateMaterialRequest (Modified)

**File**: `Maliev.MaterialService.Api/DTOs/Materials/UpdateMaterialRequest.cs`

```csharp
/// <summary>
/// Material density in g/cm³ (0-25 range)
/// </summary>
[Range(0, 25)]
public decimal Density { get; set; }

/// <summary>
/// Cost per kilogram in THB (0-999999 range)
/// </summary>
[Range(0, 999999)]
public decimal CostPerKg { get; set; }

/// <summary>
/// Technology-specific process parameters
/// </summary>
public Dictionary<string, string> ProcessParameters { get; set; } = new();
```

## Validation Rules

| Field | Validation | Error Message |
|-------|------------|---------------|
| Density | Range(0, 25) | "Density must be between 0 and 25 g/cm³" |
| CostPerKg | Range(0, 999999) | "CostPerKg must be between 0 and 999,999 THB" |
| ProcessParameters | None (keys/values stored as-is) | N/A |

## State Transitions

No state transitions - simple CRUD operations.

## Indexes

No new indexes required for this feature. Existing indexes on `PricePerUnit` may be used for cost-related queries.
