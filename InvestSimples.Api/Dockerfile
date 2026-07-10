# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
ARG VERSION=1.0.0
WORKDIR /src

# Copy csproj and restore dependencies (cache layer)
COPY ["InvestSimples.Api/InvestSimples.Api.csproj", "InvestSimples.Api/"]
RUN dotnet restore "InvestSimples.Api/InvestSimples.Api.csproj"

# Copy source and build
COPY . .
WORKDIR /src/InvestSimples.Api
RUN dotnet publish -c Release -o /app/publish --no-restore \
    -p:Version=${VERSION} \
    -p:InformationalVersion=${VERSION}

# Security scan stage
FROM aquasec/trivy:latest AS security-scan
COPY --from=build /app/publish /app
RUN trivy fs --no-progress --severity HIGH,CRITICAL /app

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS final
WORKDIR /app

# Install curl for health checks
RUN apk add --no-cache curl

# Create non-root user
RUN adduser -D -u 1000 appuser && chown -R appuser:appuser /app
USER appuser

# Copy published app
COPY --from=build --chown=appuser:appuser /app/publish .

# Expose port
EXPOSE 8080

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=10s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

# Entry point
ENTRYPOINT ["dotnet", "InvestSimples.Api.dll"]