# MaterialService Specification - Permission-Based Authorization Migration

## Permissions to Define

### Material Operations
```
material.materials.create        - Create new materials
material.materials.read          - Read material details
material.materials.update        - Update material information
material.materials.delete        - Delete materials
material.materials.export        - Export material data
```

### Inventory Operations
```
material.inventory.view          - View inventory levels
material.inventory.adjust        - Adjust inventory quantities
material.inventory.transfer      - Transfer materials between locations
material.inventory.count         - Perform inventory counts
```

### Category Operations
```
material.categories.create       - Create material categories
material.categories.read         - Read categories
material.categories.update       - Update categories
material.categories.delete       - Delete categories
```

## Predefined Roles

### material-admin
**Permissions**: All material.* permissions

### material-manager
**Permissions**: create, read, update, inventory.*, categories.*

### material-clerk
**Permissions**: create, read, update, inventory.view, inventory.count

### material-viewer
**Permissions**: read, inventory.view, categories.read

## Success Criteria
- [ ] ~14 permissions registered
- [ ] 4 predefined roles registered
