using FluentAssertions;
using million.domain.properties;
using million.domain.properties.specifications;
using million.infrastructure.Properties.Persistence;

namespace Million.Infrastructure.IntegrationTests.Properties.Persistence;

[TestFixture]
public class PropertyMongoRepositoryTest
{
    private PropertyMongoRepository _repository = null!;

    [SetUp]
    public async Task SetUp()
    {
        await MongoDbFixture.CleanDatabase();
        _repository = new PropertyMongoRepository(MongoDbFixture.Database);
    }

    [Test]
    public async Task GetBySpec_ShouldRetrievePropertiesFromDatabase()
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

        var spec = new PropertyByNameSpec("Casa");

        // Act
        var result = await _repository.GetBySpec(spec, CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(p => p.Name == "Casa en la playa");
        result.Should().Contain(p => p.Name == "Casa moderna");
    }

    [Test]
    public async Task GetBySpec_WithPropertySpecificationBuilder_ShouldWorkCorrectly()
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

        var spec = new PropertySpecificationBuilder()
            .WithName("Casa")
            .WithRangePrice(200000m, 400000m)
            .Build();

        // Act
        var result = await _repository.GetBySpec(spec, CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(p => p.Name == "Casa moderna");
        result.Should().Contain(p => p.Name == "Casa de campo");
    }

    [Test]
    public async Task GetBySpec_WithComplexFilter_ShouldReturnCorrectResults()
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

        var spec = new PropertySpecificationBuilder()
            .WithName("Casa")
            .WithAddress("Calle Sol 123")
            .WithRangePrice(100000m, 200000m)
            .Build();

        // Act
        var result = await _repository.GetBySpec(spec, CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        result.First().Name.Should().Be("Casa en la playa");
        result.First().Address.Should().Be("Calle Sol 123");
        result.First().Price.Should().Be(150000m);
    }
}
