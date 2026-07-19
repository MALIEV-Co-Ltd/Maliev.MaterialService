# Feature Specification: Material WebAPI Service

**Feature Branch**: `001-material-webapi`  
**Created**: 2024-11-18  
**Status**: Draft  
**Input**: User description: "Create a Material WebAPI service that serves as the central authoritative source for all material data used across Maliev Manufacturing services. The service must support full CRUD operations for materials, including creation, retrieval, update, and deletion, and store rich metadata about each material. Each material record must contain fields such as name, code/ID, applicable manufacturing processes (e.g., 3D Printing, CNC machining, injection molding), available colors, mechanical properties (tensile strength, hardness, elasticity, etc.), recommended post-processing methods (e.g., sanding, polishing, annealing), price per unit, stock level or inventory tracking, and optionally supplier information. Additional metadata such as createdBy, updatedBy, timestamps, version number, and active flag must be maintained to support auditing and concurrency control. The service must enforce strict validation for key fields, including unique identifiers, supported manufacturing processes, and valid mechanical property ranges. Caching is critical: implement an in-process LRU cache per instance for the most frequently requested materials, combined with a distributed Redis cache to minimize database queries and ensure sub-50ms response times for common read operations. Cache invalidation must occur immediately on write operations, and support stale-while-revalidate strategies for high concurrency reads without overloading the database. The API surface should include endpoints for querying materials by name, code, manufacturing process compatibility, mechanical property ranges, color availability, and price ranges. The service should allow bulk import and export of material data in CSV or JSON formats, with validation and dry-run modes to avoid corrupting the dataset. Administrative operations must be protected by role-based access control, while read endpoints may be public or internal depending on security policy. The Material Service must support extensibility to add new manufacturing processes, properties, or post-processing options without breaking existing API contracts. It should maintain a lightweight audit trail for all changes to allow traceability and support analytics on material usage, pricing trends, and stock levels. The service should expose health, readiness, and metrics endpoints suitable for Prometheus, and ensure robust error handling with clear validation error responses. The system must prioritize performance and minimal resource usage, serving frequent reads from cache whenever possible, and allow efficient pagination, filtering, and sorting of query results. Ensure the database schema supports indexing on frequently queried fields such as material name, process compatibility, and stock levels. Include operational considerations such as cache warming on startup, cache eviction policies, and handling of concurrent updates to material records. Request that the specification generator produce OpenAPI 3.0 YAML with examples for CRUD and query operations, SQL DDL for materials and related lookup tables (colors, post-processing methods, mechanical properties), sequence diagrams for read and update flows demonstrating cache usage and invalidation, and a concise operational playbook covering caching, audit trails, bulk import, and extension of material properties."

## User Scenarios & Testing

### User Story 1 - Manage Material Data (Priority: P1)

As a manufacturing engineer, I want to create, view, update, and delete material records so that I can maintain an accurate and up-to-date inventory of materials.

**Why this priority**: This is the core functionality of the service, enabling the management of material data which is central to Maliev Manufacturing operations. Without this, the service cannot fulfill its primary purpose.

**Independent Test**: Can be fully tested by performing CRUD operations on individual material records via the API and verifying data persistence and accuracy.

**Acceptance Scenarios**:

1.  **Given** I am an authenticated manufacturing engineer, **When** I submit a request to create a new material with valid data, **Then** the material record is successfully created and retrievable, and a unique identifier is assigned.
2.  **Given** an existing material record, **When** I request to retrieve its details, **Then** the complete and accurate material information is returned.
3.  **Given** an existing material record, **When** I submit a request to update its properties with valid data, **Then** the material record is updated, and the changes are reflected in subsequent retrievals.
4.  **Given** an existing material record, **When** I submit a request to delete it, **Then** the material record is marked as inactive or removed, and no longer retrievable via standard queries.
5.  **Given** I submit a request to create or update a material with invalid or missing required data, **When** the request is processed, **Then** an appropriate validation error message is returned, and no changes are made to the material data.

### User Story 2 - Query Material Data (Priority: P1)

As a product designer, I want to search for materials based on various criteria (e.g., name, properties, color) so that I can find suitable materials for my designs quickly.

**Why this priority**: Efficient material discovery is crucial for product design and development, directly impacting time-to-market and material selection accuracy.

**Independent Test**: Can be fully tested by performing various filtered queries on a populated dataset and verifying that the correct materials are returned based on the specified criteria.

**Acceptance Scenarios**:

