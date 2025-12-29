# Script pra testar a API - útil pra verificar se tudo tá funcionando
Write-Host "🧪 Testing Order Management API..." -ForegroundColor Green

$baseUrl = "http://localhost:5000"

# Testa health checks
Write-Host "Testing health checks..." -ForegroundColor Yellow

$services = @("api/gateway/health", "api/orders/health", "api/payments/health", "api/inventory/health", "api/notifications/health")

foreach ($service in $services) {
    try {
        $response = Invoke-RestMethod -Uri "$baseUrl/$service" -Method Get -TimeoutSec 5
        Write-Host "✅ $service - OK" -ForegroundColor Green
    }
    catch {
        Write-Host "❌ $service - Failed: $($_.Exception.Message)" -ForegroundColor Red
    }
}

# Testa criar um pedido
Write-Host "`nTesting order creation..." -ForegroundColor Yellow

$orderData = @{
    customerEmail = "test@example.com"
    customerName = "João Silva"
    shippingAddress = @{
        street = "Rua das Flores"
        number = "123"
        neighborhood = "Centro"
        city = "São Paulo"
        state = "SP"
        zipCode = "01234567"
        country = "Brasil"
    }
    items = @(
        @{
            productId = [System.Guid]::NewGuid()
            productName = "Produto Teste"
            productSku = "PROD-001"
            price = 99.90
            quantity = 2
        }
    )
} | ConvertTo-Json -Depth 3

try {
    $response = Invoke-RestMethod -Uri "$baseUrl/api/orders" -Method Post -Body $orderData -ContentType "application/json" -TimeoutSec 10
    Write-Host "✅ Order created successfully! ID: $response" -ForegroundColor Green
    
    # Testa buscar o pedido criado
    Write-Host "Testing order retrieval..." -ForegroundColor Yellow
    $orderResponse = Invoke-RestMethod -Uri "$baseUrl/api/orders/$response" -Method Get -TimeoutSec 5
    Write-Host "✅ Order retrieved successfully! Status: $($orderResponse.status)" -ForegroundColor Green
}
catch {
    Write-Host "❌ Order creation failed: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host "`n🎉 API testing completed!" -ForegroundColor Green