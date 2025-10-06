using FluentAssertions;
using million.domain.Common.specifications;
using million.domain.properties;
using million.domain.properties.specifications;
using million.infrastructure.Common.Persistence;
using million.infrastructure.Common.Persistence.mongodb;

namespace Million.Infrastructure.IntegrationTests.Common.Persistence;

[TestFixture]
public class GenericMongoRepositoryTest
{
    private GenericMongoRepository<Property> _repository = null!;

    [SetUp]
    public async Task SetUp()
    {
        await MongoDbFixture.CleanDatabase();
        _repository = new GenericMongoRepository<Property>(MongoDbFixture.Database, "properties");
    }

    [Test]
    public async Task GetBySpec_WithAllSpecification_ShouldReturnAllEntities()
    {
        // Arrange
        var properties = new List<Property>
        {
            new(Guid.NewGuid(), "Casa en la playa", "Calle Sol 123", 150000m, "C1", 2020),
            new(Guid.NewGuid(), "Casa moderna", "Avenida Luna 456", 350000m, "C2", 2021),
            new(Guid.NewGuid(), "Apartamento céntrico", "Calle Sol 789", 200000m, "C3", 2022)
        };

        var collection = MongoDbFixture.Database.GetCollection<Property>("properties");
        await collection.InsertManyAsync(properties);

        var spec = Specification<Property>.All;

        // Act
        var result = await _repository.GetBySpec(spec, CancellationToken.None);

        // Assert
        result.Should().HaveCount(3);
        result.Should().BeEquivalentTo(properties, options => options.WithStrictOrdering());
    }

    [Test]
    public async Task GetBySpec_WithNameSpecification_ShouldReturnMatchingEntities()
    {
        // Arrange
        var properties = new List<Property>
        {
            new(Guid.NewGuid(), "Casa en la playa", "Calle Sol 123", 150000m, "C1", 2020),
            new(Guid.NewGuid(), "Casa moderna", "Avenida Luna 456", 350000m, "C2", 2021),
            new(Guid.NewGuid(), "Apartamento céntrico", "Calle Sol 789", 200000m, "C3", 2022),
            new(Guid.NewGuid(), "Casa de campo", "Carretera Norte 202", 250000m, "C4", 2024)
        };

        var collection = MongoDbFixture.Database.GetCollection<Property>("properties");
        await collection.InsertManyAsync(properties);

        var spec = new PropertyByNameSpec("Casa");

        // Act
        var result = await _repository.GetBySpec(spec, CancellationToken.None);

        // Assert
        result.Should().HaveCount(3);
        result.Should().Contain(p => p.Name == "Casa en la playa");
        result.Should().Contain(p => p.Name == "Casa moderna");
        result.Should().Contain(p => p.Name == "Casa de campo");
    }

