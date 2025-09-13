# Maliev Material Service

A comprehensive material management microservice for Maliev Co. Ltd.'s 3D printing and manufacturing operations. This service handles material inventory tracking, specifications, supplier management, and material lifecycle operations.

## Overview

The Material Service is part of Maliev's microservices architecture, built with ASP.NET Core 9.0 and following GitOps deployment patterns. It provides RESTful APIs for managing materials used in 3D printing and manufacturing processes.

### Key Features
- Material inventory management
- Material specifications and properties tracking
- Supplier relationship management
- Real-time inventory updates
- Integration with manufacturing workflows
- Comprehensive monitoring and health checks

## Technology Stack

- **Framework**: ASP.NET Core 9.0
- **Database**: PostgreSQL with Entity Framework Core
- **Authentication**: JWT Bearer tokens
- **Monitoring**: Prometheus metrics, Serilog logging
- **Deployment**: Kubernetes with GitOps (ArgoCD)
- **Testing**: xUnit with FluentAssertions and Moq

## Project Structure

```
Maliev.MaterialService/
├── .github/workflows/          # CI/CD pipelines (develop, staging, main)
├── Maliev.MaterialService.Api/ # Web API project
├── Maliev.MaterialService.Data/# Data layer with EF Core
├── Maliev.MaterialService.Tests/# Unit and integration tests
└── Maliev.MaterialService.sln # Solution file
```

## Getting Started

### Prerequisites
- .NET 9.0 SDK
- Docker (for containerization)
- PostgreSQL (for database)
- kubectl (for Kubernetes operations)

### Local Development

1. **Clone the repository**
   ```bash
   git clone https://github.com/MALIEV-Co-Ltd/Maliev.MaterialService.git
   cd Maliev.MaterialService
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore Maliev.MaterialService.sln
   ```

3. **Build the solution**
   ```bash
   dotnet build Maliev.MaterialService.sln
   ```

4. **Run tests**
   ```bash
   dotnet test Maliev.MaterialService.sln --verbosity normal
   ```

5. **Configure database connection**
   - Set up local PostgreSQL instance or use port forwarding to development cluster
   - Update connection string in configuration

6. **Run the application**
   ```bash
   dotnet run --project Maliev.MaterialService.Api
   ```

The API will be available at `https://localhost:5001` with Swagger UI at `/materials/swagger`.

## API Endpoints

The service exposes RESTful APIs under the `/materials` base path:

- **Health Checks**
  - `GET /materials/liveness` - Liveness probe
  - `GET /materials/readiness` - Readiness probe with dependency checks

- **Monitoring**
  - `GET /materials/metrics` - Prometheus metrics endpoint

- **API Documentation**
  - `/materials/swagger` - Swagger UI

## Configuration

### Environment Variables
The service uses Google Secret Manager for production secrets. For development, you can use local configuration files.

### Database Connection
```bash
# For local development with port forwarding
kubectl port-forward -n maliev-dev svc/postgres-cluster-rw 5432:5432
```

### Database Migrations
```bash
# Set connection string
export MaterialServiceDbContext="Server=localhost;Port=5432;Database=material_service_db;User Id=postgres;Password=PASSWORD;"

# Apply migrations
dotnet ef database update --project Maliev.MaterialService.Data
```

## Deployment

### Kubernetes Deployment
The service is deployed using GitOps with ArgoCD. Deployment manifests are maintained in the `maliev-gitops` repository.

```bash
# View service status
kubectl get pods -n maliev-dev -l app=maliev-material-service

# View logs
kubectl logs -f deployment/maliev-material-service -n maliev-dev

# Port forward for local access
kubectl port-forward -n maliev-dev svc/maliev-material-service 8080:8080
```

### CI/CD Pipelines
The service includes three CI/CD workflows:
- `ci-develop.yml` - Builds and deploys to development environment
- `ci-staging.yml` - Builds and deploys to staging environment
- `ci-main.yml` - Builds and deploys to production environment

## Monitoring

### Access Grafana Dashboard
```powershell
cd maliev-gitops
.\scripts\open-grafana.ps1
```

### Key Metrics
- HTTP request metrics (duration, count, errors)
- Database connection health
- Memory and CPU usage
- Custom business metrics for material operations

### Logging
The service uses Serilog for structured logging with console output. Logs include:
- Request/response correlation IDs
- User context and authentication info
- Database operation performance
- Error details and stack traces

## Testing

### Run All Tests
```bash
dotnet test Maliev.MaterialService.sln --verbosity normal
```

### Test Coverage
The test suite includes:
- Unit tests for business logic
- Integration tests for API endpoints
- Database integration tests
- Health check validation

## Development Guidelines

### Code Quality
- All warnings treated as errors
- Follow established coding conventions
- Maintain test coverage above 80%
- Use FluentAssertions for test assertions

### Security
- JWT authentication required for all endpoints (except health checks)
- No secrets in source code
- Use Google Secret Manager for production secrets
- Regular security dependency updates

### Performance
- Prometheus metrics for monitoring
- Database query optimization
- Caching strategies for frequently accessed data
- Rate limiting for API protection

## Contributing

1. Create feature branch from `develop`
2. Implement changes with tests
3. Ensure all CI checks pass
4. Create pull request to `develop`
5. After review and merge, changes deploy automatically

## Support

For issues and questions:
- Check Grafana dashboards for service health
- Review logs in Kubernetes cluster
- Create GitHub issue for bugs or feature requests

## License

Internal use only - Maliev Co. Ltd.