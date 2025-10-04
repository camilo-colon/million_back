using million.domain.Common.specifications;
using million.domain.properties;
using million.infrastructure.Common.Persistence.mongodb;
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
}