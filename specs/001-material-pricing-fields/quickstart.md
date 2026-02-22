# Quickstart: Material Pricing Fields

**Feature**: 001-material-pricing-fields
**Date**: 2026-02-22

## Implementation Steps

### 1. Entity Changes

**Edit**: `Maliev.MaterialService.Data/Entities/Material.cs`

Add three new properties:
```csharp
public decimal Density { get; set; }
public decimal CostPerKg { get; set; }
public Dictionary<string, string> ProcessParameters { get; set; } = new();
```

### 2. Entity Configuration

**Edit**: `Maliev.MaterialService.Data/Configurations/MaterialConfiguration.cs`

Add JSONB mapping and defaults:
```csharp
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

### 3. Database Migration

```bash
dotnet ef migrations add AddMaterialPricingFields \
  --project Maliev.MaterialService.Data \
  --startup-project Maliev.MaterialService.Api

dotnet ef database update \
  --project Maliev.MaterialService.Data \
  --startup-project Maliev.MaterialService.Api
```

### 4. DTO Changes

**Edit**: `MaterialResponse.cs`, `CreateMaterialRequest.cs`, `UpdateMaterialRequest.cs`

Add same three properties with XML documentation and Range validation attributes.

### 5. Mapper Updates

**Edit**: `Maliev.MaterialService.Api/Mapping/DomainToDtoMapper.cs`

Add field mappings in:
- `ToMaterialResponse()` - map entity → response
- `ToMaterial()` - map create request → entity  
- `UpdateMaterial()` - map update request → entity

### 6. Controller Rename

**Rename**: `InventoryController.cs` → `StockController.cs`

1. Change class name from `InventoryController` to `StockController`
2. Keep route attribute unchanged: `[Route("material/v{version:apiVersion}/inventory")]`
3. Update constructor parameter type: `ILogger<StockController>`

### 7. Build & Test

```bash
dotnet build
dotnet test
```

## Verification Checklist

- [ ] Build succeeds with zero warnings
- [ ] All tests pass
- [ ] GET /material/v1/materials/{id} returns new fields
- [ ] POST /material/v1/materials accepts new fields
- [ ] PUT /material/v1/materials/{id} updates new fields
- [ ] Validation rejects Density outside 0-25 range
- [ ] Validation rejects CostPerKg outside 0-999999 range
- [ ] StockController class exists with route "material/v1/inventory"
- [ ] Existing inventory endpoints still work

## Files Changed Summary

| File | Action |
|------|--------|
| `Material.cs` | Modify |
| `MaterialConfiguration.cs` | Modify |
| `MaterialResponse.cs` | Modify |
| `CreateMaterialRequest.cs` | Modify |
| `UpdateMaterialRequest.cs` | Modify |
| `DomainToDtoMapper.cs` | Modify |
| `InventoryController.cs` | Rename to `StockController.cs` |
| Migration (new) | Create |
