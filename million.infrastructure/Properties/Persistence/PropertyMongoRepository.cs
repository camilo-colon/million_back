using million.domain.properties;
using million.infrastructure.Common.Persistence.mongodb;
using MongoDB.Driver;

namespace million.infrastructure.Properties.Persistence;

public class PropertyMongoRepository(IMongoDatabase database) : GenericMongoRepository<Property>(database, "properties"), IPropertyRepository;