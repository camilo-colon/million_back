using million.domain.common;
using million.domain.Common.specifications;
using MongoDB.Driver;

namespace million.infrastructure.Common.Persistence.mongodb;

public class GenericMongoRepository<TEntity>(IMongoDatabase database, string collection) : IGenericRepository<TEntity> where TEntity :  Entity
{
    private readonly IMongoCollection<TEntity> _collection = database.GetCollection<TEntity>(collection);
    
    public async Task<List<TEntity>> GetBySpec(ISpecification<TEntity> spec, CancellationToken token)
    {
        var filters = SpecificationToMongoFilterConverter<TEntity>.Converter(spec);
        return await _collection.Find(filters).ToListAsync(token);
    }
}