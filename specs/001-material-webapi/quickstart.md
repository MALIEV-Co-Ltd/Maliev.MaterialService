# Quickstart Guide: Material WebAPI Service

**Branch**: `001-material-webapi` | **Date**: 2024-11-18 | **Plan**: [link to plan.md]

This guide provides instructions to quickly set up and run the Material WebAPI Service for local development.

## Prerequisites

*   [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
*   [Docker Desktop](https://www.docker.com/products/docker-desktop) (for local PostgreSQL and Redis)
*   [Git](https://git-scm.com/downloads)

## 1. Clone the Repository

First, clone the repository to your local machine:

```bash
git clone <repository-url>
cd Maliev.MaterialService
```

## 2. Local Database and Cache Setup (Docker)

The service requires a PostgreSQL database and a Redis instance. You can easily set these up using Docker Compose.

1.  Navigate to the project root directory.
2.  Start the PostgreSQL and Redis containers:

    ```bash
    docker-compose -f docker-compose.dev.yml up -d
    ```

    *(Note: `docker-compose.dev.yml` will be created during implementation. This command assumes its existence.)*

3.  Verify that the containers are running:

    ```bash
    docker ps
    ```

## 3. Configure Environment Variables

The service relies on environment variables for configuration, including the database connection string and Redis connection string.

For local development, you'll need to set the following environment variables. You can set these in your shell or in a `.env` file if your IDE supports it.

**PowerShell Example**:

```powershell
$env:ConnectionStrings__MaterialDbContext="Host=localhost;Port=5432;Database=material_app_db;Username=postgres;Password=postgres;"
$env:Redis__ConnectionString="localhost:6379"
$env:Redis__Enabled="true"
$env:RabbitMQ__Host="localhost"
$env:RabbitMQ__Port="5672"
$env:RabbitMQ__Username="guest"
$env:RabbitMQ__Password="guest"
$env:RabbitMQ__Enabled="true"
$env:Jwt__PublicKey="<YOUR_BASE64_ENCODED_RSA_PUBLIC_KEY>" # Placeholder, replace with actual key
$env:Jwt__Issuer="https://localhost:7000" # Example for local dev
$env:Jwt__Audience="https://localhost:7000" # Example for local dev
$env:CORS_ALLOWED_ORIGINS="https://localhost:7000,http://localhost:5000" # Example for local dev
```

**Bash Example**:

```bash
export ConnectionStrings__MaterialDbContext="Host=localhost;Port=5432;Database=material_app_db;Username=postgres;Password=postgres;"
export Redis__ConnectionString="localhost:6379"
export Redis__Enabled="true"
export RabbitMQ__Host="localhost"
export RabbitMQ__Port="5672"
export RabbitMQ__Username="guest"
export RabbitMQ__Password="guest"
export RabbitMQ__Enabled="true"
export Jwt__PublicKey="<YOUR_BASE64_ENCODED_RSA_PUBLIC_KEY>" # Placeholder, replace with actual key
export Jwt__Issuer="https://localhost:7000" # Example for local dev
export Jwt__Audience="https://localhost:7000" # Example for local dev
export CORS_ALLOWED_ORIGINS="https://localhost:7000,http://localhost:5000" # Example for local dev
```

*Note: The `Jwt__PublicKey` should be a double Base64-encoded RSA public key. For local development, you might use a dummy key or generate one.*

## 4. Apply Database Migrations

Before running the service, apply the Entity Framework Core migrations to set up the database schema:

1.  Navigate to the `Maliev.MaterialService.Data` project directory:

    ```bash
    cd Maliev.MaterialService.Data
    ```

2.  Apply the migrations:

    ```bash
    dotnet ef database update
    ```

## 5. Run the Service

1.  Navigate to the `Maliev.MaterialService.Api` project directory:

    ```bash
    cd Maliev.MaterialService.Api
    ```

2.  Run the service:

    ```bash
    dotnet run
    ```

    The service will typically start on `http://localhost:5xxx` and `https://localhost:7xxx`.

## 6. Access API Documentation (Scalar UI)

Once the service is running, you can access the interactive API documentation (Scalar UI) in your browser:

*   Open your web browser and navigate to: `https://localhost:7xxx/materials/scalar/v1` (replace `7xxx` with the actual HTTPS port shown in the console output).

This UI allows you to explore the API endpoints, view schemas, and make test requests.

## 7. Stop Local Services

To stop the Docker containers:

```bash
docker-compose -f docker-compose.dev.yml down
```
