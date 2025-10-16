using System.Linq.Expressions;

namespace million.domain.Common.specifications;

public class EntityByIdSpec<TEntity>(Guid id) : Specification<TEntity> where TEntity : common.Entity
{
    public override Expression<Func<TEntity, bool>> ToExpression()
    {
        return entity => entity.Id == id;
    }
}
