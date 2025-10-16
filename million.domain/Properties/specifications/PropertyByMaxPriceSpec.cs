using System.Linq.Expressions;
using million.domain.Common.specifications;

namespace million.domain.properties.specifications;

public class PropertyByMaxPriceSpec(decimal price) : Specification<Property>
{
    public override Expression<Func<Property, bool>> ToExpression()
    {
        return p => p.Price <= price;
    }
}