# Script pra buildar tudo de uma vez - útil pra CI/CD
Write-Host "🔨 Building Order Management System..." -ForegroundColor Green

# Build da solution
Write-Host "Building solution..." -ForegroundColor Yellow
dotnet build OrderManagement.sln -c Release

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Build failed!" -ForegroundColor Red
    exit 1
}

# Roda os testes
Write-Host "Running tests..." -ForegroundColor Yellow
dotnet test --no-build -c Release --verbosity minimal

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Tests failed!" -ForegroundColor Red
    exit 1
}

Write-Host "✅ Build and tests completed successfully!" -ForegroundColor Green