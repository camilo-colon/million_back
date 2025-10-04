using System.Linq.Expressions;
using million.domain.common;

namespace million.domain.Common.specifications;

public interface ISpecification<TEntity> where TEntity : Entity
{
    public Expression<Func<TEntity, bool>> ToExpression();
}