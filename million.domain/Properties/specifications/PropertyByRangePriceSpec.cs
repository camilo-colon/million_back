using System.Linq.Expressions;
using million.domain.Common.specifications;

namespace million.domain.properties.specifications;

public class PropertyByRangePriceSpec(decimal min, decimal max) : Specification<Property>
{
    public override Expression<Func<Property, bool>> ToExpression()
    {
        return p => p.Price >= min && p.Price <= max;
    }
}