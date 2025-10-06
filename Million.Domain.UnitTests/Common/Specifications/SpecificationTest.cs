using System.Linq.Expressions;
using FluentAssertions;
using million.domain.Common.specifications;
using million.domain.properties;
using million.domain.properties.specifications;

namespace Million.Domain.UnitTests.Common.Specifications;

public class SpecificationTest
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
            new Property(Guid.NewGuid(), "Villa de lujo", "Boulevard Estrella 101", 800000m, "C4", 2023)
        };
    }

    [Test]
    public void All_ShouldReturnAllEntities()
    {
        // Arrange
        var spec = Specification<Property>.All;
        var expression = spec.ToExpression().Compile();

        // Act
        var result = _properties.Where(expression).ToList();

        // Assert
        result.Should().HaveCount(4);
        result.Should().BeEquivalentTo(_properties);
    }

    [Test]
    public void And_WithTwoSpecifications_ShouldCombineConditions()
    {
        // Arrange
        var nameSpec = new PropertyByNameSpec("Casa");
        var priceSpec = new PropertyByRangePriceSpec(100000m, 200000m);

        var combinedSpec = nameSpec.And(priceSpec);
        var expression = combinedSpec.ToExpression().Compile();

        // Act
        var result = _properties.Where(expression).ToList();

        // Assert
        result.Should().HaveCount(1);
        result[0].Name.Should().Be("Casa en la playa");
        result[0].Price.Should().BeInRange(100000m, 200000m);
    }

    [Test]
    public void And_WithMultipleChainedSpecifications_ShouldCombineAllConditions()
    {
        // Arrange
        var nameSpec = new PropertyByNameSpec("Casa");
        var addressSpec = new PropertyByAddressSpec("Calle Sol 123");
        var priceSpec = new PropertyByRangePriceSpec(100000m, 200000m);

        var combinedSpec = nameSpec
            .And(addressSpec)
            .And(priceSpec);

        var expression = combinedSpec.ToExpression().Compile();

        // Act
        var result = _properties.Where(expression).ToList();

        // Assert
        result.Should().HaveCount(1);
        result[0].Name.Should().Be("Casa en la playa");
        result[0].Address.Should().Be("Calle Sol 123");
        result[0].Price.Should().Be(150000m);
    }

    [Test]
    public void And_WithSpecificationAll_ShouldReturnSameAsOriginalSpec()
    {
        // Arrange
        var nameSpec = new PropertyByNameSpec("Casa");
        var combinedSpec = nameSpec.And(Specification<Property>.All);

        var nameExpression = nameSpec.ToExpression().Compile();
        var combinedExpression = combinedSpec.ToExpression().Compile();

        // Act
        var resultName = _properties.Where(nameExpression).ToList();
        var resultCombined = _properties.Where(combinedExpression).ToList();

        // Assert
        resultCombined.Should().HaveCount(2);
        resultCombined.Should().BeEquivalentTo(resultName);
    }

    [Test]
    public void And_WithNonMatchingSpecifications_ShouldReturnEmpty()
    {
        // Arrange
        var nameSpec = new PropertyByNameSpec("Casa");
        var addressSpec = new PropertyByAddressSpec("Dirección inexistente");

        var combinedSpec = nameSpec.And(addressSpec);
        var expression = combinedSpec.ToExpression().Compile();

        // Act
        var result = _properties.Where(expression).ToList();

        // Assert
        result.Should().BeEmpty();
    }

    [Test]
    public void And_Order_ShouldNotMatterForResults()
    {
        // Arrange
        var nameSpec = new PropertyByNameSpec("Casa");
        var priceSpec = new PropertyByRangePriceSpec(100000m, 200000m);

        var spec1 = nameSpec.And(priceSpec);
        var spec2 = priceSpec.And(nameSpec);

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

    [Test]
    public void ToExpression_ShouldReturnValidLinqExpression()
    {
        // Arrange
        var spec = new PropertyByNameSpec("Casa");

        // Act
        var expression = spec.ToExpression();

        // Assert
        expression.Should().NotBeNull();
        expression.Should().BeAssignableTo<Expression<Func<Property, bool>>>();
    }

    [Test]
    public void And_WithThreeSpecifications_ShouldApplyAllFilters()
    {
        // Arrange
        var spec1 = new PropertyByNameSpec("Casa");
        var spec2 = new PropertyByRangePriceSpec(300000m, 500000m);
        var spec3 = new PropertyByAddressSpec("Avenida Luna 456");

        var combinedSpec = spec1.And(spec2).And(spec3);
        var expression = combinedSpec.ToExpression().Compile();

        // Act
        var result = _properties.Where(expression).ToList();

        // Assert
        result.Should().HaveCount(1);
        result[0].Name.Should().Be("Casa moderna");
        result[0].Price.Should().Be(350000m);
        result[0].Address.Should().Be("Avenida Luna 456");
    }
}
