# syntax=docker/dockerfile:1.7

ARG dependency_restore_stage=restore-private

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build-base
ARG BUILD_CONFIGURATION=Release
ARG SHARED_LIBRARY_VERSION=1.0.96-alpha
ARG SERVICE_DEFAULTS_VERSION=1.0.89-alpha
ENV GITHUB_ACTIONS=true
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["NuGet.PRValidation.Config", "."]
COPY ["Directory.Build.props", "."]
COPY ["Maliev.MaterialService.Api/Maliev.MaterialService.Api.csproj", "Maliev.MaterialService.Api/"]
COPY ["Maliev.MaterialService.Domain/Maliev.MaterialService.Domain.csproj", "Maliev.MaterialService.Domain/"]
COPY ["Maliev.MaterialService.Application/Maliev.MaterialService.Application.csproj", "Maliev.MaterialService.Application/"]
COPY ["Maliev.MaterialService.Infrastructure/Maliev.MaterialService.Infrastructure.csproj", "Maliev.MaterialService.Infrastructure/"]
COPY [".ci-packages/", ".ci-packages/"]

FROM build-base AS restore-private
RUN --mount=type=secret,id=nuget_username,required=true \
    --mount=type=secret,id=nuget_password,required=true \
    NUGET_USERNAME="$(cat /run/secrets/nuget_username)" \
    NUGET_PASSWORD="$(cat /run/secrets/nuget_password)" \
    dotnet restore "./Maliev.MaterialService.Api/Maliev.MaterialService.Api.csproj" \
      -p:SharedLibraryVersion=$SHARED_LIBRARY_VERSION \
      -p:ServiceDefaultsVersion=$SERVICE_DEFAULTS_VERSION

FROM build-base AS restore-local
RUN dotnet restore "./Maliev.MaterialService.Api/Maliev.MaterialService.Api.csproj" \
      --configfile "NuGet.PRValidation.Config" \
      -p:SharedLibraryVersion=$SHARED_LIBRARY_VERSION \
      -p:ServiceDefaultsVersion=$SERVICE_DEFAULTS_VERSION

FROM ${dependency_restore_stage} AS build
ARG BUILD_CONFIGURATION=Release
ARG SHARED_LIBRARY_VERSION=1.0.96-alpha
ARG SERVICE_DEFAULTS_VERSION=1.0.89-alpha
COPY . .
WORKDIR /src/Maliev.MaterialService.Api
RUN dotnet publish "./Maliev.MaterialService.Api.csproj" \
      -c $BUILD_CONFIGURATION --no-restore -o /app/publish /p:UseAppHost=false \
      -p:SharedLibraryVersion=$SHARED_LIBRARY_VERSION \
      -p:ServiceDefaultsVersion=$SERVICE_DEFAULTS_VERSION

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --chown=app:app --from=build /app/publish .
USER app
EXPOSE 8080
ENV ASPNETCORE_HTTP_PORTS=8080 \
    ASPNETCORE_ENVIRONMENT=Production \
    DOTNET_EnableDiagnostics=0
ENTRYPOINT ["dotnet", "Maliev.MaterialService.Api.dll"]