1.  **Given** a collection of material records exists, **When** I query materials by name (partial or exact match), **Then** a list of matching materials is returned, ordered by relevance.
2.  **Given** a collection of material records exists, **When** I query materials by manufacturing process compatibility, **Then** a list of materials compatible with the specified process is returned.
3.  **Given** a collection of material records exists, **When** I query materials by a range of mechanical properties (e.g., tensile strength between X and Y), **Then** a list of materials meeting the property criteria is returned.
4.  **Given** a collection of material records exists, **When** I query materials by available color, **Then** a list of materials available in the specified color is returned.
5.  **Given** a collection of material records exists, **When** I query materials with pagination, filtering, and sorting parameters, **Then** the results are returned efficiently according to the specified parameters.

### User Story 3 - Bulk Material Operations (Priority: P2)

As a data administrator, I want to import and export material data in bulk so that I can efficiently manage large datasets and integrate with other systems.

**Why this priority**: Bulk operations are essential for initial data population, large-scale updates, and data migration, significantly reducing manual effort.

**Independent Test**: Can be fully tested by importing a large dataset (CSV/JSON) with dry-run and validation, then performing a full import, and finally exporting the data to verify integrity.

**Acceptance Scenarios**:

1.  **Given** I have a CSV or JSON file containing new material data, **When** I initiate a bulk import with a dry-run option, **Then** the system validates the data and reports any errors without making permanent changes.
2.  **Given** I have a valid CSV or JSON file containing new material data, **When** I initiate a bulk import, **Then** the system processes the data, creates new material records, and reports the success or failure of each record.
3.  **Given** existing material data in the service, **When** I request a bulk export in CSV or JSON format, **Then** the system generates and provides a file containing all requested material data.

### User Story 4 - Monitor Service Health (Priority: P3)

As a DevOps engineer, I want to monitor the health and performance of the Material Service so that I can ensure its continuous availability and optimal operation.

**Why this priority**: Operational visibility is important for maintaining service reliability and quickly identifying and resolving issues.

**Independent Test**: Can be fully tested by querying the health, readiness, and metrics endpoints and verifying that they return expected status codes and data in Prometheus format.

**Acceptance Scenarios**:

1.  **Given** the Material Service is running, **When** I access the health endpoint, **Then** it returns a 200 OK status if all critical components are operational.
2.  **Given** the Material Service is running, **When** I access the readiness endpoint, **Then** it returns a 200 OK status if it is ready to accept traffic.
3.  **Given** the Material Service is processing requests, **When** I access the metrics endpoint, **Then** it exposes relevant operational metrics in Prometheus format.

### Edge Cases

-   What happens when a material record is requested but does not exist? The service should return a 404 Not Found error.
-   How does the system handle concurrent write operations to the same material record? The system should implement optimistic concurrency control (e.g., using version numbers or timestamps) to prevent data corruption and inform the user of conflicts.
-   What happens if the distributed Redis cache is unavailable? The service should gracefully degrade, falling back to direct database access, potentially with increased latency, and log the cache unavailability.
-   How does the system handle bulk import files with malformed data or exceeding size limits? The system should reject the import, provide detailed error messages for malformed records, and enforce reasonable file size limits.

## Requirements

### Non-Functional Requirements

-   **NFR-001**: The service MUST support up to 1 million material records.
-   **NFR-002**: The service MUST handle up to 1000 read transactions per second.
-   **NFR-002.1**: During peak load (up to 1000 reads/sec), common read operations (by ID, name, process) MUST maintain sub-100ms response times for 90% of requests.
-   **NFR-003**: The service MUST handle up to 100 write transactions per second.
-   **NFR-003.1**: During peak load (up to 100 writes/sec), write operations MUST maintain sub-200ms response times for 90% of requests.
-   **NFR-004**: The service MUST support horizontal scaling up to 5 instances.
-   **NFR-005**: The service MUST achieve 99.9% uptime (max 8.76 hours downtime/year).
-   **NFR-006**: The service MUST adhere to GDPR, Thai tax law, and relevant industry standards.

### Functional Requirements

