<div align="center">

# 🎫 Helpdesk API

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?style=for-the-badge&logo=dotnet)](https://dotnet.microsoft.com/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-15-336791?style=for-the-badge&logo=postgresql&logoColor=white)](https://www.postgresql.org/)
[![Docker](https://img.shields.io/badge/Docker-Compose-2496ED?style=for-the-badge&logo=docker&logoColor=white)](https://www.docker.com/)
[![JWT](https://img.shields.io/badge/Auth-JWT-000000?style=for-the-badge&logo=jsonwebtokens)](https://jwt.io/)
[![Swagger](https://img.shields.io/badge/Docs-Swagger-85EA2D?style=for-the-badge&logo=swagger&logoColor=black)](https://swagger.io/)
[![License](https://img.shields.io/badge/License-MIT-green?style=for-the-badge)](LICENSE)

**EN** | A RESTful Helpdesk API built with .NET 10, JWT authentication, role-based access control, and PostgreSQL.

**PT** | API RESTful de Helpdesk construída com .NET 10, autenticação JWT, controle de acesso por roles e PostgreSQL.

</div>

---

## 📋 Table of Contents / Índice

- [Architecture / Arquitetura](#architecture--arquitetura)
- [Features / Funcionalidades](#features--funcionalidades)
- [Tech Stack](#tech-stack)
- [Getting Started / Primeiros Passos](#getting-started--primeiros-passos)
- [API Endpoints & Examples / Exemplos de Requisições](#api-endpoints--examples--exemplos-de-requisições)
- [Roles & Permissions / Roles e Permissões](#roles--permissions--roles-e-permissões)
- [Project Structure / Estrutura do Projeto](#project-structure--estrutura-do-projeto)

---

## Architecture / Arquitetura

```
┌─────────────────────────────────────────────────────────┐
│                        Client                           │
│                  (Swagger / HTTP Client)                │
└────────────────────────┬────────────────────────────────┘
                         │ HTTP
┌────────────────────────▼────────────────────────────────┐
│                    Helpdesk.API                         │
│                                                         │
│  ┌─────────────────────────────────────────────────┐    │
│  │              Middleware Pipeline                │    │
│  │  CorrelationId → ExceptionHandler → Auth → ...  │    │
│  └─────────────────────────────────────────────────┘    │
│                                                         │
│  ┌──────────────────┐   ┌──────────────────────────┐    │
│  │  AuthController  │   │    TicketController      │    │
│  └────────┬─────────┘   └────────────┬─────────────┘    │
└───────────┼──────────────────────────┼──────────────────┘
            │                          │
┌───────────▼──────────────────────────▼─────────────────┐
│                  Helpdesk.Application                  │
│                                                        │
│       ┌─────────────┐      ┌──────────────┐            │
│       │ AuthService │      │TicketService │            │
│       └─────────────┘      └──────────────┘            │
│       Interfaces: IUserRepository, ITicketRepository   │
│                   IPasswordHasher, IJwtTokenGenerator  │
└───────────────────────────┬────────────────────────────┘
                            │
┌───────────────────────────▼─────────────────────────────┐
│                 Helpdesk.Infrastructure                 │
│                                                         │
│  ┌────────────────┐  ┌────────────────┐                 │
│  │ UserRepository │  │TicketRepository│                 │
│  └────────────────┘  └────────────────┘                 │
│  ┌─────────────────┐  ┌──────────────────┐              │
│  │ PasswordHasher  │  │ JwtTokenGenerator│              │
│  └─────────────────┘  └──────────────────┘              │
│                    AppDbContext (EF Core)               │
└───────────────────────────┬─────────────────────────────┘
                            │
┌───────────────────────────▼─────────────────────────────┐
│                       PostgreSQL                        │
│              Users | Tickets | Migrations               │
└─────────────────────────────────────────────────────────┘
```

**EN:** Clean Architecture with 4 layers — API, Application, Infrastructure, and Domain — keeping business logic fully decoupled from frameworks and infrastructure.

**PT:** Clean Architecture com 4 camadas — API, Application, Infrastructure e Domain — mantendo a lógica de negócio completamente desacoplada de frameworks e infraestrutura.

---

## Features / Funcionalidades

| Feature | EN | PT |
|---|---|---|
| 🔐 | JWT Authentication | Autenticação JWT |
| 👥 | Role-based access (Admin, Editor, Viewer) | Controle de acesso por roles |
| 🎫 | Full ticket lifecycle (create, update, close, cancel, delete) | Ciclo de vida completo do ticket |
| 📋 | User ticket listing | Listagem de tickets do usuário |
| 🔁 | Correlation ID middleware | Middleware de Correlation ID |
| 🛡️ | Global exception handling | Tratamento global de exceções |
| 📝 | Structured logging with Serilog | Logs estruturados com Serilog |
| 🐳 | Docker Compose for PostgreSQL | Docker Compose para PostgreSQL |
| 📖 | Swagger / OpenAPI docs | Documentação Swagger / OpenAPI |
| 🔒 | BCrypt password hashing | Hash de senha com BCrypt |

---

## Tech Stack

- **Runtime:** .NET 10
- **Database:** PostgreSQL 15 + Entity Framework Core
- **Auth:** JWT Bearer Tokens + BCrypt
- **Logging:** Serilog (Console + Rolling File)
- **Docs:** Swagger / OpenAPI
- **Container:** Docker + Docker Compose

---

## Getting Started / Primeiros Passos

### Prerequisites / Pré-requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/) (for PostgreSQL)

---

### 1. Clone the repository / Clone o repositório

```bash
git clone https://github.com/seu-usuario/helpdesk-api.git
cd helpdesk-api
```

### 2. Start the database / Inicie o banco de dados

```bash
docker-compose up -d
```

### 3. Run migrations / Rode as migrations

```bash
dotnet ef database update --project Helpdesk.Infrastructure --startup-project Helpdesk.API
```

### 4. Run the API / Rode a API

```bash
$env:ASPNETCORE_ENVIRONMENT="Development"
dotnet run --project Helpdesk.API --no-launch-profile
```

### 5. Open Swagger / Abra o Swagger

```
http://localhost:5000/swagger
```

---

## API Endpoints & Examples / Exemplos de Requisições

### 🔐 Auth

#### Register / Registrar

```bash
curl -X POST http://localhost:5000/register \
  -H "Content-Type: application/json" \
  -d '{
    "email": "user@example.com",
    "password": "MyPassword123",
    "confirmPassword": "MyPassword123"
  }'
```

**Response:** `201 Created`

---

#### Login

```bash
curl -X POST http://localhost:5000/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "user@example.com",
    "password": "MyPassword123"
  }'
```

**Response:**
```json
{
  "email": "user@example.com",
  "role": "Viewer",
  "isActive": true,
  "jwtToken": "eyJhbGciOiJIUzI1NiIs..."
}
```

---

#### Update Role *(Admin only)*

```bash
curl -X PUT http://localhost:5000/role \
  -H "Authorization: Bearer {token}" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "user@example.com",
    "role": 1
  }'
```

**Response:** `204 No Content`

---

### 🎫 Tickets

> All ticket endpoints require `Authorization: Bearer {token}`

#### Create Ticket

```bash
curl -X POST http://localhost:5000/api/ticket \
  -H "Authorization: Bearer {token}" \
  -H "Content-Type: application/json" \
  -d '{
    "title": "Printer not working",
    "description": "The office printer on floor 2 is offline."
  }'
```

**Response:** `201 Created`

---

#### My Tickets / Meus Tickets

```bash
curl -X GET http://localhost:5000/api/ticket/me \
  -H "Authorization: Bearer {token}"
```

**Response:**
```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "title": "Printer not working",
    "description": "The office printer on floor 2 is offline.",
    "status": 0,
    "userEmail": "user@example.com",
    "createdAt": "2025-01-01T10:00:00Z",
    "updatedAt": "2025-01-01T10:00:00Z"
  }
]
```

---

#### Update Ticket *(Admin or Editor)*

```bash
curl -X PUT http://localhost:5000/api/ticket/{id} \
  -H "Authorization: Bearer {token}" \
  -H "Content-Type: application/json" \
  -d '{
    "title": "Printer not working - URGENT",
    "description": "Updated description.",
    "status": 1
  }'
```

**Response:** `204 No Content`

---

#### Close Ticket *(Admin or Editor)*

```bash
curl -X PUT http://localhost:5000/api/ticket/{id}/close \
  -H "Authorization: Bearer {token}"
```

**Response:** `204 No Content`

---

#### Cancel Ticket *(Admin or Editor)*

```bash
curl -X PUT http://localhost:5000/api/ticket/{id}/cancel \
  -H "Authorization: Bearer {token}"
```

**Response:** `204 No Content`

---

#### Delete Ticket *(Admin only)*

```bash
curl -X DELETE http://localhost:5000/api/ticket/{id} \
  -H "Authorization: Bearer {token}"
```

**Response:** `204 No Content`

---

## Roles & Permissions / Roles e Permissões

| Action | Viewer | Editor | Admin |
|---|:---:|:---:|:---:|
| Register / Login | ✅ | ✅ | ✅ |
| Create ticket | ✅ | ✅ | ✅ |
| View own tickets | ✅ | ✅ | ✅ |
| Update ticket | ❌ | ✅ | ✅ |
| Close ticket | ❌ | ✅ | ✅ |
| Cancel ticket | ❌ | ✅ | ✅ |
| Delete ticket | ❌ | ❌ | ✅ |
| Update user role | ❌ | ❌ | ✅ |

---

## License

MIT © Gabriel#   h e l p d e s k A P I  
 