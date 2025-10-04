using System.Linq.Expressions;
using million.domain.common;

namespace million.domain.Common.specifications;

public class IdentitySpecification<TEntity> : Specification<TEntity> where TEntity : Entity
{
    public override Expression<Func<TEntity, bool>> ToExpression()
    {
        return x => true;
    }
}