-   **FR-000**: The service MUST implement API versioning using URL path segments (e.g., /api/v1/materials) to ensure backward compatibility.
-   **FR-001**: The service MUST provide RESTful API endpoints for creating, retrieving, updating, and deleting individual material records.
-   **FR-002**: Each material record MUST include fields for name, code/ID (unique), applicable manufacturing processes, available colors, mechanical properties (structured as key-value pairs to support flexibility), recommended post-processing methods, price per unit, stock level, and optional supplier information.
-   **FR-002.1**: Mechanical properties MUST be stored as a collection of property-value pairs with units of measurement (e.g., {"property": "tensile strength", "value": 40, "unit": "MPa"}).
-   **FR-002.2**: The system MUST support predefined mechanical property types while allowing for extensibility to add new property types without schema changes.
-   **FR-002.3**: Optional supplier information MUST include supplier name, contact person, email, phone, address, and lead time for delivery.
-   **FR-003**: The service MUST maintain metadata for each material record, including `createdBy`, `updatedBy`, `createdAt` (timestamp), `updatedAt` (timestamp), `version` number, and an `active` flag.
-   **FR-004**: The service MUST enforce strict validation for key fields, including unique identifiers (code/ID), supported manufacturing processes, and valid ranges for mechanical properties.
-   **FR-005**: The service MUST implement an in-process LRU cache per instance for frequently requested materials.
-   **FR-006**: The service MUST utilize a distributed Redis cache to minimize database queries for frequently requested materials.
-   **FR-007**: Cache invalidation MUST occur immediately upon any write operation (create, update, delete) to a material record.
-   **FR-008**: The service MUST support stale-while-revalidate strategies for high concurrency read operations to minimize database load. The implementation must:
    - Return stale cached data immediately while revalidating in the background
    - Update the cache with fresh data after successful revalidation
    - Maintain a configurable TTL (time-to-live) for cache entries
    - Implement circuit breaker pattern to prevent cache stampede during high load
    - Use appropriate cache keys that include version information for automatic cache busting
-   **FR-009**: The API MUST include endpoints for querying materials by name, code, manufacturing process compatibility, mechanical property ranges, color availability, and price ranges.
-   **FR-010**: The service MUST provide endpoints for bulk import of material data in CSV or JSON formats.
-   **FR-011**: Bulk import functionality MUST include data validation and a dry-run mode.
-   **FR-012**: The service MUST provide endpoints for bulk export of material data in CSV or JSON formats.
-   **FR-013**: Administrative operations (e.g., bulk import/export, system configuration) MUST be protected by role-based access control (RBAC).
-   **FR-014**: Read endpoints MAY be public or internal based on security policy configuration.
-   **FR-014.1**: The security policy MUST be configurable through environment variables or configuration settings.
-   **FR-014.2**: Public read endpoints require no authentication but may have rate limiting applied.
-   **FR-014.3**: Internal read endpoints require valid JWT authentication and appropriate role-based access.
-   **FR-014.4**: The default configuration MUST be internal access only, requiring explicit configuration change to enable public access.
-   **FR-014.5**: Configuration changes MUST be logged in the audit trail for compliance purposes.
-   **FR-015**: The service MUST be extensible to allow the addition of new manufacturing processes, properties, or post-processing options without breaking existing API contracts.
-   **FR-015.1**: Manufacturing processes MUST be implemented as a configurable lookup table that can be extended without code changes.
-   **FR-015.2**: Post-processing methods MUST be implemented as a configurable lookup table that can be extended without code changes.
-   **FR-015.3**: New mechanical property types MUST be addable through the key-value pair structure without schema modifications.
-   **FR-015.4**: API versioning strategy MUST follow backward-compatible patterns (major version increments only when breaking changes are necessary).
-   **FR-015.5**: The system MUST support configuration-driven extensibility for entity attributes via metadata tables.
-   **FR-016**: The service MUST maintain a lightweight audit trail for all changes to material records.
-   **FR-017**: The service MUST expose health (`/health`), readiness (`/ready`), and metrics (`/metrics`) endpoints in a format suitable for Prometheus.
-   **FR-017.1**: Business metrics MUST include material_created_total, material_updated_total, material_deleted_total counters for analytics.
-   **FR-017.2**: Business metrics MUST include query_operation_duration_seconds histogram for performance analysis.
-   **FR-017.3**: All metrics MUST be tagged with service_name, version, region, and environment labels as required by the company telemetry pipeline.
-   **FR-018**: The service MUST provide robust error handling with clear and informative validation error responses.
-   **FR-019**: Query results MUST support efficient pagination, filtering, and sorting.
-   **FR-020**: The database schema MUST support indexing on frequently queried fields such as material name, code/ID, manufacturing process compatibility, and stock levels.
-   **FR-021**: The service MUST implement cache warming on startup to pre-populate the cache with critical data.
-   **FR-022**: The service MUST define and implement cache eviction policies. The implementation must:
    - Use LRU (Least Recently Used) eviction for in-process cache
    - Implement TTL (Time-To-Live) based eviction for Redis distributed cache
    - Support configurable eviction thresholds and time intervals
    - Provide monitoring metrics for cache hit/miss ratios
    - Include cache warming strategies on service startup
    - Support manual cache invalidation for administrative purposes
-   **FR-023**: The service MUST handle concurrent updates to material records without data loss or corruption.
-   **FR-023.1**: The service MUST implement optimistic concurrency control using version numbers or timestamps.
-   **FR-023.2**: When concurrent updates conflict, the service MUST return HTTP 409 Conflict with details about the conflict.
-   **FR-023.3**: The client MUST handle concurrency conflicts by retrieving the latest version and presenting the conflict to the user for resolution.
-   **FR-023.4**: The system MUST log all concurrency conflicts for audit and analysis purposes.

