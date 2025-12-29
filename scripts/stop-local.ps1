# Script pra parar todos os serviços
Write-Host "🛑 Stopping Order Management System..." -ForegroundColor Yellow

docker-compose down

Write-Host "✅ All services stopped!" -ForegroundColor Green