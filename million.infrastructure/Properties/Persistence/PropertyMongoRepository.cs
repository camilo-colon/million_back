using million.domain.properties;
using MongoDB.Bson;
using MongoDB.Driver;

namespace million.infrastructure.Properties.Persistence;

public class PropertyMongoRepository(IMongoDatabase database) : IPropertyRepository
{
    private readonly IMongoCollection<Property> _collection = database.GetCollection<Property>("properties");
    
    public async Task<List<Property>> Find()
    {
        return await _collection.Find(new BsonDocument()).ToListAsync();
    }
}