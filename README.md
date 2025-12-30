<div align="center">
  <img src="[https://readme-typing-svg.demolab.com?font=Fira+Code&weight=600&size=28&pause=1000&color=00D4FF&center=true&vCenter=true&random=false&width=600&lines=🛒+Order+Management+API;🚀+Microsserviços+em+.NET+8;💳+E-commerce+Escalável](https://cdn3d.iconscout.com/3d/premium/thumb/tecla-a-3d-icon-png-download-9741287.png" alt="Typing SVG" />
</div>

<div align="center">
  <img src="https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" />
  <img src="https://img.shields.io/badge/Docker-2496ED?style=for-the-badge&logo=docker&logoColor=white" />
  <img src="https://img.shields.io/badge/PostgreSQL-336791?style=for-the-badge&logo=postgresql&logoColor=white" />
  <img src="https://img.shields.io/badge/Redis-DC382D?style=for-the-badge&logo=redis&logoColor=white" />
  <img src="https://img.shields.io/badge/RabbitMQ-FF6600?style=for-the-badge&logo=rabbitmq&logoColor=white" />
</div>

<br />

<p align="center">
  <strong>Sistema completo de gerenciamento de pedidos construído com .NET 8</strong><br>
  Arquitetura de microsserviços escalável capaz de processar milhares de pedidos por minuto
</p>

## 🚀 Como rodar

<img align="right" alt="Docker" height="120px" src="https://raw.githubusercontent.com/devicons/devicon/master/icons/docker/docker-original.svg" />

### 🐳 Opção 1: Docker (Recomendado)

```bash
# Clone o repositório
git clone <repository-url>
cd OrderManagement

# Sobe tudo de uma vez - bancos, Redis, RabbitMQ e os serviços
docker-compose up -d

# Ou use o script PowerShell (Windows)
.\scripts\run-local.ps1

# Pra parar tudo
docker-compose down
# ou
.\scripts\stop-local.ps1
```

### 💻 Opção 2: Local (pra debug)

```bash
# 1. Suba a infraestrutura primeiro
docker-compose up -d postgres redis rabbitmq

# 2. Rode cada serviço em terminais separados
dotnet run --project src/ApiGateway
dotnet run --project src/OrderService
dotnet run --project src/PaymentService
dotnet run --project src/InventoryService
dotnet run --project src/NotificationService
```

### 🛠️ Opção 3: Scripts Úteis

```bash
# Build e testes
.\scripts\build.ps1

# Testa se a API tá funcionando
.\scripts\test-api.ps1
```

## 📋 Endpoints principais

<div align="center">

| Serviço | Endpoint | Descrição |
|---------|----------|-----------|
| 🚪 **Gateway** | `http://localhost:5000` | Ponto de entrada único |
| 📦 **Orders** | `http://localhost:5001` | Gerenciamento de pedidos |
| 💳 **Payments** | `http://localhost:5002` | Processamento de pagamentos |
| 📋 **Inventory** | `http://localhost:5003` | Controle de estoque |
| 📬 **Notifications** | `http://localhost:5004` | Envio de notificações |

</div>

### 🔍 Swagger UI
Cada serviço tem sua documentação interativa:
- **Gateway**: http://localhost:5000/swagger
- **Orders**: http://localhost:5001/swagger  
- **Payments**: http://localhost:5002/swagger
- **Inventory**: http://localhost:5003/swagger
- **Notifications**: http://localhost:5004/swagger

### 📊 Monitoramento
- **RabbitMQ Management**: http://localhost:15672 (dev/dev123)
- **Health Checks**: Disponível em `/health` em cada serviço

## 🏗️ Arquitetura do Sistema

<div align="center">

```mermaid
graph TB
    subgraph "Frontend"
        WEB[🌐 Web App]
        MOBILE[📱 Mobile App]
    end
    
    subgraph "API Gateway"
        GATEWAY[🚪 YARP Gateway<br/>Rate Limiting<br/>Load Balancing]
    end
    
    subgraph "Microsserviços"
        ORDER[📦 Order Service<br/>CRUD Pedidos<br/>Workflow Status]
        PAYMENT[💳 Payment Service<br/>Processamento<br/>Stripe Mock]
        INVENTORY[📋 Inventory Service<br/>Controle Estoque<br/>Reservas]
        NOTIFICATION[📬 Notification Service<br/>Email & SMS<br/>SendGrid Mock]
    end
    
    subgraph "Infraestrutura"
        POSTGRES[(🐘 PostgreSQL<br/>Database per Service)]
        REDIS[(⚡ Redis<br/>Cache Distribuído)]
        RABBIT[🐰 RabbitMQ<br/>Message Broker]
    end
    
    WEB --> GATEWAY
    MOBILE --> GATEWAY
    
    GATEWAY --> ORDER
    GATEWAY --> PAYMENT
    GATEWAY --> INVENTORY
    GATEWAY --> NOTIFICATION
    
    ORDER --> POSTGRES
    PAYMENT --> POSTGRES
    INVENTORY --> POSTGRES
    
    ORDER --> REDIS
    PAYMENT --> REDIS
    INVENTORY --> REDIS
    
    ORDER --> RABBIT
    PAYMENT --> RABBIT
    INVENTORY --> RABBIT
    NOTIFICATION --> RABBIT
    
    style GATEWAY fill:#00D4FF,stroke:#0099CC,color:#fff
    style ORDER fill:#4CAF50,stroke:#45a049,color:#fff
    style PAYMENT fill:#FF9800,stroke:#F57C00,color:#fff
    style INVENTORY fill:#9C27B0,stroke:#7B1FA2,color:#fff
    style NOTIFICATION fill:#F44336,stroke:#D32F2F,color:#fff
```

