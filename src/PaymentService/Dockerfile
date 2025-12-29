# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY OrderManagement.sln .
COPY src/PaymentService/PaymentService.csproj src/PaymentService/
COPY src/SharedKernel/SharedKernel.csproj src/SharedKernel/

RUN dotnet restore src/PaymentService/PaymentService.csproj

COPY src/PaymentService/ src/PaymentService/
COPY src/SharedKernel/ src/SharedKernel/

RUN dotnet build src/PaymentService/PaymentService.csproj -c Release -o /app/build

FROM build AS publish
RUN dotnet publish src/PaymentService/PaymentService.csproj -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

COPY --from=publish /app/publish .

RUN adduser --disabled-password --gecos '' appuser && chown -R appuser /app
USER appuser

EXPOSE 80

HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD curl -f http://localhost/health || exit 1

ENTRYPOINT ["dotnet", "PaymentService.dll"]