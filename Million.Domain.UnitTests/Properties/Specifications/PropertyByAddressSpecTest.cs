using FluentAssertions;
using million.domain.properties;
using million.domain.properties.specifications;

namespace Million.Domain.UnitTests.Properties.Specifications;

public class PropertyByAddressSpecTest
{
    [Test]
    public void ToExpression_WhenAddressMatches_ShouldReturnTrue()
    {
        // Arrange
        var spec = new PropertyByAddressSpec("Calle 123");
        var property = new Property(
            Guid.NewGuid(),
            "Casa moderna",
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
    public void ToExpression_WhenAddressDoesNotMatch_ShouldReturnFalse()
    {
        // Arrange
        var spec = new PropertyByAddressSpec("Calle 456");
        var property = new Property(
            Guid.NewGuid(),
            "Casa moderna",
            "Calle 123",
            100000m,
            "CODE001",
            2020);

        var expression = spec.ToExpression().Compile();

        // Act
        var result = expression(property);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void ToExpression_WhenAddressIsCaseSensitive_ShouldMatchExactly()
    {
        // Arrange
        var spec = new PropertyByAddressSpec("calle 123");
        var property = new Property(
            Guid.NewGuid(),
            "Casa moderna",
            "Calle 123",
            100000m,
            "CODE001",
            2020);

        var expression = spec.ToExpression().Compile();

        // Act
        var result = expression(property);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void ToExpression_WhenFilteringMultipleProperties_ShouldReturnOnlyExactMatch()
    {
        // Arrange
        var spec = new PropertyByAddressSpec("Avenida Siempre Viva 742");
        var properties = new List<Property>
        {
            new Property(Guid.NewGuid(), "Prop1", "Avenida Siempre Viva 742", 100000m, "C1", 2020),
            new Property(Guid.NewGuid(), "Prop2", "Calle Falsa 123", 200000m, "C2", 2021),
            new Property(Guid.NewGuid(), "Prop3", "Avenida Siempre Viva 742", 300000m, "C3", 2022)
        };

        var expression = spec.ToExpression().Compile();

        // Act
        var result = properties.Where(expression).ToList();

        // Assert
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(p => p.Address.Should().Be("Avenida Siempre Viva 742"));
    }
}
