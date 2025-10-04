using million.domain.Common.specifications;
using million.domain.properties;
using million.infrastructure.Common.Persistence.mongodb;
using MongoDB.Bson;
using MongoDB.Driver;

namespace million.infrastructure.Properties.Persistence;

public class PropertyMongoRepository(IMongoDatabase database) : IPropertyRepository
{
    private readonly IMongoCollection<Property> _collection = database.GetCollection<Property>("properties");

    public Task<List<Property>> FindAsync(ISpecification<Property> spec, CancellationToken token)
    {
        var filters = SpecificationToMongoFilterConverter<Property>.Converter(spec);
        return _collection.Find(filters).ToListAsync(token);
    }

    public Task<List<Property>> AllAsync(CancellationToken token)
    {
        return _collection.Find(new BsonDocument()).ToListAsync(token);
    }
}