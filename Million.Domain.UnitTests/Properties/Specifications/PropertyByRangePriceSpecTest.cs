using FluentAssertions;
using million.domain.properties;
using million.domain.properties.specifications;

namespace Million.Domain.UnitTests.Properties.Specifications;

public class PropertyByRangePriceSpecTest
{
    [Test]
    public void ToExpression_WhenPriceIsWithinRange_ShouldReturnTrue()
    {
        // Arrange
        var spec = new PropertyByRangePriceSpec(100000m, 500000m);
        var property = new Property(
            Guid.NewGuid(),
            "Casa moderna",
            "Calle 123",
            250000m,
            "CODE001",
            2020);

        var expression = spec.ToExpression().Compile();

        // Act
        var result = expression(property);

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public void ToExpression_WhenPriceIsEqualToMinimum_ShouldReturnTrue()
    {
        // Arrange
        var spec = new PropertyByRangePriceSpec(100000m, 500000m);
        var property = new Property(
            Guid.NewGuid(),
            "Casa económica",
            "Calle 123",
            100000m,
            "CODE001",
            2020);

        var expression = spec.ToExpression().Compile();

        // Act
        var result = expression(property);

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public void ToExpression_WhenPriceIsEqualToMaximum_ShouldReturnTrue()
    {
        // Arrange
        var spec = new PropertyByRangePriceSpec(100000m, 500000m);
        var property = new Property(
            Guid.NewGuid(),
            "Casa lujosa",
            "Calle 123",
            500000m,
            "CODE001",
            2020);

        var expression = spec.ToExpression().Compile();

        // Act
        var result = expression(property);

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public void ToExpression_WhenPriceIsBelowMinimum_ShouldReturnFalse()
    {
        // Arrange
        var spec = new PropertyByRangePriceSpec(100000m, 500000m);
        var property = new Property(
            Guid.NewGuid(),
            "Casa muy económica",
            "Calle 123",
            99999m,
            "CODE001",
            2020);

        var expression = spec.ToExpression().Compile();

        // Act
        var result = expression(property);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void ToExpression_WhenPriceIsAboveMaximum_ShouldReturnFalse()
    {
        // Arrange
        var spec = new PropertyByRangePriceSpec(100000m, 500000m);
        var property = new Property(
            Guid.NewGuid(),
            "Casa muy lujosa",
            "Calle 123",
            500001m,
            "CODE001",
            2020);

        var expression = spec.ToExpression().Compile();

        // Act
        var result = expression(property);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void ToExpression_WhenFilteringMultipleProperties_ShouldReturnOnlyWithinRange()
    {
        // Arrange
        var spec = new PropertyByRangePriceSpec(200000m, 400000m);
        var properties = new List<Property>
        {
            new Property(Guid.NewGuid(), "Prop1", "Addr1", 100000m, "C1", 2020),
            new Property(Guid.NewGuid(), "Prop2", "Addr2", 200000m, "C2", 2021),
            new Property(Guid.NewGuid(), "Prop3", "Addr3", 300000m, "C3", 2022),
            new Property(Guid.NewGuid(), "Prop4", "Addr4", 400000m, "C4", 2023),
            new Property(Guid.NewGuid(), "Prop5", "Addr5", 500000m, "C5", 2024)
        };

        var expression = spec.ToExpression().Compile();

        // Act
        var result = properties.Where(expression).ToList();

        // Assert
        result.Should().HaveCount(3);
        result.Should().Contain(p => p.Price == 200000m);
        result.Should().Contain(p => p.Price == 300000m);
        result.Should().Contain(p => p.Price == 400000m);
    }

    [Test]
    public void ToExpression_WithZeroMinimum_ShouldWorkCorrectly()
    {
        // Arrange
        var spec = new PropertyByRangePriceSpec(0m, 100000m);
        var property = new Property(
            Guid.NewGuid(),
            "Casa gratis",
            "Calle 123",
            0m,
            "CODE001",
            2020);

        var expression = spec.ToExpression().Compile();

        // Act
        var result = expression(property);

        // Assert
        result.Should().BeTrue();
    }
}
