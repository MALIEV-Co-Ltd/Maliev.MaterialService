# Research: Permission-Based Authorization Migration

## Decision 1: Permission Enforcement Pattern
**Decision**: Use the standard `[RequirePermission]` attribute from `Maliev.Aspire.ServiceDefaults`.
**Rationale**: This attribute follows the cross-service standard at Maliev, evaluating the `permissions` claim directly from the JWT with support for wildcard matching and enhanced auditing.
**Alternatives considered**: 
- Custom `IAuthorizationPolicyProvider`: Rejected in favor of the standard attribute to maintain consistency with other services like Accounting and AuthService.

## Decision 2: Permission Source of Truth
**Decision**: Centralized IAM Service.
**Rationale**: Role-to-permission mappings are managed centrally in the IAM Service. This service account (MaterialService) registers its capabilities on startup, and the IAM Service issues JWTs with the appropriate `permissions` claim based on the user's roles.
**Alternatives considered**:
- Local DB mapping: Rejected to avoid duplicating IAM logic and data across every microservice.

## Decision 3: Token Caching Policy
**Decision**: Delegated to IAM Service.
**Rationale**: The local service relies on the JWT claims for authorization decisions. Token validation and claim lookup performance is optimized at the IAM layer using distributed caching (Redis).
**Alternatives considered**:
- Local caching of mappings: No longer needed as permissions are included in the token.

## Decision 4: Event Publishing
**Decision**: MassTransit with RabbitMQ using standard contracts.
**Rationale**: Inter-service communication follows the established pattern using shared event definitions (e.g., `MaterialCreated`) to ensure eventual consistency across the Maliev platform.
