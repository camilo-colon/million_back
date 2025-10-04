using System.Linq.Expressions;

namespace million.domain.common.extensions;

public static class ExpressionExtension
{
    public static Expression<Func<T, bool>> And<T>(
        this Expression<Func<T, bool>> expr1, 
        Expression<Func<T, bool>> expr2)
    {
        var param = Expression.Parameter(typeof(T));
        var body1 = new ParameterReplacer(expr1.Parameters[0], param).Visit(expr1.Body);
        var body2 = new ParameterReplacer(expr2.Parameters[0], param).Visit(expr2.Body);
        var body = Expression.AndAlso(body1, body2);
        return Expression.Lambda<Func<T, bool>>(body, param);
    }
    
    private class ParameterReplacer(ParameterExpression old, ParameterExpression @new) : ExpressionVisitor
    {
        protected override Expression VisitParameter(ParameterExpression node) 
            => node == old ? @new : node;
    }
}