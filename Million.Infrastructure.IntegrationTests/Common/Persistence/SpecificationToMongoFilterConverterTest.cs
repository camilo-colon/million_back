using FluentAssertions;
using million.domain.properties;
using million.domain.properties.specifications;
using million.domain.Common.specifications;
using million.infrastructure.Common.Persistence.mongodb;
using MongoDB.Driver;

namespace Million.Infrastructure.IntegrationTests.Common.Persistence;

[TestFixture]
public class SpecificationToMongoFilterConverterTest
{

    [SetUp]
    public async Task SetUp()
    {
        await MongoDbFixture.CleanDatabase();
    }

    [Test]
    public async Task Converter_WithAllSpecification_ShouldReturnAllDocuments()
    {
        // Arrange
        var properties = new List<Property>
        {
            new(Guid.NewGuid(), "Casa en la playa", "Calle Sol 123", 150000m, "C1", 2020),
            new(Guid.NewGuid(), "Casa moderna", "Avenida Luna 456", 350000m, "C2", 2021)
        };

        var collection = MongoDbFixture.Database.GetCollection<Property>("properties");
        await collection.InsertManyAsync(properties);

        var spec = Specification<Property>.All;
        var filter = SpecificationToMongoFilterConverter<Property>.Converter(spec);

        // Act
        var result = await collection.Find(filter).ToListAsync();

        // Assert
        result.Should().HaveCount(2);
    }

    [Test]
    public async Task Converter_WithNameSpecification_ShouldFilterCorrectly()
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
        var filter = SpecificationToMongoFilterConverter<Property>.Converter(spec);

        // Act
        var result = await collection.Find(filter).ToListAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(p => p.Name == "Casa en la playa");
        result.Should().Contain(p => p.Name == "Casa moderna");
    }

    [Test]
    public async Task Converter_WithAddressSpecification_ShouldFilterCorrectly()
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
        var filter = SpecificationToMongoFilterConverter<Property>.Converter(spec);

        // Act
        var result = await collection.Find(filter).ToListAsync();

        // Assert
        result.Should().HaveCount(1);
        result[0].Address.Should().Be("Calle Sol 123");
    }

    [Test]
    public async Task Converter_WithPriceRangeSpecification_ShouldFilterCorrectly()
    {
        // Arrange
        var properties = new List<Property>
        {
            new(Guid.NewGuid(), "Casa en la playa", "Calle Sol 123", 150000m, "C1", 2020),
            new(Guid.NewGuid(), "Casa moderna", "Avenida Luna 456", 350000m, "C2", 2021),
            new(Guid.NewGuid(), "Apartamento céntrico", "Calle Sol 789", 200000m, "C3", 2022),
            new(Guid.NewGuid(), "Villa de lujo", "Boulevard Estrella 101", 800000m, "C4", 2023)
        };

        var collection = MongoDbFixture.Database.GetCollection<Property>("properties");
        await collection.InsertManyAsync(properties);

        var spec = new PropertyByRangePriceSpec(200000m, 400000m);
        var filter = SpecificationToMongoFilterConverter<Property>.Converter(spec);

        // Act
        var result = await collection.Find(filter).ToListAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(p => p.Name == "Casa moderna");
        result.Should().Contain(p => p.Name == "Apartamento céntrico");
    }

    [Test]
    public async Task Converter_WithAndSpecification_ShouldCombineFilters()
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
        var filter = SpecificationToMongoFilterConverter<Property>.Converter(spec);

        // Act
        var result = await collection.Find(filter).ToListAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(p => p.Name == "Casa moderna");
        result.Should().Contain(p => p.Name == "Casa de campo");
    }

    [Test]
    public async Task Converter_WithMultipleAndSpecifications_ShouldCombineAllFilters()
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
        var filter = SpecificationToMongoFilterConverter<Property>.Converter(spec);

        // Act
        var result = await collection.Find(filter).ToListAsync();

        // Assert
        result.Should().HaveCount(1);
        result[0].Name.Should().Be("Casa en la playa");
        result[0].Address.Should().Be("Calle Sol 123");
        result[0].Price.Should().Be(150000m);
    }

    [Test]
    public async Task Converter_WithNoMatches_ShouldReturnEmptyList()
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
        var filter = SpecificationToMongoFilterConverter<Property>.Converter(spec);

        // Act
        var result = await collection.Find(filter).ToListAsync();

        // Assert
        result.Should().BeEmpty();
    }

    [Test]
    public void Converter_ShouldReturnFilterDefinition()
    {
        // Arrange
        var spec = new PropertyByNameSpec("Casa");

        // Act
        var filter = SpecificationToMongoFilterConverter<Property>.Converter(spec);

        // Assert
        filter.Should().NotBeNull();
        filter.Should().BeAssignableTo<FilterDefinition<Property>>();
    }
}
