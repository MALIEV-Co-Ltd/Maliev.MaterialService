FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER 10001
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Maliev.MaterialService.Api/Maliev.MaterialService.Api.csproj", "Maliev.MaterialService.Api/"]
COPY ["Maliev.MaterialService.Data/Maliev.MaterialService.Data.csproj", "Maliev.MaterialService.Data/"]
RUN dotnet restore "./Maliev.MaterialService.Api/Maliev.MaterialService.Api.csproj"
COPY . .
WORKDIR "/src/Maliev.MaterialService.Api"
RUN dotnet build "./Maliev.MaterialService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Maliev.MaterialService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
RUN mkdir -p /app/logs && chown 10001:10001 /app/logs
USER 10001
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 CMD curl -f http://localhost:8080/materials/liveness || exit 1
ENTRYPOINT ["dotnet", "Maliev.MaterialService.Api.dll"]