# syntax=docker/dockerfile:1.4
# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
ARG SHARED_LIBRARY_VERSION=1.0.96-alpha
ARG SERVICE_DEFAULTS_VERSION=1.0.89-alpha
WORKDIR /src
COPY ["Maliev.MaterialService.Api/Maliev.MaterialService.Api.csproj", "Maliev.MaterialService.Api/"]
COPY ["Maliev.MaterialService.Domain/Maliev.MaterialService.Domain.csproj", "Maliev.MaterialService.Domain/"]
COPY ["Maliev.MaterialService.Application/Maliev.MaterialService.Application.csproj", "Maliev.MaterialService.Application/"]
COPY ["Maliev.MaterialService.Infrastructure/Maliev.MaterialService.Infrastructure.csproj", "Maliev.MaterialService.Infrastructure/"]
COPY ["Directory.Build.props", "."]
COPY ["nuget.config", "."]
RUN --mount=type=secret,id=nuget_username \
  --mount=type=secret,id=nuget_password \
  GITHUB_ACTIONS=true \
  NUGET_USERNAME=$(cat /run/secrets/nuget_username) \
  NUGET_PASSWORD=$(cat /run/secrets/nuget_password) \
  dotnet restore "./Maliev.MaterialService.Api/Maliev.MaterialService.Api.csproj" \
    -p:SharedLibraryVersion=$SHARED_LIBRARY_VERSION \
    -p:ServiceDefaultsVersion=$SERVICE_DEFAULTS_VERSION
COPY . .
WORKDIR "/src/Maliev.MaterialService.Api"
RUN GITHUB_ACTIONS=true dotnet build "./Maliev.MaterialService.Api.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/build \
    --no-restore \
    -p:SharedLibraryVersion=$SHARED_LIBRARY_VERSION \
    -p:ServiceDefaultsVersion=$SERVICE_DEFAULTS_VERSION

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
ARG SHARED_LIBRARY_VERSION=1.0.96-alpha
ARG SERVICE_DEFAULTS_VERSION=1.0.89-alpha
RUN GITHUB_ACTIONS=true dotnet publish "./Maliev.MaterialService.Api.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    --no-restore \
    /p:UseAppHost=false \
    -p:SharedLibraryVersion=$SHARED_LIBRARY_VERSION \
    -p:ServiceDefaultsVersion=$SERVICE_DEFAULTS_VERSION

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app

# Ensure 'app' owns the workdir (app user already exists in ASP.NET runtime image)
RUN chown -R app:app /app

# Switch to non-root user
USER app

# Copy published application (now owned by app)
COPY --chown=app:app --from=publish /app/publish .

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "Maliev.MaterialService.Api.dll"]
