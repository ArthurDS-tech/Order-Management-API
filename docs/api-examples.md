# Exemplos de Uso da API

Aqui estão alguns exemplos práticos de como usar a Order Management API.

## 🚀 Endpoints Principais

### 1. Criar um Pedido

```bash
POST http://localhost:5000/api/orders
Content-Type: application/json

{
  "customerEmail": "joao@example.com",
  "customerName": "João Silva",
  "shippingAddress": {
    "street": "Rua das Flores",
    "number": "123",
    "complement": "Apto 45",
    "neighborhood": "Centro",
    "city": "São Paulo",
    "state": "SP",
    "zipCode": "01234-567",
    "country": "Brasil"
  },
  "items": [
    {
      "productId": "550e8400-e29b-41d4-a716-446655440000",
      "productName": "Smartphone XYZ",
      "productSku": "PHONE-XYZ-001",
      "price": 899.90,
      "quantity": 1
    },
    {
      "productId": "550e8400-e29b-41d4-a716-446655440001",
      "productName": "Capinha Protetora",
      "productSku": "CASE-XYZ-001",
      "price": 29.90,
      "quantity": 2
    }
  ]
}
```

**Resposta:**
```json
"7d4c4c4c-1234-5678-9abc-def012345678"
```

### 2. Buscar um Pedido

```bash
GET http://localhost:5000/api/orders/7d4c4c4c-1234-5678-9abc-def012345678
```

**Resposta:**
```json
{
  "id": "7d4c4c4c-1234-5678-9abc-def012345678",
  "customerEmail": "joao@example.com",
  "customerName": "João Silva",
  "status": "Aguardando Pagamento",
  "totalAmount": 959.70,
  "currency": "BRL",
  "createdAt": "2024-01-15T10:30:00Z",
  "shippingAddress": {
    "street": "Rua das Flores",
    "number": "123",
    "complement": "Apto 45",
    "neighborhood": "Centro",
    "city": "São Paulo",
    "state": "SP",
    "zipCode": "01234567",
    "country": "Brasil"
  },
  "items": [
    {
      "productId": "550e8400-e29b-41d4-a716-446655440000",
      "productName": "Smartphone XYZ",
      "productSku": "PHONE-XYZ-001",
      "price": 899.90,
      "quantity": 1,
      "subtotal": 899.90
    },
    {
      "productId": "550e8400-e29b-41d4-a716-446655440001",
      "productName": "Capinha Protetora",
      "productSku": "CASE-XYZ-001",
      "price": 29.90,
      "quantity": 2,
      "subtotal": 59.80
    }
  ]
}
```

### 3. Listar Pedidos com Filtros

```bash
GET http://localhost:5000/api/orders?page=1&pageSize=10&customerEmail=joao@example.com&status=Paid&startDate=2024-01-01&sortBy=CreatedAt&sortDescending=true
```

### 4. Atualizar Status do Pedido

```bash
PATCH http://localhost:5000/api/orders/7d4c4c4c-1234-5678-9abc-def012345678/status
Content-Type: application/json

{
  "status": 2,
  "paymentId": "stripe_payment_123456"
}
```

## 💳 Payment Service

### Processar Pagamento

```bash
POST http://localhost:5000/api/payments
Content-Type: application/json

{
  "orderId": "7d4c4c4c-1234-5678-9abc-def012345678",
  "customerEmail": "joao@example.com",
  "amount": 959.70,
  "paymentMethod": "Credit Card"
}
```

## 📦 Inventory Service

### Verificar Estoque

```bash
GET http://localhost:5000/api/inventory/550e8400-e29b-41d4-a716-446655440000
```

### Reservar Estoque

```bash
POST http://localhost:5000/api/inventory/550e8400-e29b-41d4-a716-446655440000/reserve
Content-Type: application/json

{
  "quantity": 1,
  "orderId": "7d4c4c4c-1234-5678-9abc-def012345678"
}
```

## 📬 Notification Service

### Enviar Email

```bash
POST http://localhost:5000/api/notifications/email
Content-Type: application/json

{
  "to": "joao@example.com",
  "subject": "Pedido Confirmado!",
  "body": "<h1>Seu pedido foi confirmado</h1><p>Obrigado pela compra!</p>",
  "isHtml": true
}
```

### Enviar SMS

```bash
POST http://localhost:5000/api/notifications/sms
Content-Type: application/json

{
  "phoneNumber": "+5511999999999",
  "message": "Seu pedido #123 foi enviado! Código de rastreamento: BR123456789"
}
```

## 🔍 Health Checks

Todos os serviços têm endpoints de health check:

```bash
GET http://localhost:5000/health           # Gateway
GET http://localhost:5000/api/orders/health
GET http://localhost:5000/api/payments/health
GET http://localhost:5000/api/inventory/health
GET http://localhost:5000/api/notifications/health
```

## 📊 Status dos Pedidos

- `1` - Pending (Aguardando Pagamento)
- `2` - Paid (Pago)
- `3` - Shipped (Enviado)
- `4` - Delivered (Entregue)
- `5` - Cancelled (Cancelado)

## 🚨 Códigos de Erro Comuns

- `400` - Bad Request (dados inválidos)
- `404` - Not Found (pedido não encontrado)
- `429` - Too Many Requests (rate limit atingido)
- `500` - Internal Server Error (erro interno)

## 💡 Dicas

1. **Rate Limiting**: O Gateway limita a 100 requests por minuto por IP
2. **Paginação**: Use `page` e `pageSize` nas listagens (máximo 100 itens por página)
3. **Filtros**: Combine múltiplos filtros nas queries pra resultados mais precisos
4. **Logs**: Todos os serviços logam em JSON estruturado - fácil de analisar

---

*Feito com ☕ e muito carinho pra facilitar sua vida de dev*