</div>

## 🔄 Fluxo de Pedido

<div align="center">

```mermaid
sequenceDiagram
    participant C as 👤 Cliente
    participant G as 🚪 Gateway
    participant O as 📦 Order Service
    participant I as 📋 Inventory
    participant P as 💳 Payment
    participant N as 📬 Notification
    
    C->>G: 1. Criar Pedido
    G->>O: Validar & Criar
    O->>I: Reservar Estoque
    I-->>O: ✅ Estoque OK
    O->>N: Pedido Criado
    N->>C: 📧 Email Confirmação
    
    O->>P: Processar Pagamento
    P-->>O: ✅ Pagamento OK
    O->>O: Status → Pago
    O->>N: Pagamento Confirmado
    N->>C: 📧 Pagamento Aprovado
    
    O->>O: Status → Enviado
    O->>N: Pedido Enviado
    N->>C: 📱 SMS Rastreamento
    
    O->>O: Status → Entregue
    O->>N: Pedido Entregue
    N->>C: 📧 Avaliação
```

</div>

## 🧪 Testes

<img align="right" alt="Testing" height="100px" src="https://raw.githubusercontent.com/devicons/devicon/master/icons/csharp/csharp-original.svg" />

```bash
# Roda todos os testes
dotnet test

# Com coverage
dotnet test --collect:"XPlat Code Coverage"

# Só os testes de unidade
dotnet test --filter Category=Unit

# Testa a API funcionando
.\scripts\test-api.ps1
```

## 📚 Documentação

- **[📖 Exemplos de API](docs/api-examples.md)** - Como usar todos os endpoints
- **🔧 Swagger UI** - Documentação interativa em cada serviço
- **📊 Postman Collection** - Coleção completa para testes

## 📦 Stack Técnica

<div align="center">

