using FluentAssertions;
using million.domain.properties;
using million.domain.properties.specifications;

namespace Million.Domain.UnitTests.Properties.Specifications;

public class PropertyByNameSpecTest
{
    [Test]
    public void ToExpression_WhenPropertyNameContainsSearchTerm_ShouldReturnTrue()
    {
        // Arrange
        var spec = new PropertyByNameSpec("Casa");
        var property = new Property(
            Guid.NewGuid(),
            "Casa en la playa",
            "Address 1",
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
    public void ToExpression_WhenPropertyNameDoesNotContainSearchTerm_ShouldReturnFalse()
    {
        // Arrange
        var spec = new PropertyByNameSpec("Apartamento");
        var property = new Property(
            Guid.NewGuid(),
            "Casa en la playa",
            "Address 1",
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
    public void ToExpression_WhenSearchTermIsPartOfPropertyName_ShouldReturnTrue()
    {
        // Arrange
        var spec = new PropertyByNameSpec("playa");
        var property = new Property(
            Guid.NewGuid(),
            "Casa en la playa",
            "Address 1",
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
    public void ToExpression_WhenFilteringMultipleProperties_ShouldReturnOnlyMatching()
    {
        // Arrange
        var spec = new PropertyByNameSpec("Casa");
        var properties = new List<Property>
        {
            new Property(Guid.NewGuid(), "Casa grande", "Addr1", 100000m, "C1", 2020),
            new Property(Guid.NewGuid(), "Apartamento moderno", "Addr2", 200000m, "C2", 2021),
            new Property(Guid.NewGuid(), "Casa de campo", "Addr3", 300000m, "C3", 2022)
        };

        var expression = spec.ToExpression().Compile();

        // Act
        var result = properties.Where(expression).ToList();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(p => p.Name == "Casa grande");
        result.Should().Contain(p => p.Name == "Casa de campo");
    }
}
