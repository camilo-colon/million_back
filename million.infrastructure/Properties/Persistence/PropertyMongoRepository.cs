using million.domain.properties;
using MongoDB.Driver;

namespace million.infrastructure.Properties.Persistence;

public class PropertyMongoRepository(IMongoDatabase database) : IPropertyRepository
{
    private readonly IMongoCollection<Property> _collection = database.GetCollection<Property>("properties");
    
    public Task<List<Property>> GetByFilters(string? name, string? address, string? price,  int limit, int offset)
    {
        throw new NotImplementedException();
    }
}