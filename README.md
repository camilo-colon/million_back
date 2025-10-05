# Million Backend

This is a .NET project that implements Clean Architecture with CQRS pattern, using MongoDB as the database and Docker for containerization.

## Project Structure

```
million/
├── million.api/           # API Layer
├── million.application/   # Application Layer
├── million.domain/        # Domain Layer
├── million.infrastructure/# Infrastructure Layer
└── compose.yaml          # Docker Configuration
```

### Project Layers

#### 1. API Layer (million.api)
- Handles HTTP endpoints
- Implements OpenAPI/Swagger for documentation
- Manages application configuration

#### 2. Application Layer (million.application)
- Implements CQRS pattern
- Contains application use cases
- Structure:
  - Commands/: Data modification commands
  - Queries/: Data retrieval queries
  - Common/: Shared components

#### 3. Domain Layer (million.domain)
- Contains core entities
- Defines interfaces and contracts
- Implements business rules

#### 4. Infrastructure Layer (million.infrastructure)
- Implements MongoDB persistence
- Database configuration
- Concrete implementations of domain interfaces

## Prerequisites

- .NET 7.0 or higher
- Docker Desktop
- IDE (Visual Studio/Rider)

## Getting Started

1. **Clone the Repository**
```bash
git clone https://github.com/camilo-colon/million_back.git
cd million_back
```

2. **Start MongoDB with Docker**
```bash
docker-compose up -d
```
This will start MongoDB on port 27017 with the following credentials:
- Username: admin
- Password: password

3. **Restore Dependencies**
```bash
dotnet restore
```

4. **Build the Project**
```bash
dotnet build
```

5. **Run the Application**
```bash
cd million.api
dotnet run
```

The API will be available at:
- API: https://localhost:7001
- Swagger UI: https://localhost:7001/swagger

## Database Structure
MongoDB runs in a container with data persistence in the `mongodb_data/` folder.

### Domain Model Mapping
The project uses MongoDB.Bson for mapping domain models to MongoDB collections. Each domain entity has its corresponding configuration class in the Infrastructure layer that implements `IBsonMapConfiguration`.

Example of domain model mapping (`PropertyConfiguration.cs`):

```csharp
public class PropertyConfiguration : IBsonMapConfiguration
{
    public void Configure()
    {
        BsonClassMap.RegisterClassMap<Property>(cm =>
        {
            cm.AutoMap();
            // Maps C# property names to MongoDB field names
            cm.MapProperty(c => c.CodeInternal).SetElementName("code_internal");
            cm.MapProperty(c => c.Address).SetElementName("address");
            cm.MapProperty(c => c.Name).SetElementName("name");
            cm.MapProperty(c => c.Price).SetElementName("price");
            cm.MapProperty(c => c.Year).SetElementName("year");
            
            // Special handling for GUIDs
            cm.MapProperty(c => c.OwnerId)
              .SetElementName("owner_id")
              .SetSerializer(GuidSerializer.StandardInstance);
            
            // Array/List mappings
            cm.MapProperty(c => c.ImagesIds).SetElementName("images_ids");
            cm.MapProperty(c => c.TracesIds).SetElementName("traces_ids");
            
            // Allows for future MongoDB schema evolution
            cm.SetIgnoreExtraElements(true);
        });
    }
}
```

Key features of the mapping:
- **Snake Case Naming**: MongoDB fields use snake_case convention (e.g., `code_internal` instead of `CodeInternal`)
- **Custom Serializers**: Special types like GUIDs use custom serializers
- **Schema Evolution**: `SetIgnoreExtraElements(true)` allows for backwards compatibility
- **Relationship Handling**: Related entities are referenced by their IDs (`OwnerId`, `ImagesIds`, `TracesIds`)

To add a new domain model:
1. Create the entity class in the Domain layer
2. Create a configuration class implementing `IBsonMapConfiguration` in the Infrastructure layer
3. Define the mapping using `BsonClassMap.RegisterClassMap`

## API Documentation
API documentation is available through Swagger UI when the application runs in development mode.

## Implemented Patterns and Practices

### 1. Clean Architecture
- Clear separation of responsibilities
- Dependencies toward the center (Domain)
- Dependency inversion

### 2. CQRS with MediatR
The project implements the CQRS pattern using MediatR library, separating read and write operations into Queries and Commands.

#### Query Implementation Example

1. **Query Definition (ListPropertiesQuery.cs)**
```csharp
public record ListPropertiesQuery : IRequest<List<Property>>
{ 
    public string? Name { get; set; }
    public string? Address { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
}
```

2. **Query Handler (ListPropertiesQueryHandler.cs)**
```csharp
public class ListPropertiesQueryHandler(IPropertyRepository repository) 
    : IRequestHandler<ListPropertiesQuery, List<Property>>
{
    public async Task<List<Property>> Handle(
        ListPropertiesQuery request, 
        CancellationToken cancellationToken)
    {
        var spec = new PropertySpecificationBuilder()
            .WithName(request.Name)
            .WithAddress(request.Address)
            .WithRangePrice(request.MinPrice, request.MaxPrice)
            .Build(); 

        return await repository.FindAsync(spec, cancellationToken);
    }
}
```

