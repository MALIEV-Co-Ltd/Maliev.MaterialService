# Research: Material Pricing Fields

**Feature**: 001-material-pricing-fields
**Date**: 2026-02-22

## Research Questions

### Q1: JSONB Storage for ProcessParameters in PostgreSQL/EF Core

**Decision**: Use EF Core's `OwnsOne` or direct `Dictionary<string, string>` mapping with JSONB converter.

**Rationale**: 
- PostgreSQL JSONB provides efficient storage and querying capabilities
- EF Core 8+ supports `Dictionary<string, string>` with `ToJson()` for JSONB columns
- Allows flexible key-value pairs without schema changes for new manufacturing technologies

**Alternatives Considered**:
1. **Separate ProcessParameter table**: Rejected due to complexity and overhead for simple key-value storage
2. **JSON string column**: Rejected because loses querying capability and type safety
3. **Individual columns per parameter**: Rejected because requires schema migration for each new technology

**Implementation**:
```csharp
builder.Property(m => m.ProcessParameters)
    .HasConversion(
        v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
        v => JsonSerializer.Deserialize<Dictionary<string, string>>(v, (JsonSerializerOptions?)null) ?? new()
    )
    .HasColumnType("jsonb")
    .HasDefaultValueSql("'{}'::jsonb");
```

### Q2: Decimal Precision for Density and CostPerKg

**Decision**: Use `decimal(18,4)` for Density and `decimal(18,2)` for CostPerKg.

**Rationale**:
- Density values typically range from 0.01 (aerogels) to ~23 (osmium) g/cm³, 4 decimal places sufficient for precision
- CostPerKg in THB: 2 decimal places appropriate for currency
- `decimal` type avoids floating-point precision issues in financial calculations

**Alternatives Considered**:
1. **float/double**: Rejected due to floating-point precision issues with currency
2. **decimal(10,4)**: Rejected as insufficient for large CostPerKg values (up to 999,999 THB/kg per spec)

### Q3: Default Values for Existing Data Migration

**Decision**: Set `Density = 0` and `CostPerKg = 0` as default values via EF Core configuration and migration.

**Rationale**:
- Zero values represent "unconfigured" state per spec edge cases
- No data backfill required, migration completes quickly
- Pricing Service responsible for handling zero values gracefully

**Alternatives Considered**:
1. **NULL defaults**: Rejected because adds nullable handling complexity
2. **Meaningful defaults**: Rejected because no universal default makes sense for all materials

### Q4: Controller Rename Strategy

**Decision**: Rename class from `InventoryController` to `StockController` while preserving route attribute.

**Rationale**:
- Simple class rename with no API contract changes
- Route attribute `[Route("material/v{version:apiVersion}/inventory")]` remains unchanged
- Minimal risk, improves code clarity

**Alternatives Considered**:
1. **Keep current name**: Rejected per spec requirement to avoid confusion with InventoryService
2. **Change route too**: Rejected because would break existing API consumers

## Technology Stack Confirmed

| Component | Technology | Version |
|-----------|------------|---------|
| Runtime | .NET | 10.0 |
| ORM | Entity Framework Core | 10.x |
| Database | PostgreSQL | 16.x |
| JSON Handling | System.Text.Json | Built-in |
| Testing | xUnit + Testcontainers | Latest |

## No External Clarifications Required

All technical questions resolved from existing codebase patterns and constitutional guidelines.
