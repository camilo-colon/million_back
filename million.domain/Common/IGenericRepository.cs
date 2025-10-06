using million.domain.Common.specifications;

namespace million.domain.common;

public interface IGenericRepository<TEntity> where TEntity :  Entity
{
    public Task<List<TEntity>> GetBySpec(ISpecification<TEntity> spec, CancellationToken token);
}