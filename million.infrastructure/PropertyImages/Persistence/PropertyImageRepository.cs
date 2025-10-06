using million.domain.PropertyImages;
using million.infrastructure.Common.Persistence.mongodb;
using MongoDB.Driver;

namespace million.infrastructure.PropertyImages.Persistence;

public class PropertyImageRepository(IMongoDatabase database) : GenericMongoRepository<PropertyImage>(database, "property_images"), IPropertyImageRepository;