    [Test]
    public async Task GetBySpec_WithAddressSpecification_ShouldReturnMatchingEntity()
    {
        // Arrange
        var properties = new List<Property>
        {
            new(Guid.NewGuid(), "Casa en la playa", "Calle Sol 123", 150000m, "C1", 2020),
            new(Guid.NewGuid(), "Casa moderna", "Avenida Luna 456", 350000m, "C2", 2021)
        };

        var collection = MongoDbFixture.Database.GetCollection<Property>("properties");
        await collection.InsertManyAsync(properties);

        var spec = new PropertyByAddressSpec("Calle Sol 123");

        // Act
        var result = await _repository.GetBySpec(spec, CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        result[0].Name.Should().Be("Casa en la playa");
        result[0].Address.Should().Be("Calle Sol 123");
    }

    [Test]
    public async Task GetBySpec_WithPriceRangeSpecification_ShouldReturnEntitiesInRange()
    {
        // Arrange
        var properties = new List<Property>
        {
            new(Guid.NewGuid(), "Casa en la playa", "Calle Sol 123", 150000m, "C1", 2020),
            new(Guid.NewGuid(), "Casa moderna", "Avenida Luna 456", 350000m, "C2", 2021),
            new(Guid.NewGuid(), "Apartamento céntrico", "Calle Sol 789", 200000m, "C3", 2022),
            new(Guid.NewGuid(), "Villa de lujo", "Boulevard Estrella 101", 800000m, "C4", 2023),
            new(Guid.NewGuid(), "Casa de campo", "Carretera Norte 202", 250000m, "C5", 2024)
        };

        var collection = MongoDbFixture.Database.GetCollection<Property>("properties");
        await collection.InsertManyAsync(properties);

        var spec = new PropertyByRangePriceSpec(200000m, 400000m);

        // Act
        var result = await _repository.GetBySpec(spec, CancellationToken.None);

        // Assert
        result.Should().HaveCount(3);
        result.Should().Contain(p => p.Name == "Casa moderna");
        result.Should().Contain(p => p.Name == "Apartamento céntrico");
        result.Should().Contain(p => p.Name == "Casa de campo");
    }

    [Test]
    public async Task GetBySpec_WithCombinedSpecifications_ShouldReturnEntitiesMatchingAllCriteria()
    {
        // Arrange
        var properties = new List<Property>
        {
            new(Guid.NewGuid(), "Casa en la playa", "Calle Sol 123", 150000m, "C1", 2020),
            new(Guid.NewGuid(), "Casa moderna", "Avenida Luna 456", 350000m, "C2", 2021),
            new(Guid.NewGuid(), "Apartamento céntrico", "Calle Sol 789", 200000m, "C3", 2022),
            new(Guid.NewGuid(), "Casa de campo", "Carretera Norte 202", 250000m, "C4", 2024)
        };

        var collection = MongoDbFixture.Database.GetCollection<Property>("properties");
        await collection.InsertManyAsync(properties);

        var spec = new PropertyByNameSpec("Casa")
            .And(new PropertyByRangePriceSpec(200000m, 400000m));

        // Act
        var result = await _repository.GetBySpec(spec, CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(p => p.Name == "Casa moderna");
        result.Should().Contain(p => p.Name == "Casa de campo");
    }

    [Test]
    public async Task GetBySpec_WithNoMatches_ShouldReturnEmptyList()
    {
        // Arrange
        var properties = new List<Property>
        {
            new(Guid.NewGuid(), "Casa en la playa", "Calle Sol 123", 150000m, "C1", 2020),
            new(Guid.NewGuid(), "Casa moderna", "Avenida Luna 456", 350000m, "C2", 2021)
        };

        var collection = MongoDbFixture.Database.GetCollection<Property>("properties");
        await collection.InsertManyAsync(properties);

        var spec = new PropertyByNameSpec("Mansión");

        // Act
        var result = await _repository.GetBySpec(spec, CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
    }

    [Test]
    public async Task GetBySpec_WithEmptyCollection_ShouldReturnEmptyList()
    {
        // Arrange
        var spec = Specification<Property>.All;

        // Act
        var result = await _repository.GetBySpec(spec, CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
    }

    [Test]
    public async Task GetBySpec_WithMultipleAndSpecifications_ShouldReturnCorrectResults()
    {
        // Arrange
        var properties = new List<Property>
        {
            new(Guid.NewGuid(), "Casa en la playa", "Calle Sol 123", 150000m, "C1", 2020),
            new(Guid.NewGuid(), "Casa moderna", "Avenida Luna 456", 350000m, "C2", 2021),
            new(Guid.NewGuid(), "Apartamento céntrico", "Calle Sol 789", 200000m, "C3", 2022)
        };

        var collection = MongoDbFixture.Database.GetCollection<Property>("properties");
        await collection.InsertManyAsync(properties);

        var spec = new PropertyByNameSpec("Casa")
            .And(new PropertyByAddressSpec("Calle Sol 123"))
            .And(new PropertyByRangePriceSpec(100000m, 200000m));

        // Act
        var result = await _repository.GetBySpec(spec, CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        result[0].Name.Should().Be("Casa en la playa");
        result[0].Address.Should().Be("Calle Sol 123");
        result[0].Price.Should().Be(150000m);
    }
}
