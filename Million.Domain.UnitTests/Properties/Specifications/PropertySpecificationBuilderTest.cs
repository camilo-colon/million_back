using FluentAssertions;
using million.domain.properties;
using million.domain.properties.specifications;

namespace Million.Domain.UnitTests.Properties.Specifications;

public class PropertySpecificationBuilderTest
{
    private List<Property> _properties = null!;

    [SetUp]
    public void SetUp()
    {
        _properties = new List<Property>
        {
            new Property(Guid.NewGuid(), "Casa en la playa", "Calle Sol 123", 150000m, "C1", 2020),
            new Property(Guid.NewGuid(), "Casa moderna", "Avenida Luna 456", 350000m, "C2", 2021),
            new Property(Guid.NewGuid(), "Apartamento céntrico", "Calle Sol 789", 200000m, "C3", 2022),
            new Property(Guid.NewGuid(), "Villa de lujo", "Boulevard Estrella 101", 800000m, "C4", 2023),
            new Property(Guid.NewGuid(), "Casa de campo", "Carretera Norte 202", 250000m, "C5", 2024)
        };
    }

    [Test]
    public void Build_WithNoFilters_ShouldReturnAllProperties()
    {
        // Arrange
        var spec = new PropertySpecificationBuilder()
            .Build();

        var expression = spec.ToExpression().Compile();

        // Act
        var result = _properties.Where(expression).ToList();

        // Assert
        result.Should().HaveCount(5);
    }

    [Test]
    public void Build_WithNameFilter_ShouldReturnMatchingProperties()
    {
        // Arrange
        var spec = new PropertySpecificationBuilder()
            .WithName("Casa")
            .Build();

        var expression = spec.ToExpression().Compile();

        // Act
        var result = _properties.Where(expression).ToList();

        // Assert
        result.Should().HaveCount(3);
        result.Should().Contain(p => p.Name == "Casa en la playa");
        result.Should().Contain(p => p.Name == "Casa moderna");
        result.Should().Contain(p => p.Name == "Casa de campo");
    }

    [Test]
    public void Build_WithAddressFilter_ShouldReturnMatchingProperties()
    {
        // Arrange
        var spec = new PropertySpecificationBuilder()
            .WithAddress("Calle Sol 123")
            .Build();

        var expression = spec.ToExpression().Compile();

        // Act
        var result = _properties.Where(expression).ToList();

        // Assert
        result.Should().HaveCount(1);
        result[0].Name.Should().Be("Casa en la playa");
        result[0].Address.Should().Be("Calle Sol 123");
    }

    [Test]
    public void Build_WithPriceRangeFilter_ShouldReturnMatchingProperties()
    {
        // Arrange
        var spec = new PropertySpecificationBuilder()
            .WithRangePrice(200000m, 400000m)
            .Build();

        var expression = spec.ToExpression().Compile();

        // Act
        var result = _properties.Where(expression).ToList();

        // Assert
        result.Should().HaveCount(3);
        result.Should().Contain(p => p.Name == "Casa moderna");
        result.Should().Contain(p => p.Name == "Apartamento céntrico");
        result.Should().Contain(p => p.Name == "Casa de campo");
    }

    [Test]
    public void Build_WithNameAndPriceRange_ShouldReturnPropertiesMatchingBothFilters()
    {
        // Arrange
        var spec = new PropertySpecificationBuilder()
            .WithName("Casa")
            .WithRangePrice(200000m, 400000m)
            .Build();

        var expression = spec.ToExpression().Compile();

        // Act
        var result = _properties.Where(expression).ToList();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(p => p.Name == "Casa moderna");
        result.Should().Contain(p => p.Name == "Casa de campo");
    }

    [Test]
    public void Build_WithAllFilters_ShouldReturnPropertiesMatchingAllCriteria()
    {
        // Arrange
        var spec = new PropertySpecificationBuilder()
            .WithName("Casa")
            .WithAddress("Calle Sol 123")
            .WithRangePrice(100000m, 200000m)
            .Build();

        var expression = spec.ToExpression().Compile();

        // Act
        var result = _properties.Where(expression).ToList();

        // Assert
        result.Should().HaveCount(1);
        result[0].Name.Should().Be("Casa en la playa");
    }

