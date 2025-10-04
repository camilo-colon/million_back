using System.Linq.Expressions;
using million.domain.common;

namespace million.domain.Common.specifications;

public abstract class Specification<TEntity> : ISpecification<TEntity> where TEntity : Entity
{
    public static readonly Specification<TEntity>  All = new IdentitySpecification<TEntity>();
    public abstract Expression<Func<TEntity, bool>> ToExpression();
    
    public Specification<TEntity> And(Specification<TEntity> specification)
    {
        return new AndSpecification<TEntity>(this, specification);
    }
}