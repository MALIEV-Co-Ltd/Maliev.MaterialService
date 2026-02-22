# API Contracts: Material Pricing Fields

**Feature**: 001-material-pricing-fields
**Date**: 2026-02-22
**API Version**: v1

## Modified Endpoints

### GET /material/v1/materials/{id}

**Description**: Retrieve a single material by ID

**Changes**: Response now includes `density`, `costPerKg`, and `processParameters` fields.

**Response (200 OK)**:
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "name": "PLA Filament",
  "code": "PLA-001",
  "description": "Polylactic Acid filament for FDM printing",
  "pricePerUnit": 25.00,
  "density": 1.25,
  "costPerKg": 500.00,
  "processParameters": {
    "FdmVolumetricFlowRate": "15",
    "FdmMinLayerTime": "5"
  },
  "stockLevel": 100,
  "supplierId": "3fa85f64-5717-4562-b3fc-2c963f66afa7",
  "supplierName": "Filament Supplier Co.",
  "manufacturingProcesses": [...],
  "availableColors": [...],
  "postProcessingMethods": [...],
  "mechanicalProperties": [...],
  "createdBy": "admin@example.com",
  "createdAt": "2026-02-22T10:00:00Z",
  "updatedBy": null,
  "updatedAt": null,
  "version": "AAAAAAAAB9I=",
  "active": true
}
```

---

### GET /material/v1/materials

**Description**: List all materials

**Changes**: Response items now include new fields.

**Response (200 OK)**:
```json
[
  {
    "id": "...",
    "name": "PLA Filament",
    "code": "PLA-001",
    "density": 1.25,
    "costPerKg": 500.00,
    "processParameters": { ... },
    ...
  }
]
```

---

### POST /material/v1/materials

**Description**: Create a new material

**Changes**: Request now accepts `density`, `costPerKg`, and `processParameters` fields.

**Request Body**:
```json
{
  "name": "ABS Filament",
  "code": "ABS-001",
  "description": "Acrylonitrile Butadiene Styrene filament",
  "pricePerUnit": 30.00,
  "density": 1.04,
  "costPerKg": 600.00,
  "processParameters": {
    "FdmVolumetricFlowRate": "12",
    "FdmMinLayerTime": "8"
  },
  "stockLevel": 50,
  "supplierId": null,
  "manufacturingProcessIds": [],
  "colorIds": [],
  "postProcessingMethodIds": [],
  "mechanicalProperties": []
}
```

**Validation Errors (400 Bad Request)**:
```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Density": ["Density must be between 0 and 25 g/cm³"],
    "CostPerKg": ["CostPerKg must be between 0 and 999,999 THB"]
  }
}
```

**Response (201 Created)**:
```json
{
  "id": "new-guid-here",
  "name": "ABS Filament",
  "code": "ABS-001",
  "density": 1.04,
  "costPerKg": 600.00,
  "processParameters": {
    "FdmVolumetricFlowRate": "12",
    "FdmMinLayerTime": "8"
  },
  ...
}
```

---

### PUT /material/v1/materials/{id}

**Description**: Update an existing material

**Changes**: Request now accepts new fields; they are persisted on update.

**Request Body**:
```json
{
  "name": "ABS Filament Premium",
  "code": "ABS-001",
  "description": "Updated description",
  "pricePerUnit": 35.00,
  "density": 1.05,
  "costPerKg": 650.00,
  "processParameters": {
    "FdmVolumetricFlowRate": "14"
  },
  "stockLevel": 45,
  "supplierId": null,
  "manufacturingProcessIds": [],
  "colorIds": [],
  "postProcessingMethodIds": [],
  "mechanicalProperties": [],
  "version": "AAAAAAAAB9I="
}
```

**Response (200 OK)**: Returns updated material with new field values.

---

## Unchanged Endpoints

### Inventory/Stock Endpoints

**Route**: `material/v1/inventory/*` (unchanged)

The controller class is renamed from `InventoryController` to `StockController`, but the API routes remain identical:

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/material/v1/inventory/{materialId}` | Get stock level |
| POST | `/material/v1/inventory/count` | Record inventory count |
| POST | `/material/v1/inventory/adjust` | Adjust inventory |

**No API contract changes** for inventory endpoints.

---

## Backward Compatibility

| Aspect | Status |
|--------|--------|
| Breaking Changes | None |
| New Required Fields | No (all have defaults) |
| Response Schema | Additive only |
| Request Schema | Additive only |
| Route Changes | None |

Existing API consumers will continue to work without modification. New fields are optional on create (default to 0/empty) and always returned in responses.
