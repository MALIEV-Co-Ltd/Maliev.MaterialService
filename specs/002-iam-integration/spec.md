# Feature Specification: Permission-Based Authorization Migration

**Feature Branch**: `002-iam-integration`  
**Created**: 2025-12-22  
**Status**: Draft  
**Input**: User description: "Permission-Based Authorization Migration"

## Clarifications

### Session 2025-12-22
- Q: IAM Source → A: External OIDC/OAuth2 (Keycloak, Auth0, Entra ID)
- Q: Role-to-Permission Mapping Storage → A: Centralized in IAM Service (Roles mapped to Permissions at IAM level)
- Q: Public Access Scope → A: Read-only (Materials)
- Q: Permission Enforcement Level → A: Controller/Action level (via ServiceDefaults RequirePermission attribute)
- Q: Token Caching Policy → A: Distributed Cache (Redis) used by IAM for token validation and claim lookup

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Full System Administration (Priority: P1)

As a system administrator, I want to have unrestricted access to all material service operations so that I can manage the entire system without limitations.

**Why this priority**: Essential for system setup, maintenance, and handling critical operations that others cannot.

**Independent Test**: Can be fully tested by authenticating as a user with the `material-admin` role and successfully performing any operation (create, read, update, delete, export, inventory management, category management).

**Acceptance Scenarios**:

1. **Given** a user with the `material-admin` role, **When** they attempt to delete a material, **Then** the operation is permitted.
2. **Given** a user with the `material-admin` role, **When** they attempt to export material data, **Then** the operation is permitted.

---

### User Story 2 - Material Management (Priority: P2)

As a material manager, I want to manage materials, inventory, and categories so that I can keep the catalog and stock levels up to date without having full administrative destructive powers like deleting materials.

**Why this priority**: Core business flow for managing the material lifecycle and organizational structure.

**Independent Test**: Can be fully tested by authenticating as a user with the `material-manager` role and verifying they can create/update materials and manage categories/inventory, but are blocked from deleting materials.

**Acceptance Scenarios**:

1. **Given** a user with the `material-manager` role, **When** they attempt to update a material category, **Then** the operation is permitted.
2. **Given** a user with the `material-manager` role, **When** they attempt to delete a material, **Then** the operation is denied with an "Unauthorized" response.

---

### User Story 3 - Daily Inventory Operations (Priority: P3)

As a material clerk, I want to record new materials and perform inventory counts so that the physical stock matches the digital records.

**Why this priority**: Necessary for day-to-day operations and maintaining data accuracy.

**Independent Test**: Can be fully tested by authenticating as a user with the `material-clerk` role and verifying they can create materials and perform inventory counts, but cannot modify categories or transfer inventory.

**Acceptance Scenarios**:

1. **Given** a user with the `material-clerk` role, **When** they attempt to perform an inventory count, **Then** the operation is permitted.
2. **Given** a user with the `material-clerk` role, **When** they attempt to transfer materials between locations, **Then** the operation is denied.

---

### User Story 4 - Read-Only Auditing (Priority: P4)

As an auditor or viewer, I want to see material details and inventory levels without being able to make any changes so that I can verify data without risk of accidental modification.

**Why this priority**: Important for reporting and oversight without compromising data integrity.

**Independent Test**: Can be fully tested by authenticating as a user with the `material-viewer` role and verifying all read operations work while all write/delete operations are blocked.

**Acceptance Scenarios**:

1. **Given** a user with the `material-viewer` role, **When** they attempt to view inventory levels, **Then** the data is displayed.
2. **Given** a user with the `material-viewer` role, **When** they attempt to create a new material, **Then** the operation is denied.

### Edge Cases

- **Multiple Roles**: How does the system handle a user assigned multiple roles? (Assumption: Permissions are additive).
- **Unknown Permissions**: How does the system handle a request for a permission that doesn't exist in the registry? (Assumption: Deny by default).
- **Missing Role Mapping**: What happens if a user has no roles assigned? (Assumption: User has no permissions beyond public access).
- **Anonymous Access**: Public users can access material read operations without authentication.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: System MUST define 14 permissions following the 'material.resource.action' format.
- **FR-002**: System MUST register permissions and predefined roles with the IAM Service on startup using a hosted registration service.
- **FR-003**: System MUST integrate with the IAM Service for client-side token validation and resilient communication.
- **FR-004**: System MUST enforce permission checks at the Controller/Action level using the standard `RequirePermission` attribute from ServiceDefaults.
- **FR-005**: System MUST evaluate permissions from the `permissions` claim within the JWT token.
- **FR-006**: System MUST support wildcard permission matching (e.g., 'material.inventory.*') as provided by the standard matcher.
- **FR-007**: System MUST support enhanced audit logging for critical operations marked in the authorization attributes.
- **FR-008**: System MUST allow public (unauthenticated) access to material read-only endpoints.

### Key Entities *(include if feature involves data)*

- **Permission**: A granular action identifier (e.g., `material.materials.create`) that represents a specific capability within the service.
- **Role**: A named collection of permissions (e.g., `material-clerk`) used to simplify permission management for users.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: All 14 specified permissions are correctly registered in the system.
- **SC-002**: All 4 predefined roles are correctly registered with their assigned permissions.
- **SC-003**: 100% of protected API endpoints correctly enforce their required permissions.
- **SC-004**: Permission check overhead adds less than 10ms to the total API response time.
- **SC-005**: Unauthorized access attempts are blocked and logged with the relevant user identity and attempted permission.
- **SC-006**: Permission mappings are retrieved from a distributed cache (Redis) to ensure high performance and consistency.