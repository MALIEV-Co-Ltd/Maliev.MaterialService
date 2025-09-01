# Maliev.MaterialService

This repository contains the `Maliev.MaterialService` project, migrated to .NET 9. This service provides API endpoints for managing material-related data.

## Key Features:

*   **Target Framework**: .NET 9.0
*   **Project Structure**: Multi-project solution with clear separation of concerns:
    *   `Maliev.MaterialService.Api`: ASP.NET Core Web API for handling API requests.
    *   `Maliev.MaterialService.Common`: .NET Class Library for common logic and enumerations.
    *   `Maliev.MaterialService.Data`: .NET Class Library for data access logic and models.
    *   `Maliev.MaterialService.Tests`: xUnit Test Project for unit and integration tests.
*   **API Design**: Uses Data Transfer Objects (DTOs) for clear API contracts, a service layer for business logic, and asynchronous operations.
*   **Configuration**: Externalized sensitive information using user secrets for local development and designed for Google Secret Manager integration in production.
*   **Health Checks**: Includes `/liveness` and `/readiness` endpoints for application health monitoring.
*   **Swagger/OpenAPI**: Configured for API documentation and testing in development environments.

## Getting Started (Local Development):

1.  **Clone the repository**.
2.  **Navigate to the project root** (`R:\maliev\Maliev.MaterialService`).
3.  **Restore NuGet packages**: `dotnet restore`
4.  **Set up User Secrets**: This project uses .NET User Secrets for sensitive configuration like JWT security keys and database connection strings. Execute the following commands from the project root:
    ```bash
    dotnet user-secrets set "Jwt:Issuer" "your_jwt_issuer_here" --project Maliev.MaterialService.Api
    dotnet user-secrets set "Jwt:Audience" "your_jwt_audience_here" --project Maliev.MaterialService.Api
    dotnet user-secrets set "JwtSecurityKey" "your_jwt_security_key_here_at_least_16_chars" --project Maliev.MaterialService.Api
    dotnet user-secrets set "ConnectionStrings:MaterialServiceDbContext" "Server=(localdb)\\mssqllocaldb;Database=MaterialServiceDb;Trusted_Connection=True;MultipleActiveResultSets=true" --project Maliev.MaterialService.Api
    ```
    **Note**: Replace `your_jwt_issuer_here`, `your_jwt_audience_here`, and `your_jwt_security_key_here_at_least_16_chars` with your actual values. The connection string provided is for a local SQL Server LocalDB instance.
5.  **Build the solution**: `dotnet build`
6.  **Run tests**: `dotnet test`
7.  **Run the API**: Navigate to the `Maliev.MaterialService.Api` directory and run `dotnet run`.

    The API will typically be available at `https://localhost:7000` (or a similar port). Swagger UI will be accessible at `/materialservice/swagger` (e.g., `https://localhost:7000/materialservice/swagger`).

## Deployment Considerations (Google Kubernetes Engine):

*   **Secrets Management**: Ensure `JwtSecurityKey` and `ConnectionStrings-MaterialServiceDbContext` secrets are configured in Google Secret Manager.
*   **`SecretProviderClass`**: Verify that the `maliev-shared-secrets` `SecretProviderClass` is correctly applied to your Kubernetes cluster to fetch secrets.

## Migration Details:

For a detailed overview of the migration process, key changes, and rationale, please refer to [GEMINI.md](GEMINI.md).

## Contributing:

Contributions are welcome. Please ensure your code adheres to the project's coding standards and all tests pass.