### Key Entities

-   **Material**: Represents a single material used in manufacturing. Attributes include name, code/ID, description, manufacturing processes, colors, mechanical properties, post-processing methods, price, stock level, supplier info, and audit metadata.
-   **Manufacturing Process**: A lookup entity representing a specific manufacturing technique (e.g., 3D Printing, CNC machining). Materials can be associated with multiple processes.
-   **Color**: A lookup entity representing available colors for materials.
-   **Mechanical Property**: A lookup entity representing a specific mechanical characteristic (e.g., Tensile Strength, Hardness, Elasticity) with associated values for a material.
-   **Post-Processing Method**: A lookup entity representing methods applied after manufacturing (e.g., sanding, polishing, annealing).
-   **Supplier**: An optional entity representing the supplier of a material. Attributes include supplier name, contact person, email, phone, address, and lead time for delivery.

### Data Management & Recovery Requirements

-   **DR-001**: The service MUST implement automated database backup procedures with point-in-time recovery capability.
-   **DR-002**: Database backups MUST be performed daily with retention for 30 days, and transaction log backups every 15 minutes for point-in-time recovery.
-   **DR-003**: The service MUST support database restore procedures from backups with minimal data loss (RPO < 15 minutes) and recovery time (RTO < 4 hours).
-   **DR-004**: Backup data MUST be encrypted and stored in geographically separate locations from the primary database.
-   **DR-005**: The service MUST implement disaster recovery procedures including failover to a secondary region within 15 minutes of outage detection.

### Messaging & Integration Requirements

-   **MI-001**: The service MUST support event-driven communication via message queues for asynchronous operations (e.g., bulk import completions, audit trail notifications).
-   **MI-002**: The service MUST publish material change events (create, update, delete) to a message bus for interested consumers.
-   **MI-003**: The service MUST provide eventual consistency for cross-service operations through message-based communication.
-   **MI-004**: The service MUST implement circuit breaker patterns for resilience when communicating with external services via message queues.

## Out of Scope

-   Direct integration with ERP/MES systems.
-   Complex material lifecycle workflows (e.g., approval processes).

### Success Criteria

### Measurable Outcomes

-   **SC-001**: Common read operations (retrieving a material by ID, querying by name, querying by manufacturing process compatibility) MUST achieve sub-50ms response times for 95% of requests under normal load (up to 80% of maximum capacity: 800 reads/sec, 80 writes/sec).
-   **SC-001.1**: 'Common read operations' are defined as GET requests to retrieve single material records by ID and basic search queries by name, code, or manufacturing process.
-   **SC-001.2**: 'Normal load' is defined as up to 80% of the maximum specified transaction rates (800 reads/sec, 80 writes/sec) with database response times under 20ms average.
-   **SC-002**: The service MUST maintain data consistency across the database and caches, ensuring that read-after-write operations return the most recent data within 100ms.
-   **SC-003**: The service MUST successfully process bulk imports of up to 10,000 material records within 5 minutes, with an error rate of less than 0.1% for valid data.
-   **SC-004**: The service's health and readiness endpoints MUST consistently return a 200 OK status under normal operating conditions.
-   **SC-005**: The service MUST maintain an average CPU utilization below 70% and memory utilization below 80% under peak load conditions.
-   **SC-006**: The audit trail MUST accurately record all create, update, and delete operations on material records, including the user, timestamp, and changes made.
-   **SC-007**: The API documentation (OpenAPI 3.0 YAML) MUST be generated and accurately reflect all CRUD and query operations, including examples.
-   **SC-008**: The database schema (SQL DDL) MUST be generated and include appropriate indexing for frequently queried fields.

## Clarifications

### Session 2025-11-18

- Q: What are the expected data volume and transaction rates for the Material Service? → A: Up to 1 million material records, 1000 reads/sec, 100 writes/sec
- Q: What are the expected scalability requirements for the Material Service? → A: Support horizontal scaling up to 5 instances
- Q: What are the expected reliability and availability requirements for the Material Service? → A: 99.9% uptime (max 8.76 hours downtime/year)
- Q: Are there any specific functionalities or integrations that are explicitly out of scope for the initial release of the Material Service? → A: Direct integration with ERP/MES systems; complex material lifecycle workflows (e.g., approval processes)
- Q: Are there any specific compliance or regulatory constraints (e.g., GDPR, industry standards, local laws) that the Material Service must adhere to? → A: GDPR, Thai tax law, and relevant industry standards