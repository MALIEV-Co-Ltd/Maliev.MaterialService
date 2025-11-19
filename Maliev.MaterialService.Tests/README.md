# Material Service Tests

This directory contains unit and integration tests for the Material Service API.

## Test Structure

- **Integration/**: API integration tests using `WebApplicationFactory` and **Testcontainers**
- **Unit/**: Unit tests for services and business logic
- **Performance/**: Load and stress testing scripts using k6

## Prerequisites

### For Integration Tests

Integration tests use **Testcontainers** to automatically spin up PostgreSQL, Redis, and RabbitMQ containers. 

**Requirements**:
- Docker Desktop must be running
- Docker daemon must be accessible (default socket or TCP)

## Running Tests

### All Tests

```bash
# Runs both unit and integration tests
# Requires Docker for integration tests
dotnet test
```

### Unit Tests Only

Unit tests use in-memory databases and mocked dependencies:

```bash
dotnet test --filter "FullyQualifiedName~Unit"
```

### Integration Tests Only

Integration tests use Testcontainers (requires Docker):

```bash
dotnet test --filter "FullyQualifiedName~Integration"
```

**What happens during integration tests**:
1. Testcontainers automatically pulls and starts:
   - PostgreSQL 16 Alpine
   - Redis 7 Alpine
   - RabbitMQ 3 Management Alpine
2. Tests run against real database instances
3. Containers are automatically stopped and removed after tests complete

### Performance Tests

Performance tests require the API to be running:

```bash
# Start the API
cd Maliev.MaterialService.Api
dotnet run

# In another terminal, run k6 tests
cd Maliev.MaterialService.Tests/Performance
k6 run load-test.js
```

## Test Helpers

- **IntegrationTestFixture**: Manages Testcontainers lifecycle for PostgreSQL, Redis, and RabbitMQ
- **TestAuthHandler**: Mocks JWT authentication for integration tests
- **TestCacheService**: In-memory cache implementation for unit testing
- **WebApplicationFactory**: Configured to use Testcontainer endpoints

## CI/CD Considerations

For CI/CD pipelines, ensure:
- Docker is available (most CI providers support Docker-in-Docker or Docker socket mounting)
- Sufficient resources for running 3 containers simultaneously
- Network connectivity to pull container images

### GitHub Actions Example

```yaml
- name: Run Tests
  run: dotnet test
  env:
    DOCKER_HOST: unix:///var/run/docker.sock
```

### Azure Pipelines Example

```yaml
- task: Docker@2
  inputs:
    command: 'login'
- script: dotnet test
  displayName: 'Run Tests'
```

## Troubleshooting

### "Cannot connect to Docker daemon"
- Ensure Docker Desktop is running
- Check Docker is accessible: `docker ps`

### Tests timeout
- Increase test timeout if pulling images for the first time
- Check Docker resource limits (memory/CPU)

### Port conflicts
- Testcontainers uses random ports, so conflicts are rare
- If issues persist, stop all Docker containers: `docker stop $(docker ps -aq)`

## Test Coverage

- **Unit Tests**: Service layer business logic
- **Integration Tests**: API endpoints, database operations, caching, messaging
- **Performance Tests**: Load, stress, and endurance testing with k6
