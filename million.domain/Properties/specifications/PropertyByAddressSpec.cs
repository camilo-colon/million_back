using System.Linq.Expressions;
using million.domain.Common.specifications;

namespace million.domain.properties.specifications;

public class PropertyByAddressSpec(string address) : Specification<Property>
{
    public override Expression<Func<Property, bool>> ToExpression()
    {
        return p => p.Address == address;
    }
}