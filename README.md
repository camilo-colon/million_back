# Million - Real Estate Management System

A modern real estate management API built with .NET 9.0, implementing Clean Architecture principles with CQRS, Specification Pattern, and MongoDB persistence.

## 🏗️ Architecture

This project follows **Clean Architecture** principles with clear separation of concerns across four layers:

```
┌─────────────────────────────────────────────────────────┐
│                     Presentation                         │
│                   (million.api)                          │
│              Controllers, DTOs, OpenAPI                  │
└──────────────────────┬──────────────────────────────────┘
                       │
┌──────────────────────▼──────────────────────────────────┐
│                     Application                          │
│                (million.application)                     │
│          MediatR Handlers, Queries, Commands             │
└──────────────────────┬──────────────────────────────────┘
                       │
┌──────────────────────▼──────────────────────────────────┐
│                       Domain                             │
│                  (million.domain)                        │
│    Entities, Specifications, Business Rules              │
└──────────────────────▲──────────────────────────────────┘
                       │
┌──────────────────────┴──────────────────────────────────┐
│                   Infrastructure                         │
│               (million.infrastructure)                   │
│      MongoDB Repositories, Persistence Config            │
└─────────────────────────────────────────────────────────┘
```

### Key Architectural Patterns

#### 1. CQRS with MediatR
- **Queries**: Read operations return DTOs optimized for display
- **Commands**: Write operations encapsulate business logic
- Handlers are auto-discovered and registered via assembly scanning

#### 2. Specification Pattern
Composable, reusable query logic with builder pattern:

```csharp
var spec = new PropertySpecificationBuilder()
    .WithName("Casa")
    .WithAddress("Calle Sol")
    .WithRangePrice(100000m, 500000m)
    .Build();

var properties = await repository.GetBySpec(spec);
```

#### 3. Repository Pattern
Generic repository with specification support for MongoDB:
- `IGenericRepository<TEntity>` with `GetBySpec()` method
- Automatic conversion from LINQ expressions to MongoDB filters
- BsonClassMap configuration for entity serialization

## 🚀 Getting Started

### Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop) (for MongoDB)
- IDE: Visual Studio 2022, JetBrains Rider, or VS Code

### Installation

1. **Clone the repository**
```bash
git clone <repository-url>
cd million
```

2. **Start MongoDB using Docker Compose**
```bash
docker-compose up -d
```

This will start MongoDB on `localhost:27017` with credentials:
- Username: `admin`
- Password: `password`
- Database: `million`

3. **Restore dependencies**
```bash
dotnet restore
```

4. **Build the solution**
```bash
dotnet build
```

5. **Run the API**
```bash
dotnet run --project million.api/million.api.csproj
```

The API will be available at:
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:5001`
- Swagger UI: `https://localhost:5001/swagger`

## 📡 API Endpoints

### Properties

#### GET /api/properties
Get a list of properties with optional filters.

**Query Parameters:**
- `name` (string, optional): Filter by property name (partial match)
- `address` (string, optional): Filter by address (exact match)
- `minPrice` (decimal, optional): Minimum price filter
- `maxPrice` (decimal, optional): Maximum price filter

**Example Request:**
```bash
curl "https://localhost:5001/api/properties?name=Casa&minPrice=100000&maxPrice=500000"
```

**Example Response:**
```json
[
  {
    "idOwner": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "name": "Casa en la playa",
    "address": "Calle Sol 123",
    "price": 250000.00,
    "images": [
      "https://example.com/image1.jpg",
      "https://example.com/image2.jpg"
    ]
  }
]
```

## 🧪 Testing

The project includes three types of tests:

### Unit Tests

**Domain Tests** (`Million.Domain.UnitTests`)
- Specification pattern tests
- Business logic validation
- Entity behavior tests

```bash
dotnet test Million.Domain.UnitTests/Million.Domain.UnitTests.csproj
```

**Application Tests** (`Million.Application.UnitTests`)
- Query/Command handler tests
- Use case validation
- Mocked dependencies

```bash
dotnet test Million.Application.UnitTests/Million.Application.UnitTests.csproj
```

### Integration Tests

**Infrastructure Tests** (`Million.Infrastructure.IntegrationTests`)
- Real MongoDB integration using Testcontainers
- Repository functionality tests
- Specification-to-MongoDB filter conversion tests

```bash
dotnet test Million.Infrastructure.IntegrationTests/Million.Infrastructure.IntegrationTests.csproj
```

### Run All Tests
```bash
dotnet test million.sln
```

### Test Coverage
- **19** integration tests with Testcontainers
- **NUnit** as test framework
- **FluentAssertions** for readable assertions
- **Moq** for mocking in unit tests

## 🛠️ Development

### Project Structure

