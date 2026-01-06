# Challenge - Complete Solution

Layered architecture solution developed in .NET 8 for Product and Category management, implemented as part of a technical assessment.

## 🏗️ General Architecture

This solution implements a **Layered Architecture** with **Clean Architecture** and **Domain-Driven Design (DDD)** principles, combining multiple design patterns to create a scalable, maintainable, and testable system.

### Architectural Principles

- **Separation of Concerns**: Each layer has a specific and well-defined responsibility
- **Dependency Inversion**: Upper layers depend on abstractions, not implementations
- **Framework Independence**: The domain does not depend on external frameworks
- **Testability**: Architecture designed to facilitate unit and integration testing

### Design Patterns Implemented

#### 1. CQRS (Command Query Responsibility Segregation)
- **Clear separation** between write operations (Commands) and read operations (Queries)
- **MediatR** as mediator to decouple controllers from business logic
- **Scalability**: Allows independent scaling of read/write stores in the future

#### 2. Domain-Driven Design (DDD)
- **Domain Entities**: Contain encapsulated business logic
- **Value Objects**: Immutable objects for domain concepts
- **Repositories**: Interfaces in the domain, implementations in infrastructure
- **Domain Exceptions**: Business-specific errors

#### 3. Repository Pattern
- **Abstraction** of data access
- **Interfaces in the domain**, implementations in infrastructure
- **Facilitates testing** with mocks and stubs

#### 4. Pipeline Behaviors
- **Cross-cutting concerns** handled centrally
- **Automatic validation** with FluentValidation
- **Structured logging** with Serilog

## 📐 Solution Structure

```
Challenge.Api/
├── src/
│   ├── Challenge.Api/                    # Presentation Layer
│   │   ├── Controllers/                    # REST Endpoints
│   │   ├── Behaviors/                      # Pipeline Behaviors
│   │   └── ConfigureServices/              # Service Configuration
│   │
│   ├── Challenge.Domain/                  # Domain Layer (Core)
│   │   ├── Entities/                       # Business entities
│   │   ├── ValueObjects/                   # Value objects
│   │   ├── Repositories/                   # Repository interfaces
│   │   ├── Services/                        # Service interfaces
│   │   └── Exceptions/                     # Domain exceptions
│   │
│   ├── Challenge.Commands/                # Commands Layer (CQRS)
│   │   ├── Categories/                     # Category commands
│   │   └── Products/                        # Product commands
│   │
│   ├── Challenge.Queries/                 # Queries Layer (CQRS)
│   │   ├── Categories/                     # Category queries
│   │   ├── Products/                        # Product queries
│   │   └── ProductCategories/               # Product-Category relationship queries
│   │
│   ├── Challenge.Infrastructure.Data/     # Infrastructure Layer - Data
│   │   ├── Persistence/                    # DbContext, EF Core
│   │   ├── Repositories/                   # Repository implementations
│   │   └── EntityConfigurations/           # EF configurations
│   │
│   └── Challenge.Infrastructure.CrossCutting/  # Cross-Cutting Concerns
│       ├── Authentications/                # JWT Authentication
│       ├── HealthCheck/                     # Health checks
│       ├── Logging/                         # Logging middleware
│       ├── Storage/                         # Storage services
│       ├── Swagger/                         # Swagger configuration
│       └── Versioning/                      # API versioning
│
└── tests/
    ├── Challenge.Tests/                   # E2E Tests
    │   ├── E2E/                            # End-to-end tests
    │   └── BogusData/                      # Test data generators
    │
    └── Challenge.Commands.Tests/          # Unit Tests
        ├── Categories/                     # Category command tests
        └── Products/                        # Product command tests
```

## 🔄 Data Flow

### HTTP Request Flow

**For Commands (Write Operations):**
```
1. HTTP Request (POST/PUT/DELETE)
   ↓
2. Controller (Challenge.Api)
   ↓
3. MediatR Pipeline
   ├─→ LoggingPipelineBehavior (logging)
   ├─→ ValidatorPipelineBehavior (validation)
   ↓
4. Command Handler (Challenge.Commands)
   ↓
5. Domain Logic (Challenge.Domain)
   ↓
6. Repository (Challenge.Infrastructure.Data)
   ↓
7. Database (SQL Server)
   ↓
8. Response
```

**For Queries (Read Operations):**
```
1. HTTP Request (GET)
   ↓
2. Controller (Challenge.Api)
   ↓
3. MediatR Pipeline
   ├─→ LoggingPipelineBehavior (logging)
   ↓
4. Query Handler (Challenge.Queries)
   ↓
5. Database (SQL Server) - Direct access via DbContext
   ↓
6. Response (DTOs)
```

### Layer Separation

```
┌─────────────────────────────────────┐
│   Challenge.Api                   │  ← Presentation (Controllers)
│   (Depends on Commands/Queries)    │
└──────────────┬──────────────────────┘
               │
┌──────────────▼──────────────────────┐
│   Challenge.Commands/Queries       │  ← Application (CQRS)
│   (Depends on Domain)                │
└──────────────┬──────────────────────┘
               │
┌──────────────▼──────────────────────┐
│   Challenge.Domain                 │  ← Domain (Core)
│   (No external dependencies)         │
└──────────────┬──────────────────────┘
               │
┌──────────────▼──────────────────────┐
│   Challenge.Infrastructure.*       │  ← Infrastructure
│   (Implements Domain interfaces)     │
└─────────────────────────────────────┘
```

## 🎯 Benefits of This Architecture

### Maintainability
- **Organized code** by responsibilities
- **Easy location** of functionalities
- **Low coupling** between layers

### Scalability
- **CQRS** allows independent scaling of read/write
- **Repository Pattern** facilitates database changes
- **Services** can be easily migrated to microservices

### Testability
- **Interfaces** allow easy mocking
- **Business logic** isolated in the domain
- **Fast and isolated** unit tests

### Flexibility
- **Infrastructure changes** do not affect the domain
- **New features** can be added without modifying existing code
- **Migration** to new technologies simplified

## 🚀 Technology Stack

- **.NET 8** - Main framework
- **Entity Framework Core 6.0** - ORM
- **SQL Server** - Database
- **MediatR** - CQRS implementation
- **FluentValidation** - Validation
- **Serilog** - Structured logging
- **Swagger/OpenAPI** - API documentation
- **xUnit, FluentAssertions, NSubstitute** - Testing

## 📚 Detailed Documentation

For detailed information about scalability, architectural design, and high-volume strategies, please refer to:

👉 **[Service README - Scalability](./Challenge-siainteractive.Api/README.md)**

This document contains:
- Scalable API design for thousands of devices
- Mass content distribution strategies
- Large-scale query optimization
- Diagrams and technical explanations
- Pros and cons of each proposed solution

## 🧪 Testing

The solution includes two levels of testing:

- **Unit Tests** (`Challenge.Commands.Tests`): Fast and isolated tests for handlers
- **E2E Tests** (`Challenge.Tests`): Integration tests that verify the complete flow

```bash
# Run all tests
dotnet test

# Unit tests only
dotnet test tests/Challenge.Commands.Tests

# E2E tests only
dotnet test tests/Challenge.Tests
```

## 🏁 Quick Start

1. **Clone the repository**
2. **Configure connection string** in `appsettings.json`
3. **Apply migrations**: `dotnet ef database update --project src/Challenge.Infrastructure.Data`
4. **Run the application**: `dotnet run --project src/Challenge.Api`
5. **Access Swagger**: `https://localhost:5001/swagger`

For more details about scalability and architectural design, see the [Service README](./Challenge.Api/README.md).

---

**Developed as part of a technical assessment**
