using FluentAssertions;
using million.domain.Common.specifications;
using million.domain.properties;
using million.domain.PropertyImages;
using million.infrastructure.Properties.Persistence;
using million.infrastructure.PropertyImages.Persistence;

namespace Million.Infrastructure.IntegrationTests.Properties.Queries;

[TestFixture]
public class GetPropertyByIdIntegrationTest
{
    private PropertyMongoRepository _propertyRepository = null!;
    private PropertyImageRepository _propertyImageRepository = null!;

    [SetUp]
    public async Task SetUp()
    {
        await MongoDbFixture.CleanDatabase();
        _propertyRepository = new PropertyMongoRepository(MongoDbFixture.Database);
        _propertyImageRepository = new PropertyImageRepository(MongoDbFixture.Database);
    }

    [Test]
    public async Task GetByIdAsync_WhenPropertyExists_ShouldReturnProperty()
    {
        // Arrange
        var propertyId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();
        var property = new Property(
            ownerId,
            "Casa en la playa",
            "Calle Sol 123",
            500000m,
            "CODE001",
            2020,
            propertyId);

        var collection = MongoDbFixture.Database.GetCollection<Property>("properties");
        await collection.InsertOneAsync(property);

        // Act
        var result = await _propertyRepository.GetByIdAsync(propertyId, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(propertyId);
        result.OwnerId.Should().Be(ownerId);
        result.Name.Should().Be("Casa en la playa");
        result.Address.Should().Be("Calle Sol 123");
        result.Price.Should().Be(500000m);
        result.CodeInternal.Should().Be("CODE001");
        result.Year.Should().Be(2020);
    }

    [Test]
    public async Task GetByIdAsync_WhenPropertyDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var nonExistentPropertyId = Guid.NewGuid();

        // Act
        var result = await _propertyRepository.GetByIdAsync(nonExistentPropertyId, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public async Task GetPropertyWithImages_ShouldReturnPropertyAndAllItsImages()
    {
        // Arrange
        var propertyId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();

        var property = new Property(
            ownerId,
            "Casa moderna",
            "Avenida Luna 456",
            750000m,
            "CODE002",
            2022,
            propertyId);

        var images = new List<PropertyImage>
        {
            new(propertyId, "image1.jpg", true),
            new(propertyId, "image2.jpg", true),
            new(propertyId, "image3.jpg", true)
        };

        var propertyCollection = MongoDbFixture.Database.GetCollection<Property>("properties");
        await propertyCollection.InsertOneAsync(property);

        var imagesCollection = MongoDbFixture.Database.GetCollection<PropertyImage>("property_images");
        await imagesCollection.InsertManyAsync(images);

        var imagesSpec = new million.domain.PropertyImages.Specifications.PropertyImageByPropertyIdSpec(propertyId);

        // Act
        var propertyResult = await _propertyRepository.GetByIdAsync(propertyId, CancellationToken.None);
        var imagesResult = await _propertyImageRepository.GetBySpec(imagesSpec, CancellationToken.None);

        // Assert
        propertyResult.Should().NotBeNull();
        propertyResult!.Id.Should().Be(propertyId);
        propertyResult.Name.Should().Be("Casa moderna");

        imagesResult.Should().HaveCount(3);
        imagesResult.Should().AllSatisfy(img =>
        {
            img.PropertyId.Should().Be(propertyId);
            img.Enabled.Should().BeTrue();
        });
    }

    [Test]
    public async Task GetPropertyWithImages_WhenPropertyHasNoImages_ShouldReturnPropertyWithEmptyImagesList()
    {
        // Arrange
        var propertyId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();

        var property = new Property(
            ownerId,
            "Apartamento sin fotos",
            "Carrera 789",
            300000m,
            "CODE003",
            2023,
            propertyId);

        var propertyCollection = MongoDbFixture.Database.GetCollection<Property>("properties");
        await propertyCollection.InsertOneAsync(property);

        var imagesSpec = new million.domain.PropertyImages.Specifications.PropertyImageByPropertyIdSpec(propertyId);

        // Act
        var propertyResult = await _propertyRepository.GetByIdAsync(propertyId, CancellationToken.None);
        var imagesResult = await _propertyImageRepository.GetBySpec(imagesSpec, CancellationToken.None);

        // Assert
        propertyResult.Should().NotBeNull();
        propertyResult!.Id.Should().Be(propertyId);
        imagesResult.Should().BeEmpty();
    }

    [Test]
    public async Task GetPropertyWithImages_ShouldOnlyReturnEnabledImages()
    {
        // Arrange
        var propertyId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();

        var property = new Property(
            ownerId,
            "Casa con imágenes mixtas",
            "Diagonal 321",
            450000m,
            "CODE004",
            2024,
            propertyId);

        var images = new List<PropertyImage>
        {
            new(propertyId, "enabled_image1.jpg", true),
            new(propertyId, "disabled_image.jpg", false),
            new(propertyId, "enabled_image2.jpg", true)
        };

        var propertyCollection = MongoDbFixture.Database.GetCollection<Property>("properties");
        await propertyCollection.InsertOneAsync(property);

        var imagesCollection = MongoDbFixture.Database.GetCollection<PropertyImage>("property_images");
        await imagesCollection.InsertManyAsync(images);

        var imagesSpec = new million.domain.PropertyImages.Specifications.PropertyImageByPropertyIdSpec(propertyId);

        // Act
        var imagesResult = await _propertyImageRepository.GetBySpec(imagesSpec, CancellationToken.None);

        // Assert
        imagesResult.Should().HaveCount(2);
        imagesResult.Should().AllSatisfy(img => img.Enabled.Should().BeTrue());
        imagesResult.Should().NotContain(img => img.File == "disabled_image.jpg");
    }

    [Test]
    public async Task GetPropertyWithImages_MultipleProperties_ShouldReturnCorrectImagesForEachProperty()
    {
        // Arrange
        var property1Id = Guid.NewGuid();
        var property2Id = Guid.NewGuid();

        var property1 = new Property(
            Guid.NewGuid(),
            "Propiedad 1",
            "Dirección 1",
            100000m,
            "CODE001",
            2020,
            property1Id);

        var property2 = new Property(
            Guid.NewGuid(),
            "Propiedad 2",
            "Dirección 2",
            200000m,
            "CODE002",
            2021,
            property2Id);

        var property1Images = new List<PropertyImage>
        {
            new(property1Id, "prop1_img1.jpg", true),
            new(property1Id, "prop1_img2.jpg", true)
        };

        var property2Images = new List<PropertyImage>
        {
            new(property2Id, "prop2_img1.jpg", true)
        };

        var propertyCollection = MongoDbFixture.Database.GetCollection<Property>("properties");
        await propertyCollection.InsertManyAsync(new[] { property1, property2 });

        var imagesCollection = MongoDbFixture.Database.GetCollection<PropertyImage>("property_images");
        await imagesCollection.InsertManyAsync(property1Images.Concat(property2Images));

        var property1ImagesSpec = new million.domain.PropertyImages.Specifications.PropertyImageByPropertyIdSpec(property1Id);
        var property2ImagesSpec = new million.domain.PropertyImages.Specifications.PropertyImageByPropertyIdSpec(property2Id);

        // Act
        var property1ImagesResult = await _propertyImageRepository.GetBySpec(property1ImagesSpec, CancellationToken.None);
        var property2ImagesResult = await _propertyImageRepository.GetBySpec(property2ImagesSpec, CancellationToken.None);

        // Assert
        property1ImagesResult.Should().HaveCount(2);
        property1ImagesResult.Should().AllSatisfy(img => img.PropertyId.Should().Be(property1Id));

        property2ImagesResult.Should().HaveCount(1);
        property2ImagesResult.Should().AllSatisfy(img => img.PropertyId.Should().Be(property2Id));
    }
}
