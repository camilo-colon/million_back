# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build and Development Commands

### Building the Solution
```bash
dotnet build million.sln
```

### Running the Application
```bash
dotnet run --project million.api/million.api.csproj
```

### Running Tests
```bash
# Run all tests
dotnet test million.sln

# Run tests for a specific project
dotnet test Million.Application.UnitTests/Million.Application.UnitTests.csproj
dotnet test Million.Domain.UnitTests/Million.Domain.UnitTests.csproj
dotnet test Million.Infrastructure.IntegrationTests/Million.Infrastructure.IntegrationTests.csproj

# Run a specific test
dotnet test --filter "FullyQualifiedName~TestClassName.TestMethodName"
```

### Docker Compose
```bash
# Start MongoDB
docker-compose -f compose.yaml up -d

# Stop MongoDB
docker-compose -f compose.yaml down
```

## Architecture Overview

This is a .NET 9.0 application following **Clean Architecture** principles with four main layers:

### Layer Structure
- **million.api** - Presentation layer with API controllers
- **million.application** - Application layer implementing CQRS with MediatR
- **million.domain** - Core domain entities and business logic with Specification Pattern
- **million.infrastructure** - Infrastructure layer with MongoDB persistence

### Key Architectural Patterns

#### 1. Clean Architecture Layering
Dependencies flow inward: API → Application → Domain ← Infrastructure

Each layer has a `DependencyInjection.cs` file that registers services:
- `AddPresentation()` - Controllers and API-specific services
- `AddApplication()` - MediatR handlers
- `AddInfrastructure(configuration)` - Database and repositories

#### 2. CQRS Pattern with MediatR
Application layer uses MediatR for queries and commands. Structure:
```
million.application/
  Properties/
    Queries/
      ListProperties/
        ListPropertiesQuery.cs
        ListPropertiesQueryHandler.cs
        PropertyResult.cs
```

#### 3. Specification Pattern
The domain implements a robust specification pattern for querying:

- Base interface: `ISpecification<TEntity>` with `ToExpression()` method
- Base class: `Specification<TEntity>` with composability via `And()` operations
- Builder pattern: `PropertySpecificationBuilder` for fluent query construction
- Concrete specs in `million.domain/{Entity}/specifications/`

Example usage:
```csharp
var spec = new PropertySpecificationBuilder()
    .WithName(name)
    .WithAddress(address)
    .WithRangePrice(minPrice, maxPrice)
    .Build();
```

#### 4. Repository Pattern
- Generic interface: `IGenericRepository<TEntity>` with `GetBySpec()` method
- MongoDB implementation: `GenericMongoRepository<TEntity>`
- Specification-to-MongoDB filter conversion via `SpecificationToMongoFilterConverter`

### Data Persistence

**Database**: MongoDB (configured via Docker Compose)

Configuration in `appsettings.Development.json`:
```json
{
  "MongoDbSettings": {
    "ConnectionString": "mongodb://admin:password@localhost:27017/",
    "DatabaseName": "million"
  }
}
```

MongoDB client and database are registered as singletons in infrastructure DI.
BsonClassMap registration happens automatically from assembly in `AddPersistence()`.

### Testing Framework
- **NUnit** for test framework
- **FluentAssertions** for assertion library
- **Moq** for mocking dependencies (unit tests)
- **Testcontainers** for integration testing with MongoDB

Test projects:
- `Million.Application.UnitTests` - Tests for application layer handlers
- `Million.Domain.UnitTests` - Tests for domain logic and specifications
- `Million.Infrastructure.IntegrationTests` - Integration tests for MongoDB repositories using Testcontainers

#### Integration Tests with Testcontainers
Integration tests use Testcontainers to spin up a real MongoDB instance in Docker:
- Tests run against an actual MongoDB database, not mocks
- Each test suite shares a MongoDB container for performance
- Database is cleaned between tests via `MongoDbFixture.CleanDatabase()`
- BsonClassMap configuration is reused from infrastructure layer via `BsonClassMapRegister`

## Domain Entities

The codebase manages real estate data with these main aggregates:
- **Property** - Real estate listings with price, address, name
- **PropertyImage** - Images associated with properties
- **Owner** - Property ownership information

Each entity inherits from `Entity` base class.

## Adding New Features

### Adding a New Query
1. Create folder in `million.application/{Entity}/Queries/{QueryName}/`
2. Create `{QueryName}Query.cs` implementing `IRequest<TResponse>`
3. Create `{QueryName}QueryHandler.cs` implementing `IRequestHandler<TQuery, TResponse>`
4. Create result DTOs as needed
5. MediatR automatically registers handlers via assembly scanning

### Adding a New Specification
1. Create `{Entity}By{Criteria}Spec.cs` in `million.domain/{Entity}/specifications/`
2. Inherit from `Specification<{Entity}>`
3. Implement `ToExpression()` returning the filter expression
4. Add builder method to `{Entity}SpecificationBuilder` if using builder pattern

### Adding a New Repository
1. Define interface in `million.domain/{Entity}/I{Entity}Repository.cs`
2. Implement in `million.infrastructure/{Entity}/Persistence/{Entity}MongoRepository.cs`
3. Register in `DependencyInjection.AddPersistence()`