| Categoria | Tecnologias |
|-----------|-------------|
| **Backend** | ![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat-square&logo=dotnet) ![C#](https://img.shields.io/badge/C%23-239120?style=flat-square&logo=c-sharp) |
| **Database** | ![PostgreSQL](https://img.shields.io/badge/PostgreSQL-336791?style=flat-square&logo=postgresql) ![Redis](https://img.shields.io/badge/Redis-DC382D?style=flat-square&logo=redis) |
| **Messaging** | ![RabbitMQ](https://img.shields.io/badge/RabbitMQ-FF6600?style=flat-square&logo=rabbitmq) |
| **Container** | ![Docker](https://img.shields.io/badge/Docker-2496ED?style=flat-square&logo=docker) |
| **Testing** | ![xUnit](https://img.shields.io/badge/xUnit-512BD4?style=flat-square) ![Moq](https://img.shields.io/badge/Moq-239120?style=flat-square) |
| **Patterns** | CQRS, DDD, Repository, Event Sourcing |

</div>

## 🎯 Features Principais

<div align="center">

| Feature | Descrição | Status |
|---------|-----------|--------|
| 📦 **CRUD Pedidos** | Criação, leitura, atualização e exclusão de pedidos | ✅ |
| 🔄 **Workflow Status** | Transições automáticas de status (Pendente → Pago → Enviado → Entregue) | ✅ |
| ⚡ **Cache Distribuído** | Redis para consultas rápidas e invalidação inteligente | ✅ |
| 🛡️ **Rate Limiting** | Proteção contra spam (100 req/min por IP) | ✅ |
| 📊 **Logs Estruturados** | Serilog com formato JSON para análise | ✅ |
| 🔍 **Health Checks** | Monitoramento de saúde de todos os serviços | ✅ |
| 📧 **Notificações** | Email e SMS automáticos para eventos do pedido | ✅ |
| 💳 **Pagamentos** | Integração mock com gateways (Stripe ready) | ✅ |
| 📋 **Controle Estoque** | Reserva e liberação automática de produtos | ✅ |
| 🧪 **Testes** | Cobertura de testes unitários e integração | ✅ |

</div>

## 🏛️ Padrões Arquiteturais

<div align="center">

```mermaid
graph LR
    subgraph "Clean Architecture"
        DOMAIN[🎯 Domain<br/>Entities<br/>Value Objects<br/>Events]
        APP[⚙️ Application<br/>Commands<br/>Queries<br/>Handlers]
        INFRA[🔧 Infrastructure<br/>Repositories<br/>EF Core<br/>External APIs]
        PRES[🌐 Presentation<br/>Controllers<br/>DTOs<br/>Validators]
    end
    
    PRES --> APP
    APP --> DOMAIN
    INFRA --> DOMAIN
    
    style DOMAIN fill:#4CAF50,stroke:#45a049,color:#fff
    style APP fill:#2196F3,stroke:#1976D2,color:#fff
    style INFRA fill:#FF9800,stroke:#F57C00,color:#fff
    style PRES fill:#9C27B0,stroke:#7B1FA2,color:#fff
```

</div>

### 🎨 Design Patterns Utilizados

- **🎯 CQRS** - Separação de comandos e consultas com MediatR
- **📦 Repository** - Abstração da camada de dados
- **🏭 Factory** - Criação controlada de entidades
- **📢 Domain Events** - Comunicação entre agregados
- **🔄 Event Sourcing** - Histórico imutável de mudanças
- **🛡️ Result Pattern** - Tratamento de erros sem exceptions

## 🚀 Roadmap & Melhorias

<div align="center">

| Fase | Feature | Prioridade |
|------|---------|------------|
| 🎯 **Fase 1** | Autenticação JWT & Autorização | 🔴 Alta |
| 🎯 **Fase 2** | Tracing Distribuído (Jaeger) | 🟡 Média |
| 🎯 **Fase 3** | GraphQL Gateway (HotChocolate) | 🟡 Média |
| 🎯 **Fase 4** | Kubernetes Deployment | 🟢 Baixa |
| 🎯 **Fase 5** | Métricas Prometheus/Grafana | 🟢 Baixa |

</div>

## 🤝 Como Contribuir

<img align="right" alt="Contribution" height="120px" src="https://raw.githubusercontent.com/devicons/devicon/master/icons/git/git-original.svg" />

1. **Fork** o projeto
2. **Clone** seu fork: `git clone <your-fork-url>`
3. **Crie** uma branch: `git checkout -b feature/nova-feature`
4. **Commit** suas mudanças: `git commit -m 'Add: nova feature'`
5. **Push** para a branch: `git push origin feature/nova-feature`
6. **Abra** um Pull Request

### 📋 Checklist para PRs

- [ ] Código segue os padrões do projeto
- [ ] Testes unitários adicionados/atualizados
- [ ] Documentação atualizada
- [ ] Build passa sem erros
- [ ] Cobertura de testes mantida

## 📊 Estatísticas do Projeto

<div align="center">

![GitHub repo size](https://img.shields.io/github/repo-size/username/OrderManagement?style=for-the-badge&color=00D4FF)
![GitHub language count](https://img.shields.io/github/languages/count/username/OrderManagement?style=for-the-badge&color=00D4FF)
![GitHub top language](https://img.shields.io/github/languages/top/username/OrderManagement?style=for-the-badge&color=00D4FF)
![GitHub last commit](https://img.shields.io/github/last-commit/username/OrderManagement?style=for-the-badge&color=00D4FF)

</div>

## 📞 Contato & Suporte

<div align="center">

[![Email](https://img.shields.io/badge/-Email-000?style=for-the-badge&logo=microsoft-outlook&logoColor=00D4FF&color:FFF)](mailto:seu-email@exemplo.com)
[![LinkedIn](https://img.shields.io/badge/-LinkedIn-000?style=for-the-badge&logo=linkedin&logoColor=00D4FF&color:FFF)](https://www.linkedin.com/in/seu-perfil/)
[![GitHub](https://img.shields.io/badge/-GitHub-000?style=for-the-badge&logo=github&logoColor=00D4FF&color:FFF)](https://github.com/seu-usuario)

</div>

## 📄 Licença

Este projeto está sob a licença MIT. Veja o arquivo [LICENSE](LICENSE) para mais detalhes.

---

<div align="center">

**⭐ Se este projeto te ajudou, deixe uma estrela! ⭐**

<br>

*Feito com ☕ e muito carinho para não virar mais um boilerplate sem alma*

<br>

![Visitor Count](https://profile-counter.glitch.me/OrderManagement/count.svg)

</div>
