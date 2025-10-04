using System.Linq.Expressions;
using million.domain.Common.specifications;

namespace million.domain.properties.specifications;

public class PropertyByNameSpec(string name) : Specification<Property>
{
    public override Expression<Func<Property, bool>> ToExpression()
    {
        return p => p.Name.Equals(name);
    }
}