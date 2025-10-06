using System.Linq.Expressions;
using million.domain.Common.specifications;

namespace million.domain.PropertyImages.Specifications;

public class PropertyImageByPropertyIdSpec(Guid propertyId) : Specification<PropertyImage>
{
    public override Expression<Func<PropertyImage, bool>> ToExpression()
    {
        return p => p.PropertyId == propertyId && p.Enabled;
    }

}