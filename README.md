# FCG.Lib.Shared

**Tech Challenge - Fase 2**  
Biblioteca compartilhada entre todos os microsserviços da plataforma FCG.

## Propósito

Esta biblioteca contém código comum reutilizado por todas as APIs, eliminando duplicação e garantindo consistência.

## O que contém

### Application
- **Behaviors** - Pipeline behaviors do MediatR (validação, logging)
- **Controllers** - Controller base com tratamento de erros
- **Extensions** - Métodos de extensão para configuração

### Domain
- **Entities** - Classe base para entidades
- **Enumerations** - Enums comuns

### Infrastructure
- **Data** - Configuração do EF Core e SQL Server
- **DependencyInjection** - Extensões para registro de serviços
- **Middlewares** - Middlewares compartilhados (CORS, Auth, Swagger)

### Messaging
- **Configuration** - Configuração do MassTransit + RabbitMQ
- **Contracts** - Eventos compartilhados:
  - `UserCreatedEvent`
  - `OrderPlacedEvent`
  - `PaymentProcessedEvent`

## Como usar

Esta biblioteca é publicada no **GitHub Packages** e referenciada nos `.csproj` de cada API:

```xml
<PackageReference Include="FCG.Lib.Shared" Version="1.0.8" />
```

**Grupo 11 - FIAP 2026**
