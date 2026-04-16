# ☕ B2B Coffee Roastery Platform

> Enterprise-grade B2B Coffee Procurement Platform built with .NET 10 Microservices Architecture

[![.NET](https://img.shields.io/badge/.NET-10.0-purple)](https://dotnet.microsoft.com)
[![Architecture](https://img.shields.io/badge/Architecture-Clean%20Architecture-blue)](/)
[![Pattern](https://img.shields.io/badge/Pattern-CQRS%20%2B%20Saga-orange)](/)
[![Messaging](https://img.shields.io/badge/Messaging-RabbitMQ-red)](/)

---

## 📋 Overview

A fully automated B2B coffee procurement platform that handles the complete
business lifecycle — from client onboarding with OTP verification and admin
approval, to order placement, real-time inventory management, delivery
tracking, and Gmail notifications at every step.

---

## 🏗️ Architecture
---

## 🚀 Tech Stack

| Layer | Technology |
|-------|-----------|
| Runtime | .NET 10 |
| Architecture | Clean Architecture (Domain → Application → Infrastructure → API) |
| CQRS | MediatR 14 |
| Messaging | MassTransit 8 + RabbitMQ |
| Saga | MassTransit State Machine |
| ORM | Entity Framework Core 9 |
| Database | SQL Server Express (Database-per-service) |
| Gateway | Ocelot 23 |
| Auth | JWT Bearer + BCrypt |
| Email | MailKit + MimeKit (Real Gmail) |
| PDF | QuestPDF |
| AI Chatbot | Anthropic Claude API |
| Docs | Swashbuckle + SwaggerForOcelot |

---

## 📦 Services

| Service | Port | Responsibility |
|---------|------|---------------|
| AuthService | 5001 | SSO, JWT, OTP, Identity Hierarchy |
| ProductService | 5002 | Coffee catalog, reviews, filtering |
| InventoryService | 5003 | Stock management, saga deduction |
| OrderService | 5004 | Orders, CQRS, Fulfillment Saga |
| DeliveryService | 5005 | Delivery tracking, agent assignment |
| NotificationService | 5006 | Real Gmail emails, PDF invoices |
| ApiGateway | 7101 | Ocelot routing, JWT validation, CORS |

---

## 🔑 Identity Hierarchy
---

## 📧 Real-Time Gmail Notifications

| Trigger | Email Sent |
|---------|-----------|
| Client registers | OTP verification code |
| Admin approves | Welcome & Account Activated |
| Order placed | Order received + Invoice PDF |
| Order confirmed | In-Process notification |
| Order dispatched | Dispatched + Tracking number |
| Order delivered | Delivery confirmation |
| Order rejected | Rejection with reason |

---

## ⚙️ Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [SQL Server Express](https://www.microsoft.com/sql-server)
- [Docker](https://docker.com) (for RabbitMQ)
- Gmail account with App Password enabled

---

## 🛠️ Setup & Run

### 1. Start RabbitMQ
```bash
docker run -d --hostname rabbitmq --name rabbitmq \
  -p 5672:5672 -p 15672:15672 \
  rabbitmq:3-management
```

### 2. Configure Gmail
Update `SenderEmail` and `Password` in the `SmtpConfig` seed data
in `AuthService.Infrastructure/Data/AuthDbContext.cs` and
`NotificationService.Infrastructure/Data/NotificationDbContext.cs`

### 3. Run Migrations (run once per service)
```bash
cd AuthService/AuthService.API
dotnet ef database update --project ../AuthService.Infrastructure/AuthService.Infrastructure.csproj
```
Repeat for each service.

### 4. Start All Services
Open 7 terminals and run each:
```bash
# Terminal 1-6: individual services
cd {ServiceName}/{ServiceName}.API && dotnet run

# Terminal 7: Gateway (start last)
cd ApiGateway && dotnet run
```

### 5. Access
- **Gateway:** http://localhost:7101
- **RabbitMQ UI:** http://localhost:15672 (guest/guest)
- **SuperAdmin Login:** superadmin@b2bcoffee.com / SuperAdmin@123

---

## 🔄 Order Fulfillment Saga
---

## 📁 Project Structure

---

## 🤖 AI Chatbot
Integrated Anthropic Claude API for intelligent client support —
product recommendations, order queries, and brewing guides.

---


