# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY OrderManagement.sln .
COPY src/OrderService/OrderService.csproj src/OrderService/
COPY src/SharedKernel/SharedKernel.csproj src/SharedKernel/

# Restore dependencies
RUN dotnet restore src/OrderService/OrderService.csproj

# Copy source code
COPY src/OrderService/ src/OrderService/
COPY src/SharedKernel/ src/SharedKernel/

# Build the application
RUN dotnet build src/OrderService/OrderService.csproj -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish src/OrderService/OrderService.csproj -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Copy published app
COPY --from=publish /app/publish .

# Create non-root user for security
RUN adduser --disabled-password --gecos '' appuser && chown -R appuser /app
USER appuser

# Expose port
EXPOSE 80

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD curl -f http://localhost/health || exit 1

ENTRYPOINT ["dotnet", "OrderService.dll"]