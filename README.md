# 🚀 Professional .NET 8 Web API Template

A robust, architecturally sound Web API template following **Clean Architecture** principles, **SOLID** design patterns, and modern development best practices.

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat&logo=dotnet)
![License](https://img.shields.io/badge/License-MIT-blue.svg)
![Build](https://img.shields.io/badge/Build-Passing-green)

## 📋 Table of Contents

- [🚀 Professional .NET 8 Web API Template](#-professional-net-8-web-api-template)
  - [📋 Table of Contents](#-table-of-contents)
  - [🎯 Vision \& Purpose](#-vision--purpose)
    - [Vision](#vision)
    - [Purpose](#purpose)
    - [Use Cases](#use-cases)
  - [✨ Features](#-features)
    - [🏗️ Architecture \& Design](#️-architecture--design)
    - [🔐 Security \& Standards](#-security--standards)
    - [💾 Data \& Performance](#-data--performance)
    - [📊 Observability](#-observability)
    - [📚 Developer Experience](#-developer-experience)
  - [🏛️ Architecture Overview](#️-architecture-overview)
  - [📁 Project Structure](#-project-structure)
  - [🚀 Getting Started](#-getting-started)
    - [Prerequisites](#prerequisites)
    - [Installation](#installation)
  - [🎨 Design Patterns](#-design-patterns)
  - [🗺️ Roadmap](#️-roadmap)
  - [🤝 Contributing](#-contributing)
  - [📄 License](#-license)
  - [⚠️ Disclaimer \& Attribution](#️-disclaimer--attribution)
  - [👨‍💻 Author](#-author)

---

## 🎯 Vision & Purpose

### Vision

To provide a comprehensive reference implementation for building scalable .NET Web APIs, bridging the gap between simple tutorials and complex real-world system requirements.

### Purpose

This template acts as a sophisticated starting point (boilerplate), allowing developers to:

- ✅ **Accelerate Development:** Skip the setup of standard architectural plumbing.
- ✅ **Adhere to Standards:** Implement industry-recognized best practices (Clean Architecture, DDD) from day one.
- ✅ **Ensure Maintainability:** Use a structure designed for testability and long-term growth.
- ✅ **Learn & Adapt:** Serve as an educational resource for advanced .NET concepts.

### Use Cases

- **Complex Business Logic:** Applications requiring strict separation of concerns.
- **SaaS Blueprints:** Multi-tenant ready architecture structures.
- **Microservices:** Can be adapted as a template for individual services.
- **Educational:** A study guide for developers mastering Clean Architecture.

---

## ✨ Features

### 🏗️ Architecture & Design

- **Clean Architecture (Onion):** Strict separation of dependencies (Core depends on nothing).
- **Domain-Driven Design (DDD):** Domain-centric implementation with rich entities.
- **SOLID Principles:** Rigorous adherence to software design principles.
- **CQRS Ready:** Structured to easily support Command Query Responsibility Segregation.

### 🔐 Security & Standards

- **Authentication Standards:** Interfaces prepared for JWT and API Key integration.
- **Audit Capabilities:** Base entity structures for tracking creation and modifications.
- **Security Best Practices:** CORS policies, secure headers setup, and proper input sanitization.

### 💾 Data & Performance

- **EF Core 8:** Optimized configuration with SQL Server.
- **Repository Pattern:** Generic implementations to reduce boilerplate data access code.
- **Performance Optimization:** Async/Await throughout, response compression, and caching strategies.
- **Database Migrations:** Design-time factory support for ease of deployment.

### 📊 Observability

- **Structured Logging:** Integrated Serilog for JSON-formatted logs.
- **Tracing:** Correlation IDs to track requests across the pipeline.
- **Health Checks:** Built-in endpoints to monitor application status.
- **Global Exception Handling:** Centralized middleware for consistent API error responses.

### 📚 Developer Experience

- **Swagger/OpenAPI:** Comprehensive API documentation.
- **AutoMapper:** Efficient object-to-object mapping.
- **FluentValidation:** Expressive, strong-typed validation rules.
- **Pagination:** Standardized parameters and result wrappers.

---

## 🏛️ Architecture Overview

This project adheres to the **Dependency Rule** as described by Robert C. Martin (Uncle Bob).

```text
┌─────────────────────────────────────────────────────────┐
│                     API Layer                           │
│   Entry point, Controllers, Middleware                  │
└───────────────────────┬─────────────────────────────────┘
                        │ depends on ↓
┌───────────────────────▼─────────────────────────────────┐
│                 Application Layer                       │
│   Interfaces, DTOs, Use Cases, Validation               │
└───────────────────────┬─────────────────────────────────┘
                        │ depends on ↓
┌───────────────────────▼─────────────────────────────────┐
│                   Domain Layer                          │
│   Entities, Enums, Specifications (Pure C#)             │
└───────────────────────▲─────────────────────────────────┘
                        │ implements ↑
┌───────────────────────┴─────────────────────────────────┐
│              Infrastructure Layer                       │
│   EF Core, External Services, File System               │
└─────────────────────────────────────────────────────────┘
```

**Key Principle:** The Domain and Application layers differ from the Infrastructure and API layers. The core logic creates interfaces; the outer layers provide implementations (Dependency Inversion).

---

## 📁 Project Structure

```plaintext
src/
├── ProjectName.Api/            # 🌐 Entry Point
├── ProjectName.Application/    # 📋 Business Logic (Interfaces)
├── ProjectName.Domain/         # 🎯 Core Entities (Pure)
└── ProjectName.Infrastructure/ # 🔧 Data Access & External Libs
```

---

## 🚀 Getting Started

### Prerequisites

- .NET 8 SDK
- SQL Server (LocalDB or Docker container)
- IDE (VS 2022, Rider, or VS Code)

### Installation

1. **Clone the repository**

```bash
git clone https://github.com/Onkar-Mahamuni/ReferenceWebApi.git
```

2. **Configuration**

Update `appsettings.json` connection string:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=clean_arch_db;Trusted_Connection=True;"
}
```

3. **Database Migration**

```bash
dotnet ef database update --project src/ProjectName.Infrastructure --startup-project src/ProjectName.Api
```

4. **Run**

```bash
dotnet run --project src/ProjectName.Api
```

---

## 🎨 Design Patterns

This template demonstrates the practical application of several GoF and Enterprise patterns:

- **Repository Pattern:** Abstraction of data persistence.
- **Unit of Work:** Managing atomic transactions (via EF Core).
- **Specification Pattern:** Encapsulating query logic.
- **Decorator Pattern:** Used in Middleware pipeline (Logging, Error Handling).
- **Strategy Pattern:** Used for interchangeable services (e.g., Caching providers).

---

## 🗺️ Roadmap

- [ ] Integration Tests setup using xUnit and TestContainers.
- [ ] Docker Compose support.
- [ ] CI/CD Workflow examples (GitHub Actions).
- [ ] Redis Caching implementation example.

Suggestions? Open an issue to discuss!

---

## 🤝 Contributing

Contributions are welcome! This is a community-driven project.

1. Fork the Project
2. Create your Feature Branch (`git checkout -b feature/AmazingFeature`)
3. Commit your Changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the Branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

---

## 📄 License

Distributed under the MIT License. See LICENSE for more information.

---

## ⚠️ Disclaimer & Attribution

**Personal Project Status:** This project is a personal undertaking developed entirely on personal time and equipment. It is not affiliated with, endorsed by, or connected to my current or past employers.

**Public Domain Knowledge:** The architectural patterns (Clean Architecture, Onion Architecture) and code structures demonstrated here are based on public domain knowledge, open-source documentation (Microsoft Docs, eShopOnContainers), and standard software engineering literature. No proprietary code, trade secrets, or confidential logic from any employer has been used.

**Use at Your Own Risk:** While this template follows professional standards, it is provided "as-is" without warranty of any kind. Developers should review and audit the code before using it in production environments.

---

## 👨‍💻 Author

Onkar Mahamuni

- [LinkedIn](https://www.linkedin.com/in/onkar-mahamuni/)

If you find this template helpful, please give it a ⭐ to support the project!
