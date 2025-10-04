using System.Linq.Expressions;
using million.domain.common;

namespace million.domain.Common.specifications;

public class AndSpecification<T>(Specification<T> left, Specification<T> right) : Specification<T>
    where T : Entity
{
    public override Expression<Func<T, bool>> ToExpression()
    {
        var leftExpression = left.ToExpression();
        var rightExpression = right.ToExpression();
        var parameter = Expression.Parameter(typeof(T));
        var body = Expression.AndAlso(leftExpression, rightExpression);
        return Expression.Lambda<Func<T, bool>>(body, parameter);
    }
}