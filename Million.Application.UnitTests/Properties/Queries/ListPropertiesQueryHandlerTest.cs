using million.application.Properties.Queries.ListProperties;
using million.domain.Common.specifications;
using million.domain.properties;
using million.domain.PropertyImages;

namespace Million.Application.UnitTests.Properties.Queries;

using FluentAssertions;
using Moq;

public class ListPropertiesQueryHandlerTest
{
    private Mock<IPropertyRepository> _propertyRepositoryMock;
    private Mock<IPropertyImageRepository> _propertyImageRepositoryMock;
    private ListPropertiesQueryHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _propertyRepositoryMock = new Mock<IPropertyRepository>();
        _propertyImageRepositoryMock = new Mock<IPropertyImageRepository>();
        _handler = new ListPropertiesQueryHandler(
            _propertyRepositoryMock.Object,
            _propertyImageRepositoryMock.Object);
    }

    [Test]
    public async Task Handle_WhenNoPropertiesExist_ShouldReturnEmptyList()
    {
        // Arrange
        var query = new ListPropertiesQuery();

        _propertyRepositoryMock
            .Setup(x => x.GetBySpec(
                It.IsAny<Specification<Property>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
        _propertyRepositoryMock.Verify(x => x.GetBySpec(
            It.IsAny<Specification<Property>>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Handle_WhenPropertiesExistWithoutImages_ShouldReturnPropertiesWithEmptyImagesList()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var propertyId = Guid.NewGuid();
        var query = new ListPropertiesQuery();

        var properties = new List<Property>
        {
            new Property(
                ownerId,
                "Casa en la playa",
                "Calle 123",
                500000m,
                "CODE001",
                2020,
                propertyId)
        };

        _propertyRepositoryMock
            .Setup(x => x.GetBySpec(
                It.IsAny<Specification<Property>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(properties);

        _propertyImageRepositoryMock
            .Setup(x => x.GetBySpec(
                It.IsAny<Specification<PropertyImage>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        result[0].IdOwner.Should().Be(ownerId);
        result[0].Name.Should().Be("Casa en la playa");
        result[0].Address.Should().Be("Calle 123");
        result[0].Price.Should().Be(500000m);
        result[0].Image.Should().BeEmpty();
    }

    [Test]
    public async Task Handle_WhenPropertiesExistWithImages_ShouldReturnPropertiesWithImages()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var propertyId = Guid.NewGuid();
        var query = new ListPropertiesQuery();

        var properties = new List<Property>
        {
            new Property(
                ownerId,
                "Casa moderna",
                "Avenida 456",
                750000m,
                "CODE002",
                2022,
                propertyId)
        };

        var images = new List<PropertyImage>
        {
            new PropertyImage(propertyId, "image1.jpg", true),
            new PropertyImage(propertyId, "image2.jpg", true),
            new PropertyImage(propertyId, "image3.jpg", true)
        };

        _propertyRepositoryMock
            .Setup(x => x.GetBySpec(
                It.IsAny<Specification<Property>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(properties);

        _propertyImageRepositoryMock
            .Setup(x => x.GetBySpec(
                It.IsAny<Specification<PropertyImage>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(images);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        result[0].IdOwner.Should().Be(ownerId);
        result[0].Name.Should().Be("Casa moderna");
        result[0].Image.Should().Be("image1.jpg");
    }

    [Test]
    public async Task Handle_WhenMultiplePropertiesExist_ShouldReturnAllWithTheirRespectiveImages()
    {
        // Arrange
        var owner1Id = Guid.NewGuid();
        var owner2Id = Guid.NewGuid();
        var property1Id = Guid.NewGuid();
        var property2Id = Guid.NewGuid();
        var query = new ListPropertiesQuery();

        var properties = new List<Property>
        {
            new(owner1Id, "Propiedad 1", "Dirección 1", 100000m, "CODE001", 2020, property1Id),
            new(owner2Id, "Propiedad 2", "Dirección 2", 200000m, "CODE002", 2021, property2Id)
        };

        var imagesProperty1 = new List<PropertyImage>
        {
            new(property1Id, "prop1_img1.jpg", true)
        };

        var imagesProperty2 = new List<PropertyImage>
        {
            new(property2Id, "prop2_img1.jpg", true),
            new(property2Id, "prop2_img2.jpg", true)
        };

        _propertyRepositoryMock
            .Setup(x => x.GetBySpec(
                It.IsAny<Specification<Property>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(properties);

        _propertyImageRepositoryMock
            .SetupSequence(x => x.GetBySpec(
                It.IsAny<Specification<PropertyImage>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(imagesProperty1)
            .ReturnsAsync(imagesProperty2);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);

        result[0].IdOwner.Should().Be(owner1Id);
        result[0].Name.Should().Be("Propiedad 1");
        result[0].Image.Should().Be("prop1_img1.jpg");

        result[1].IdOwner.Should().Be(owner2Id);
        result[1].Name.Should().Be("Propiedad 2");
        result[1].Image.Should().Be("prop2_img1.jpg");
    }

    [Test]
    public async Task Handle_WithFilters_ShouldPassSpecificationToRepository()
    {
        // Arrange
        var query = new ListPropertiesQuery
        {
            Name = "Casa",
            Address = "Calle 123",
            MinPrice = 100000m,
            MaxPrice = 500000m
        };

        _propertyRepositoryMock
            .Setup(x => x.GetBySpec(
                It.IsAny<Specification<Property>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        _propertyRepositoryMock.Verify(x => x.GetBySpec(
            It.IsAny<Specification<Property>>(),
            It.IsAny<CancellationToken>()), Times.Once);
        result.Should().BeEmpty();
    }

    [Test]
    public async Task Handle_ShouldQueryImagesForEachProperty()
    {
        // Arrange
        var property1Id = Guid.NewGuid();
        var property2Id = Guid.NewGuid();
        var query = new ListPropertiesQuery();

        var properties = new List<Property>
        {
            new(Guid.NewGuid(), "Prop1", "Addr1", 100000m, "C1", 2020, property1Id),
            new(Guid.NewGuid(), "Prop2", "Addr2", 200000m, "C2", 2021, property2Id)
        };

        _propertyRepositoryMock
            .Setup(x => x.GetBySpec(
                It.IsAny<Specification<Property>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(properties);

        _propertyImageRepositoryMock
            .Setup(x => x.GetBySpec(
                It.IsAny<Specification<PropertyImage>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _propertyImageRepositoryMock.Verify(x => x.GetBySpec(
            It.IsAny<Specification<PropertyImage>>(),
            It.IsAny<CancellationToken>()), Times.Exactly(2));
    }
}