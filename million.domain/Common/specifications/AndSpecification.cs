using System.Linq.Expressions;
using million.domain.common;
using million.domain.common.extensions;

namespace million.domain.Common.specifications;

public class AndSpecification<T>(Specification<T> left, Specification<T> right) : Specification<T>
    where T : Entity
{
    public override Expression<Func<T, bool>> ToExpression()
    {
        return left.ToExpression().And(right.ToExpression());
    }
}