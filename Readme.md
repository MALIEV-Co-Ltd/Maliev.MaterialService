# Material Service API

A RESTful web service for managing material data with comprehensive CRUD operations, advanced querying, and enterprise-grade features.

## Features

### Core Functionality
- ✅ **Material Management**: Full CRUD operations for materials
- ✅ **Advanced Querying**: Pagination, filtering, sorting, and search
- ✅ **Related Data**: Manufacturing processes, colors, mechanical properties, suppliers
- ✅ **Soft Delete**: Materials are marked inactive instead of being physically deleted
- ✅ **Optimistic Concurrency**: Version-based conflict detection

### Enterprise Features
- ✅ **Authentication & Authorization**: JWT-based with role-based access control
- ✅ **Caching**: Redis distributed caching with automatic invalidation
- ✅ **Logging**: Structured logging with Serilog
- ✅ **API Documentation**: OpenAPI 3.0 with Scalar UI
- ✅ **Health Checks**: Liveness and readiness endpoints
- ✅ **Metrics**: Prometheus metrics for monitoring
- ✅ **Rate Limiting**: Built-in ASP.NET Core rate limiting
- ✅ **CORS**: Configurable cross-origin resource sharing
- ✅ **Database Migrations**: EF Core migrations for PostgreSQL

## Tech Stack

- **Framework**: ASP.NET Core 10.0
- **Database**: PostgreSQL with Entity Framework Core
- **Caching**: Redis (StackExchange.Redis)
- **Messaging**: RabbitMQ (MassTransit)
- **Logging**: Serilog
- **API Documentation**: OpenAPI 3.0 + Scalar
- **Metrics**: Prometheus

## Prerequisites

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/) (for PostgreSQL and Redis)
- [Docker Compose](https://docs.docker.com/compose/)

## Quick Start

### 1. Start Infrastructure Services

```powershell
# Start PostgreSQL and Redis using Docker Compose
docker-compose -f docker-compose.dev.yml up -d
```

### 2. Apply Database Migrations

```powershell
# Navigate to the solution directory
cd r:\maliev\Maliev.MaterialService

# Apply migrations
dotnet ef database update --project Maliev.MaterialService.Data --startup-project Maliev.MaterialService.Api
```

### 3. Run the Application

```powershell
# Run the API
dotnet run --project Maliev.MaterialService.Api
```

The API will start at:
- **HTTP**: `http://localhost:5007`
- **HTTPS**: `https://localhost:7133`

The **Scalar UI** will automatically open in your browser at:
- `https://localhost:7133/scalar/v1`

## API Endpoints

### Materials

| Method | Endpoint | Description | Authorization |
|--------|----------|-------------|---------------|
| GET | `/materials/v1/materials` | Get materials (with filtering, pagination, sorting) | None |
| GET | `/materials/v1/materials/{id}` | Get material by ID | None |
| POST | `/materials/v1/materials` | Create new material | EmployeeOrHigher |
| PUT | `/materials/v1/materials/{id}` | Update material | EmployeeOrHigher |
| DELETE | `/materials/v1/materials/{id}` | Delete material (soft delete) | Manager |

### Bulk Operations

| Method | Endpoint | Description | Authorization |
|--------|----------|-------------|---------------|
| POST | `/materials/v1/bulk/import` | Bulk import materials | EmployeeOrHigher |
| GET | `/materials/v1/bulk/export` | Bulk export materials | Employee |

### Query Parameters (GET /materials/v1/materials)

- `page` - Page number (default: 1)
- `pageSize` - Items per page (default: 10, max: 100)
- `search` - Search in name, code, or description
- `sortBy` - Sort field: `name`, `code`, `price`, `stock`, `createdat`
- `sortDesc` - Sort descending (default: false)
- `minPrice` - Minimum price filter
- `maxPrice` - Maximum price filter
- `supplierId` - Filter by supplier ID

### Health & Monitoring

| Endpoint | Description |
|----------|-------------|
| `/liveness` | Liveness probe |
| `/readiness` | Readiness probe (checks DB, Redis) |
| `/metrics` | Prometheus metrics |

## Configuration

### Connection Strings

Update `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=material_app_db;Username=postgres;Password=password",
    "Redis": "localhost:6379",
    "RabbitMq": "amqp://guest:guest@localhost:5672"
  }
}
```

### Authorization Roles

- **Customer**: Read-only access
- **Employee**: Can create and update materials
- **Manager**: Can delete materials
- **Admin**: Full access

## Development

### Project Structure

```
Maliev.MaterialService/
├── Maliev.MaterialService.Api/          # Web API project
│   ├── Controllers/                     # API controllers
│   ├── DTOs/                            # Data transfer objects
│   ├── Services/                        # Business logic
│   ├── Mapping/                         # Explicit domain/DTO mappers
│   └── Middleware/                      # Custom middleware
├── Maliev.MaterialService.Data/         # Data access layer
│   ├── Entities/                        # Entity models
│   ├── Configurations/                  # EF Core configurations
│   ├── DbContext/                       # Database context
│   └── Migrations/                      # EF Core migrations
└── Maliev.MaterialService.Tests/        # Test project
```

### Build

```powershell
dotnet build
```

### Run Tests

```powershell
dotnet test
```

### Create Migration

```powershell
dotnet ef migrations add MigrationName --project Maliev.MaterialService.Data --startup-project Maliev.MaterialService.Api
```

### Apply Migration

```powershell
dotnet ef database update --project Maliev.MaterialService.Data --startup-project Maliev.MaterialService.Api
```

## Docker

### Build Docker Image

```powershell
docker build -t material-service:latest -f Maliev.MaterialService.Api/Dockerfile .
```

### Run with Docker

```powershell
docker run -d -p 5007:80 --name material-service material-service:latest
```

## Example Requests

### Create Material

```http
POST /materials/v1/materials
Content-Type: application/json
Authorization: Bearer {token}

{
  "name": "PLA Filament",
  "code": "PLA-001",
  "description": "3D printing filament",
  "pricePerUnit": 25.99,
  "stockLevel": 100,
  "manufacturingProcessIds": [],
  "colorIds": [],
  "postProcessingMethodIds": [],
  "mechanicalProperties": []
}
```

### Get Materials with Filters

```http
GET /materials/v1/materials?page=1&pageSize=10&search=PLA&sortBy=price&minPrice=20&maxPrice=50
```

### Update Material

```http
PUT /materials/v1/materials/{id}
Content-Type: application/json
Authorization: Bearer {token}

{
  "name": "PLA Filament Updated",
  "code": "PLA-001",
  "description": "Updated description",
  "pricePerUnit": 27.99,
  "stockLevel": 150,
  "version": 1,
  "manufacturingProcessIds": [],
  "colorIds": [],
  "postProcessingMethodIds": [],
  "mechanicalProperties": []
}
```

## Monitoring

### Prometheus Metrics

Access metrics at `/metrics` for:
- HTTP request duration
- HTTP request count
- Database query duration
- Database query count
- Custom business metrics

### Health Checks

- **Liveness**: `/liveness` - Returns 200 if the app is running
- **Readiness**: `/readiness` - Returns 200 if the app can serve requests (DB + Redis healthy)

## License

Proprietary - MALIEV Co., Ltd.

## Support

For issues or questions, contact the development team.
