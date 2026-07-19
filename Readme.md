# Maliev Material Service

[![Build Status](https://img.shields.io/badge/Build-Passing-success)](https://github.com/ORGANIZATION/Maliev.MaterialService)
[![.NET Version](https://img.shields.io/badge/.NET-10.0-blue)](https://dotnet.microsoft.com/download/dotnet/10.0)
[![Database](https://img.shields.io/badge/Database-PostgreSQL%2018-blue)](https://www.postgresql.org/)

Enterprise-grade material catalog and manufacturing resource management service.

**Role in MALIEV Architecture**: The authoritative repository for all manufacturing inputs. It manages the detailed properties of materials, manufacturing processes, and technical specifications used across the platform to calculate quotes, drive production, and manage supplier relationships.

---

## 🏗️ Architecture & Tech Stack

- **Framework**: ASP.NET Core 10.0 (C# 13)
- **Database**: PostgreSQL 18 with Entity Framework Core 10.x
- **Distributed Cache**: Redis 7.x (High-performance catalog discovery)
- **Messaging**: RabbitMQ via MassTransit
- **API Documentation**: OpenAPI 3.1 + Scalar UI
- **Observability**: OpenTelemetry (Metrics, Traces, Logging)

---

## ⚖️ Constitution Rules

This service strictly adheres to the platform development mandates:

### Banned Libraries
To maintain high performance and low complexity, the following are **NOT** used:
- ❌ **AutoMapper**: Explicit manual mapping only.
- ❌ **FluentValidation**: Standard Data Annotations (`[Required]`, `[EmailAddress]`) only.
- ❌ **FluentAssertions**: Standard xUnit `Assert` methods only.
- ❌ **In-memory Test DB**: All integration tests use **Testcontainers** with real PostgreSQL 18.

### Mandatory Practices
- ✅ **TreatWarningsAsErrors**: Enabled in all `.csproj` files.
- ✅ **XML Documentation**: Required on all public methods and properties.
- ✅ **No Secrets in Code**: All sensitive configuration injected via environment variables.
- ✅ **No Test Config in Program.cs**: Test configuration in test fixtures only.
- ✅ **IAM Integration**: Self-registers permissions with the IAM Service using GCP-style naming: `{service}.{resource}.{action}`.

---

## ✨ Key Features

- **Rich Material Catalog**: Comprehensive management of physical materials including mechanical properties, colors, and pricing.
- **Process Hierarchy**: Detailed modeling of manufacturing processes (3D Printing, CNC, etc.) and their specific technical constraints.
- **Bulk Integration Engine**: High-performance import/export tools for large-scale material list management and supplier updates.
- **Intelligent Search**: Advanced filtering and search capabilities based on technical specifications and stock levels.
- **Conflict-Free Updates**: Version-based optimistic concurrency ensures data integrity during multi-user catalog management.

---

## 🚀 Quick Start

### Prerequisites
- .NET 10.0 SDK
- Docker Desktop (for infrastructure)
- PostgreSQL 18 (Alpine)

### Local Development Setup

1. **Clone the repository**
```bash
git clone https://github.com/ORGANIZATION/Maliev.MaterialService.git
cd Maliev.MaterialService
```

2. **Spin up Infrastructure**
```bash
docker run --name material-db -e POSTGRES_PASSWORD=YOUR_PASSWORD -p 5432:5432 -d postgres:18-alpine
docker run --name material-redis -p 6379:6379 -d redis:7-alpine
```

3. **Configure Environment**
```powershell
# Windows PowerShell
$env:ConnectionStrings__MaterialDbContext="YOUR_POSTGRES_CONNECTION_STRING"
$env:ConnectionStrings__Cache="YOUR_REDIS_CONNECTION_STRING"
```

4. **Apply Migrations & Run**
```bash
dotnet ef database update --project Maliev.MaterialService.Data
dotnet run --project Maliev.MaterialService.Api
```

The service will be available at `http://localhost:5000/materials`. Access the interactive documentation at `http://localhost:5000/materials/scalar`.

---

## 📡 API Endpoints

All endpoints are prefixed with `/materials/v1/`.

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/materials` | Search and filter the material catalog |
| POST | `/materials` | Register a new material resource |
| POST | `/bulk/import` | Bulk upload material records |
| GET | `/materials/{id}` | Retrieve detailed technical specifications |

---

## 🏥 Health & Monitoring

Standardized health probes for Kubernetes orchestration:
- **Liveness**: `GET /materials/liveness`
- **Readiness**: `GET /materials/readiness` (Checks DB and Redis connectivity)
- **Metrics**: `GET /materials/metrics` (Prometheus format)

---

## 🧪 Testing

We prioritize reliable tests over mock-heavy unit tests.

```bash
# Run all tests using Testcontainers
dotnet test --verbosity normal
```

- **Integration Tests**: Use real PostgreSQL 18 containers.
- **Contract Tests**: Ensure API stability for consumers.

---

## 📦 Deployment

Infrastructure management is handled via GitOps patterns.

- **Docker Image**: `REGION-docker.pkg.dev/PROJECT_ID/REPOSITORY/maliev-material-service:{sha}`
- **Environments**: Development, Staging, Production

---

## 📄 License

Proprietary - © 2025 MALIEV Co., Ltd. All rights reserved.