3. **Controller Implementation (PropertiesController.cs)**
```csharp
[ApiController]
[Route("api/[controller]")]
public class PropertiesController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] GetPropertiesRequest request)
    {
        var query = new ListPropertiesQuery
        {
            Name = request.Name,
            Address = request.Address,
            MinPrice = request.MinPrice,
            MaxPrice = request.MaxPrice
        };
        var response = await sender.Send(query);
        return Ok(response);
    }
}
```

#### Key Components and Flow:
1. **Queries/Commands**: 
   - Implement `IRequest<TResponse>`
   - Define the request parameters
   - Specify the return type

2. **Handlers**:
   - Implement `IRequestHandler<TRequest, TResponse>`
   - Contain the business logic
   - Use domain services and repositories
   - Return the response

3. **Controllers**:
   - Inject `ISender` interface
   - Map API requests to queries/commands
   - Use `sender.Send()` to dispatch requests
   - Handle responses

#### Benefits:
- **Separation of Concerns**: Clear separation between API models, domain logic, and data access
- **Single Responsibility**: Each handler focuses on one specific use case
- **Maintainability**: Easy to add new features without modifying existing code
- **Testability**: Handlers can be tested in isolation
- **Middleware Support**: MediatR pipeline for cross-cutting concerns
- **Scalability**: Easy to add new queries and commands as the application grows

### 3. Dependency Injection
- Each layer registers its services
- Modular configuration
- Easy testability

### 4. Specification Pattern
The project implements the Specification Pattern for building flexible and composable queries. This pattern allows for:
- Encapsulation of query criteria
- Reusable query specifications
- Composable filters using logical operators
- Clean translation to MongoDB queries

#### Base Specification
```csharp
public abstract class Specification<TEntity> : ISpecification<TEntity> where TEntity : Entity
{
    public static readonly Specification<TEntity> All = new IdentitySpecification<TEntity>();
    public abstract Expression<Func<TEntity, bool>> ToExpression();
    
    public Specification<TEntity> And(Specification<TEntity> specification)
    {
        return new AndSpecification<TEntity>(this, specification);
    }
}
```

#### Example Concrete Specification
```csharp
public class PropertyByNameSpec(string name) : Specification<Property>
{
    public override Expression<Func<Property, bool>> ToExpression()
    {
        return p => p.Name.Contains(name);
    }
}
```

#### Specification Builder Pattern
The project uses a builder pattern to construct complex specifications:

```csharp
public class PropertySpecificationBuilder
{
    private Specification<Property> _spec = Specification<Property>.All;

    public PropertySpecificationBuilder WithName(string? name)
    {
        if (!string.IsNullOrEmpty(name))
        {
            _spec = _spec.And(new PropertyByNameSpec(name));
        }
        return this;
    }

    public PropertySpecificationBuilder WithAddress(string? address)
    {
        if (!string.IsNullOrEmpty(address))
        {
            _spec = _spec.And(new PropertyByAddressSpec(address));
        }
        return this;
    }

    public Specification<Property> Build() => _spec;
}
```

#### MongoDB Repository Implementation
The specifications are translated to MongoDB filters in the repository layer:

```csharp
public class PropertyMongoRepository(IMongoDatabase database) : IPropertyRepository
{
    private readonly IMongoCollection<Property> _collection = 
        database.GetCollection<Property>("properties");

    public Task<List<Property>> FindAsync(
        ISpecification<Property> spec, 
        CancellationToken token)
    {
        var filters = SpecificationToMongoFilterConverter<Property>.Converter(spec);
        return _collection.Find(filters).ToListAsync(token);
    }
}
```

#### Usage Example
```csharp
// Create a specification for properties
var specBuilder = new PropertySpecificationBuilder()
    .WithName("Beach House")
    .WithAddress("Miami")
    .WithRangePrice(100000, 500000);

// Use the specification in the repository
var properties = await propertyRepository.FindAsync(
    specBuilder.Build(), 
    CancellationToken.None);
```

Key benefits:
- **Encapsulation**: Query logic is encapsulated in specification classes
- **Reusability**: Specifications can be reused across different queries
- **Composability**: Specifications can be combined using logical operators
- **Testability**: Easy to unit test individual specifications
- **Maintainability**: Clear separation between query logic and data access
- **Type Safety**: Strongly typed specifications prevent runtime errors

## Development Guide

To add new features:
1. Define entities in `million.domain`
2. Create Commands/Queries in `million.application`
3. Implement persistence in `million.infrastructure`
4. Expose endpoints in `million.api`

## Contributing
Please read [CONTRIBUTING.md](CONTRIBUTING.md) for details on our code of conduct and the process for submitting pull requests.

## License
This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.