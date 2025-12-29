# Script pra rodar os serviços localmente - útil pra desenvolvimento
Write-Host "🚀 Starting Order Management System locally..." -ForegroundColor Green

# Verifica se o Docker está rodando
$dockerRunning = docker info 2>$null
if (-not $dockerRunning) {
    Write-Host "❌ Docker is not running. Please start Docker first." -ForegroundColor Red
    exit 1
}

# Sobe a infraestrutura (bancos, Redis, RabbitMQ)
Write-Host "Starting infrastructure services..." -ForegroundColor Yellow
docker-compose up -d postgres redis rabbitmq

# Espera um pouco pros serviços subirem
Write-Host "Waiting for infrastructure to be ready..." -ForegroundColor Yellow
Start-Sleep -Seconds 10

# Sobe os serviços da aplicação
Write-Host "Starting application services..." -ForegroundColor Yellow
docker-compose up -d

Write-Host "✅ All services are starting up!" -ForegroundColor Green
Write-Host ""
Write-Host "🌐 Available endpoints:" -ForegroundColor Cyan
Write-Host "  - API Gateway: http://localhost:5000" -ForegroundColor White
Write-Host "  - Order Service: http://localhost:5001" -ForegroundColor White
Write-Host "  - Payment Service: http://localhost:5002" -ForegroundColor White
Write-Host "  - Inventory Service: http://localhost:5003" -ForegroundColor White
Write-Host "  - Notification Service: http://localhost:5004" -ForegroundColor White
Write-Host ""
Write-Host "📊 Management UIs:" -ForegroundColor Cyan
Write-Host "  - RabbitMQ: http://localhost:15672 (dev/dev123)" -ForegroundColor White
Write-Host ""
Write-Host "Use 'docker-compose logs -f [service-name]' to see logs" -ForegroundColor Gray