```
million/
├── million.api/                      # Presentation Layer
│   ├── Controllers/                  # API Controllers
│   ├── DTOs/                        # Data Transfer Objects
│   └── Program.cs                   # Application entry point
│
├── million.application/              # Application Layer
│   └── Properties/
│       └── Queries/                 # CQRS Queries
│           └── ListProperties/
│               ├── ListPropertiesQuery.cs
│               ├── ListPropertiesQueryHandler.cs
│               └── PropertyResult.cs
│
├── million.domain/                   # Domain Layer
│   ├── Common/
│   │   ├── Entity.cs               # Base entity
│   │   ├── IGenericRepository.cs   # Repository interface
│   │   └── specifications/         # Specification pattern
│   ├── Properties/
│   │   ├── Property.cs             # Property entity
│   │   ├── IPropertyRepository.cs
│   │   └── specifications/         # Property specifications
│   ├── PropertyImages/
│   └── Owners/
│
├── million.infrastructure/           # Infrastructure Layer
│   ├── Common/
│   │   └── Persistence/
│   │       └── mongodb/            # MongoDB configuration
│   ├── Properties/
│   │   └── Persistence/
│   │       ├── PropertyMongoRepository.cs
│   │       └── PropertyConfiguration.cs
│   └── DependencyInjection.cs
│
├── Million.Domain.UnitTests/         # Domain unit tests
├── Million.Application.UnitTests/    # Application unit tests
├── Million.Infrastructure.IntegrationTests/  # Integration tests
│
├── compose.yaml                      # Docker Compose for MongoDB
├── CLAUDE.md                        # AI assistant guidance
└── README.md                        # This file
```

### Adding New Features

#### 1. Create a New Query

```csharp
// 1. Define the query in million.application
public record GetPropertyByIdQuery(Guid Id) : IRequest<PropertyResult>;

// 2. Create the handler
public class GetPropertyByIdQueryHandler : IRequestHandler<GetPropertyByIdQuery, PropertyResult>
{
    private readonly IPropertyRepository _repository;

    public async Task<PropertyResult> Handle(GetPropertyByIdQuery request, CancellationToken cancellationToken)
    {
        // Implementation
    }
}

// 3. Add controller endpoint
[HttpGet("{id}")]
public async Task<IActionResult> GetById(Guid id)
{
    var result = await _sender.Send(new GetPropertyByIdQuery(id));
    return Ok(result);
}
```

#### 2. Create a New Specification

```csharp
// In million.domain/Properties/specifications/
public class PropertyByYearSpec(int year) : Specification<Property>
{
    public override Expression<Func<Property, bool>> ToExpression()
    {
        return p => p.Year == year;
    }
}

// Update the builder
public PropertySpecificationBuilder WithYear(int? year)
{
    if (year.HasValue)
    {
        _spec = _spec.And(new PropertyByYearSpec(year.Value));
    }
    return this;
}
```

#### 3. Add MongoDB Configuration

```csharp
// In million.infrastructure/Properties/Persistence/
public class PropertyConfiguration : IBsonMapConfiguration
{
    public void Configure()
    {
        BsonClassMap.RegisterClassMap<Property>(cm =>
        {
            cm.AutoMap();
            cm.MapProperty(c => c.Name).SetElementName("name");
            // Add more mappings
        });
    }
}
```

### Code Style & Conventions

- **Naming**: PascalCase for public members, camelCase for private fields
- **Records**: Use for immutable DTOs and queries
- **Primary constructors**: Preferred for dependency injection
- **Nullable reference types**: Enabled across all projects
- **File-scoped namespaces**: Used throughout

### Database Migrations

MongoDB is schema-less, but entity structure changes require:
1. Update domain entity
2. Update BsonClassMap configuration
3. Run integration tests to verify
4. Update existing documents if needed (via migration script)

## 🔧 Configuration

### appsettings.json

```json
{
  "MongoDbSettings": {
    "ConnectionString": "mongodb://admin:password@localhost:27017/",
    "DatabaseName": "million"
  }
}
```

### Environment Variables (Optional)

```bash
export MongoDbSettings__ConnectionString="mongodb://admin:password@localhost:27017/"
export MongoDbSettings__DatabaseName="million"
```

## 🐳 Docker

### MongoDB Container
The project uses Docker Compose for MongoDB:

```yaml
services:
  mongodb:
    image: mongo:latest
    ports:
      - "27017:27017"
    volumes:
      - ./mongodb_data:/data/db
    environment:
      MONGO_INITDB_ROOT_USERNAME: admin
      MONGO_INITDB_ROOT_PASSWORD: password
```

**Commands:**
```bash
# Start MongoDB
docker-compose up -d

# Stop MongoDB
docker-compose down

# View logs
docker-compose logs -f mongodb

# Access MongoDB shell
docker exec -it mongodb mongosh -u admin -p password
```

## 📚 Technology Stack

### Backend
- **.NET 9.0** - Latest .NET framework
- **ASP.NET Core** - Web API framework
- **MediatR** - CQRS implementation
- **MongoDB.Driver** - MongoDB .NET driver

### Testing
- **NUnit** - Test framework
- **FluentAssertions** - Fluent assertion library
- **Moq** - Mocking framework
- **Testcontainers** - Integration testing with Docker

### Tools
- **Swagger/OpenAPI** - API documentation
- **Docker & Docker Compose** - Containerization

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

### Contribution Guidelines

- Follow Clean Architecture principles
- Write tests for new features
- Update documentation
- Follow existing code style
- Ensure all tests pass before submitting PR

## 📝 License

This project is licensed under the MIT License.

## 👥 Authors

- Camilo Colon - Initial work

## 🙏 Acknowledgments

- Clean Architecture by Robert C. Martin
- Domain-Driven Design patterns
- CQRS pattern
- Specification pattern by Martin Fowler
