using million.domain.common;
using million.domain.Common.specifications;
using MongoDB.Driver;

namespace million.infrastructure.Common.Persistence.mongodb;

public static class SpecificationToMongoFilterConverter<TEntity> where TEntity : Entity
{
    public static FilterDefinition<TEntity> Converter(ISpecification<TEntity> spec)
    {
        var expression = spec.ToExpression();
        return Builders<TEntity>.Filter.Where(expression);
    }
}