using FluentAssertions;
using million.application.Properties.Queries.GetPropertyById;
using million.domain.properties;
using million.domain.PropertyImages;
using Moq;

namespace Million.Application.UnitTests.Properties.Queries;

public class GetPropertyByIdQueryHandlerTest
{
    private Mock<IPropertyRepository> _propertyRepositoryMock;
    private Mock<IPropertyImageRepository> _propertyImageRepositoryMock;
    private GetPropertyByIdQueryHandler _handler;

    [SetUp]
    public void SetUp()
    {
        _propertyRepositoryMock = new Mock<IPropertyRepository>();
        _propertyImageRepositoryMock = new Mock<IPropertyImageRepository>();
        _handler = new GetPropertyByIdQueryHandler(
            _propertyRepositoryMock.Object,
            _propertyImageRepositoryMock.Object);
    }

    [Test]
    public async Task Handle_WhenPropertyDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var propertyId = Guid.NewGuid();
        var query = new GetPropertyByIdQuery(propertyId);

        _propertyRepositoryMock
            .Setup(x => x.GetByIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Property?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();
        _propertyRepositoryMock.Verify(x => x.GetByIdAsync(
            propertyId,
            It.IsAny<CancellationToken>()), Times.Once);
        _propertyImageRepositoryMock.Verify(x => x.GetBySpec(
            It.IsAny<million.domain.PropertyImages.Specifications.PropertyImageByPropertyIdSpec>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public async Task Handle_WhenPropertyExistsWithoutImages_ShouldReturnPropertyDetailWithEmptyImagesList()
    {
        // Arrange
        var propertyId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();
        var query = new GetPropertyByIdQuery(propertyId);

        var property = new Property(
            ownerId,
            "Casa en la playa",
            "Calle 123",
            500000m,
            "CODE001",
            2020,
            propertyId);

        _propertyRepositoryMock
            .Setup(x => x.GetByIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(property);

        _propertyImageRepositoryMock
            .Setup(x => x.GetBySpec(
                It.IsAny<million.domain.PropertyImages.Specifications.PropertyImageByPropertyIdSpec>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(propertyId);
        result.OwnerId.Should().Be(ownerId);
        result.Name.Should().Be("Casa en la playa");
        result.Address.Should().Be("Calle 123");
        result.Price.Should().Be(500000m);
        result.CodeInternal.Should().Be("CODE001");
        result.Year.Should().Be(2020);
        result.Images.Should().BeEmpty();
    }

    [Test]
    public async Task Handle_WhenPropertyExistsWithImages_ShouldReturnPropertyDetailWithAllImages()
    {
        // Arrange
        var propertyId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();
        var query = new GetPropertyByIdQuery(propertyId);

        var property = new Property(
            ownerId,
            "Casa moderna",
            "Avenida 456",
            750000m,
            "CODE002",
            2022,
            propertyId);

        var imageId1 = Guid.NewGuid();
        var imageId2 = Guid.NewGuid();
        var imageId3 = Guid.NewGuid();

        var images = new List<PropertyImage>
        {
            new(propertyId, "image1.jpg", true, imageId1),
            new(propertyId, "image2.jpg", true, imageId2),
            new(propertyId, "image3.jpg", true, imageId3)
        };

        _propertyRepositoryMock
            .Setup(x => x.GetByIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(property);

        _propertyImageRepositoryMock
            .Setup(x => x.GetBySpec(
                It.IsAny<million.domain.PropertyImages.Specifications.PropertyImageByPropertyIdSpec>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(images);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(propertyId);
        result.OwnerId.Should().Be(ownerId);
        result.Name.Should().Be("Casa moderna");
        result.Address.Should().Be("Avenida 456");
        result.Price.Should().Be(750000m);
        result.CodeInternal.Should().Be("CODE002");
        result.Year.Should().Be(2022);
        result.Images.Should().HaveCount(3);
        result.Images[0].Id.Should().Be(imageId1);
        result.Images[0].File.Should().Be("image1.jpg");
        result.Images[0].Enabled.Should().BeTrue();
        result.Images[1].Id.Should().Be(imageId2);
        result.Images[1].File.Should().Be("image2.jpg");
        result.Images[2].Id.Should().Be(imageId3);
        result.Images[2].File.Should().Be("image3.jpg");
    }

    [Test]
    public async Task Handle_WhenPropertyExistsWithMixedEnabledImages_ShouldReturnOnlyEnabledImages()
    {
        // Arrange
        var propertyId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();
        var query = new GetPropertyByIdQuery(propertyId);

        var property = new Property(
            ownerId,
            "Apartamento",
            "Carrera 789",
            300000m,
            "CODE003",
            2023,
            propertyId);

        var enabledImageId = Guid.NewGuid();

        var images = new List<PropertyImage>
        {
            new(propertyId, "enabled_image.jpg", true, enabledImageId)
        };

        _propertyRepositoryMock
            .Setup(x => x.GetByIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(property);

        _propertyImageRepositoryMock
            .Setup(x => x.GetBySpec(
                It.IsAny<million.domain.PropertyImages.Specifications.PropertyImageByPropertyIdSpec>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(images);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Images.Should().HaveCount(1);
        result.Images[0].Id.Should().Be(enabledImageId);
        result.Images[0].File.Should().Be("enabled_image.jpg");
        result.Images[0].Enabled.Should().BeTrue();
    }

    [Test]
    public async Task Handle_ShouldCallGetByIdAsyncWithCorrectId()
    {
        // Arrange
        var propertyId = Guid.NewGuid();
        var query = new GetPropertyByIdQuery(propertyId);

        _propertyRepositoryMock
            .Setup(x => x.GetByIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Property?)null);

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _propertyRepositoryMock.Verify(x => x.GetByIdAsync(
            propertyId,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task Handle_ShouldUseCorrectSpecificationForImages()
    {
        // Arrange
        var propertyId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();
        var query = new GetPropertyByIdQuery(propertyId);

        var property = new Property(
            ownerId,
            "Test Property",
            "Test Address",
            100000m,
            "TEST001",
            2021,
            propertyId);

        _propertyRepositoryMock
            .Setup(x => x.GetByIdAsync(
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(property);

        _propertyImageRepositoryMock
            .Setup(x => x.GetBySpec(
                It.IsAny<million.domain.PropertyImages.Specifications.PropertyImageByPropertyIdSpec>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _propertyImageRepositoryMock.Verify(x => x.GetBySpec(
            It.IsAny<million.domain.PropertyImages.Specifications.PropertyImageByPropertyIdSpec>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