    [Test]
    public void Build_WithNullName_ShouldIgnoreNameFilter()
    {
        // Arrange
        var spec = new PropertySpecificationBuilder()
            .WithName(null)
            .WithRangePrice(100000m, 200000m)
            .Build();

        var expression = spec.ToExpression().Compile();

        // Act
        var result = _properties.Where(expression).ToList();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(p => p.Name == "Casa en la playa");
        result.Should().Contain(p => p.Name == "Apartamento céntrico");
    }

    [Test]
    public void Build_WithEmptyName_ShouldIgnoreNameFilter()
    {
        // Arrange
        var spec = new PropertySpecificationBuilder()
            .WithName("")
            .WithRangePrice(100000m, 200000m)
            .Build();

        var expression = spec.ToExpression().Compile();

        // Act
        var result = _properties.Where(expression).ToList();

        // Assert
        result.Should().HaveCount(2);
    }

    [Test]
    public void Build_WithNullAddress_ShouldIgnoreAddressFilter()
    {
        // Arrange
        var spec = new PropertySpecificationBuilder()
            .WithAddress(null)
            .WithName("Casa")
            .Build();

        var expression = spec.ToExpression().Compile();

        // Act
        var result = _properties.Where(expression).ToList();

        // Assert
        result.Should().HaveCount(3);
    }

    [Test]
    public void Build_WithNullPriceRange_ShouldIgnorePriceFilter()
    {
        // Arrange
        var spec = new PropertySpecificationBuilder()
            .WithRangePrice(null, null)
            .WithName("Casa")
            .Build();

        var expression = spec.ToExpression().Compile();

        // Act
        var result = _properties.Where(expression).ToList();

        // Assert
        result.Should().HaveCount(3);
    }

    [Test]
    public void Build_WithOnlyMinPrice_ShouldIgnorePriceFilter()
    {
        // Arrange
        var spec = new PropertySpecificationBuilder()
            .WithRangePrice(200000m, null)
            .WithName("Casa")
            .Build();

        var expression = spec.ToExpression().Compile();

        // Act
        var result = _properties.Where(expression).ToList();

        // Assert
        result.Should().HaveCount(3);
    }

    [Test]
    public void Build_WithOnlyMaxPrice_ShouldIgnorePriceFilter()
    {
        // Arrange
        var spec = new PropertySpecificationBuilder()
            .WithRangePrice(null, 400000m)
            .WithName("Casa")
            .Build();

        var expression = spec.ToExpression().Compile();

        // Act
        var result = _properties.Where(expression).ToList();

        // Assert
        result.Should().HaveCount(3);
    }

    [Test]
    public void Build_WithStrictFilters_ShouldReturnEmptyWhenNoMatch()
    {
        // Arrange
        var spec = new PropertySpecificationBuilder()
            .WithName("Mansión")
            .WithAddress("Dirección inexistente")
            .WithRangePrice(1000000m, 2000000m)
            .Build();

        var expression = spec.ToExpression().Compile();

        // Act
        var result = _properties.Where(expression).ToList();

        // Assert
        result.Should().BeEmpty();
    }

    [Test]
    public void Build_ChainingShouldWorkInAnyOrder()
    {
        // Arrange
        var spec1 = new PropertySpecificationBuilder()
            .WithName("Casa")
            .WithAddress("Calle Sol 123")
            .WithRangePrice(100000m, 200000m)
            .Build();

        var spec2 = new PropertySpecificationBuilder()
            .WithRangePrice(100000m, 200000m)
            .WithAddress("Calle Sol 123")
            .WithName("Casa")
            .Build();

        var expression1 = spec1.ToExpression().Compile();
        var expression2 = spec2.ToExpression().Compile();

        // Act
        var result1 = _properties.Where(expression1).ToList();
        var result2 = _properties.Where(expression2).ToList();

        // Assert
        result1.Should().HaveCount(1);
        result2.Should().HaveCount(1);
        result1[0].Id.Should().Be(result2[0].Id);
    }
}
