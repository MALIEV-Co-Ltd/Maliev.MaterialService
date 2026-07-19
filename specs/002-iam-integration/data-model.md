# Data Model: IAM Integration

## Centralized RBAC Model

The Material Service delegates the ownership and management of Permissions and Roles to the centralized **Maliev IAM Service**. 

### Registered Capabilities

On startup, the Material Service registers the following capabilities with IAM:

- **Service Name**: `MaterialService`
- **Permissions**: 14 granular permissions (e.g., `material.materials.create`, `material.inventory.adjust`).
- **Predefined Roles**: 4 roles (`roles.material.admin`, `roles.material.manager`, etc.) with their associated permission mappings.

## Local Data Impact

The Material Service database **does not** contain tables for Permissions or Roles. 

### Identity Reference

- **Audit Fields**: The `CreatedBy` and `UpdatedBy` fields in domain entities (Material, Category, etc.) store the `PrincipalId` (Subject) extracted from the JWT token.
- **Permission Evaluation**: Access control is performed by matching the required permission string against the `permissions` claim collection present in the user's JWT.

## Event Schema

Identity-related changes (e.g., Role Granted) are consumed via the IAM Service events if local reaction is required (currently not implemented as logic is stateless/claims-based).
